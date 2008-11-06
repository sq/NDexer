using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Data.SQLite;
using Squared.Task;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using Squared.Task.Data;
using Squared.Task.IO;
using System.Reflection;
using System.Text;

namespace Ndexer {
    public class ActiveWorker : IDisposable {
        public string Description = null;

        public ActiveWorker (string description) {
            Description = description;
            Resume();
        }

        public void Resume () {
            lock (Program.ActiveWorkers)
                Program.ActiveWorkers.Add(this);
        }

        public void Suspend () {
            lock (Program.ActiveWorkers)
                Program.ActiveWorkers.Remove(this);
        }

        public void Dispose () {
            Suspend();
        }
    }

    public class TagGroup : List<Tag> {
        public string Filename;
        public long Timestamp;

        public TagGroup (string filename, long timestamp)
            : base() {
            Filename = filename;
            Timestamp = timestamp;
        }

        public IEnumerator<object> Commit () {
            var transaction = Program.Database.Connection.CreateTransaction();
            yield return transaction;

            yield return Program.Database.DeleteTagsForFile(Filename);

            yield return Program.Database.MakeSourceFileID(Filename, Timestamp);

            foreach (var tag in this)
                yield return Program.Database.AddTag(tag);

            yield return transaction.Commit();
        }

        public override string ToString () {
            return String.Format("TagGroup(fn={0}, ts={1}) {2} item(s)", Filename, Timestamp, Count);
        }
    }

    public static class Program {
        public const string Revision = "$Rev$";

        public static TaskScheduler Scheduler;
        public static TagDatabase Database;
        public static List<ActiveWorker> ActiveWorkers = new List<ActiveWorker>();
        public static NotifyIcon NotifyIcon;
        public static Icon Icon_Monitoring;
        public static Icon Icon_Working_1, Icon_Working_2;
        public static ContextMenuStrip ContextMenu;
        public static string TrayCaption;
        public static string DatabasePath;
        public static Dictionary<string, TagGroup> TagGroups = new Dictionary<string, TagGroup>();

        private const int BatchSize = 256;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main (string[] argv) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (argv.Length < 1) {
                MessageBox.Show(
                    "NDexer cannot start without a path specified for the index database on the command line.\n" +
                    @"For example: ndexer.exe C:\mysource\index.db",
                    "NDexer Error"
                );
                return;
            }

            DatabasePath = System.IO.Path.GetFullPath(argv[0]);

            if (!System.IO.File.Exists(DatabasePath))
                System.IO.File.Copy(GetDataPath() + @"\ndexer.db", DatabasePath);

            Scheduler = new TaskScheduler(JobQueue.WindowsMessageBased);

            Database = new TagDatabase(Scheduler, DatabasePath);

            Icon_Monitoring = Icon.FromHandle(Properties.Resources.database_monitoring.GetHicon());
            Icon_Working_1 = Icon.FromHandle(Properties.Resources.database_working_1.GetHicon());
            Icon_Working_2 = Icon.FromHandle(Properties.Resources.database_working_2.GetHicon());

            ContextMenu = new ContextMenuStrip();
            ContextMenu.Items.Add(
                "&Search", null,
                (e, s) => {
                    Scheduler.Start(ShowSearchTask(), TaskExecutionPolicy.RunAsBackgroundTask);
                }
            );
            ContextMenu.Items.Add(
                "&Configure", null,
                (e, s) => {
                    using (var dialog = new ConfigurationDialog(Database))
                        if (dialog.ShowDialog() == DialogResult.OK)
                            Scheduler.Start(RestartTask(), TaskExecutionPolicy.RunAsBackgroundTask);
                }
            );
            ContextMenu.Items.Add(
                "&Rebuild Index", null,
                (e, s) => {
                    Scheduler.Start(ConfirmRebuildIndexTask(), TaskExecutionPolicy.RunAsBackgroundTask);
                }
            );
            ContextMenu.Items.Add("-");
            ContextMenu.Items.Add(
                "E&xit", null,
                (e, s) => {
                    Scheduler.Start(ExitTask(), TaskExecutionPolicy.RunAsBackgroundTask);
                }
            );

            NotifyIcon = new NotifyIcon();
            NotifyIcon.ContextMenuStrip = ContextMenu;
            NotifyIcon.DoubleClick += (EventHandler)((s, e) => {
                Scheduler.Start(ShowSearchTask(), TaskExecutionPolicy.RunAsBackgroundTask);
            });

            Scheduler.Start(
                RefreshTrayIcon(),
                TaskExecutionPolicy.RunAsBackgroundTask
            );

            Scheduler.Start(
                MainTask(argv),
                TaskExecutionPolicy.RunAsBackgroundTask
            );

            Application.Run();
        }

        public static string GetExecutablePath() {
            string executablePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath)
                .ToLower().Replace(@"\bin\debug", "").Replace(@"\bin\release", "");
            return executablePath;
        }

        public static string GetCTagsPath() {
            return GetExecutablePath() + @"\ctags\ctags.exe";
        }

        public static string GetDataPath () {
            return GetExecutablePath() + @"\data\";
        }

        public static IEnumerator<object> ConfirmRebuildIndexTask () {
            if (MessageBox.Show(
                        "Are you sure you want to rebuild the index? This will take a while!", "Rebuild Index",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    ) == DialogResult.Yes) {
                var trans = Database.Connection.CreateTransaction();
                yield return trans;
                yield return Database.Connection.ExecuteSQL("DELETE FROM Tags; DELETE FROM SourceFiles; DELETE FROM TagContexts; DELETE FROM TagKinds; DELETE FROM TagLanguages");
                yield return trans.Commit();
                yield return RestartTask();
            }
        }

        public static void ShowConfiguration () {
            var dlg = new ConfigurationDialog(Database);
            dlg.ShowDialog();
            dlg.Dispose();
        }

        public static string[] GetLanguageNames () {
            var buffer = new List<string>();

            var info = new ProcessStartInfo(GetCTagsPath(), "--list-languages");
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;

            using (var process = Process.Start(info)) {
                process.WaitForExit();
                while (!process.StandardOutput.EndOfStream)
                    buffer.Add(process.StandardOutput.ReadLine());
            }

            return buffer.ToArray();
        }

        public static string[] GetDirectorNames () {
            var results = new List<string>();
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var baseType = typeof(Director);
            foreach (var type in types) {
                if (type.IsSubclassOf(baseType) && type.Name.EndsWith("Director"))
                    results.Add(type.Name.Replace("Director", ""));
            }
            results.Sort();
            return results.ToArray();
        }

        public static IBasicDirector GetDirector () {
            var editorName = (string)Scheduler.WaitFor(Database.GetPreference("TextEditor.Name"));
            var editorPath = (string)Scheduler.WaitFor(Database.GetPreference("TextEditor.Location"));
            var directorType = Type.GetType(String.Format("Ndexer.{0}Director", editorName), true, true);
            var constructor = directorType.GetConstructor(new Type[] { typeof(string) });
            var director = (IBasicDirector)constructor.Invoke(new object[] { editorPath });
            return director;
        }

        public static IEnumerator<object> ShowSearchTask () {
            var rtc = new RunToCompletion(Database.OpenReadConnection());
            yield return rtc;
            var conn = (ConnectionWrapper)rtc.Result;
            var dialog = new SearchDialog(conn);
            dialog.Show();
        }

        private static IEnumerator<object> TeardownTask () {
            yield return Database.Dispose();

            NotifyIcon.Visible = false;
        }

        public static IEnumerator<object> RestartTask () {
            yield return TeardownTask();

            Application.Restart();
        }

        public static IEnumerator<object> ExitTask () {
            yield return TeardownTask();

            Application.Exit();
        }

        public static IEnumerator<object> AutoShowConfiguration (string[] argv) {
            bool show = false;
            Future f;

            if (argv.Contains("--configure")) {
                show = true;
            } else {
                {
                    var iter = new TaskIterator<TagDatabase.Folder>(Database.GetFolders());
                    yield return iter.Start();
                    f = iter.ToArray();
                }

                yield return f;

                if (((TagDatabase.Folder[])f.Result).Length == 0) {
                    show = true;
                } else {
                    {
                        var iter = new TaskIterator<TagDatabase.Filter>(Database.GetFilters());
                        yield return iter.Start();
                        f = iter.ToArray();
                    }

                    yield return f;

                    if (((TagDatabase.Filter[])f.Result).Length == 0)
                        show = true;
                }
            }

            if (show)
                ShowConfiguration();
        }

        public static IEnumerator<object> MainTask (string[] argv) {
            yield return Database.Initialize();

            yield return AutoShowConfiguration(argv);

            using (new ActiveWorker("Compacting index")) {
                yield return Database.Compact();
            }

            var sourceFiles = new BlockingQueue<string>();

            if (argv.Contains("--noscan"))
                Scheduler.Start(
                    MonitorForChanges(sourceFiles),
                    TaskExecutionPolicy.RunAsBackgroundTask
                );
            else
                Scheduler.Start(
                    ScanThenMonitor(sourceFiles),
                    TaskExecutionPolicy.RunAsBackgroundTask
                );

            Scheduler.Start(
                UpdateIndex(sourceFiles),
                TaskExecutionPolicy.RunAsBackgroundTask
            );
        }

        public static IEnumerator<object> ScanThenMonitor (BlockingQueue<string> sourceFiles) {
            yield return ScanFiles(sourceFiles);

            yield return MonitorForChanges(sourceFiles);
        }

        public static IEnumerator<object> RefreshTrayIcon () {
            bool toggle = false;

            while (true) {
                toggle = !toggle;

                Icon theIcon;
                string statusMessage = "Idle";
                int numWorkers = 0;
                lock (ActiveWorkers)
                    numWorkers = ActiveWorkers.Count;
                if (numWorkers > 0) {
                    theIcon = (toggle) ? Icon_Working_1 : Icon_Working_2;
                    lock (ActiveWorkers)
                        statusMessage = ActiveWorkers[0].Description;
                } else {
                    theIcon = Icon_Monitoring;
                }

                TrayCaption = String.Format("NDexer ({0}) - {1}", System.IO.Path.GetFileName(DatabasePath), statusMessage);
                if (TrayCaption.Length >= 64)
                    TrayCaption = TrayCaption.Substring(0, 60) + "...";

                if (NotifyIcon.Icon != theIcon)
                    NotifyIcon.Icon = theIcon;
                if (NotifyIcon.Text != TrayCaption)
                    NotifyIcon.Text = TrayCaption;
                if (NotifyIcon.Visible == false)
                    NotifyIcon.Visible = true;

                yield return new Sleep(0.5);
            }
        }

        public static IEnumerator<object> ScanFiles (BlockingQueue<string> sourceFiles) {
            using (new ActiveWorker("Scanning folders for changes")) {
                var changeSet = new TaskIterator<TagDatabase.Change>(
                    Database.UpdateFileListAndGetChangeSet()
                );
                yield return changeSet.Start();

                Transaction transaction = Database.Connection.CreateTransaction();
                yield return transaction;
                int numChanges = 0;
                int numDeletes = 0;

                while (!changeSet.Disposed) {
                    var change = changeSet.Current;
                    if (change.Deleted) {
                        yield return Database.DeleteSourceFile(change.Filename);
                        numDeletes += 1;
                    } else {
                        yield return Database.GetSourceFileID(change.Filename);
                        sourceFiles.Enqueue(change.Filename);
                        numChanges += 1;
                    }

                    yield return changeSet.MoveNext();
                }

                yield return transaction.Commit();

                System.Diagnostics.Debug.WriteLine(String.Format("Disk scan complete. {0} change(s), {1} delete(s).", numChanges, numDeletes));
            }
        }

        private static void OnNextFileHandler (string filename, long lastWriteTime) {
            if (TagGroups.ContainsKey(filename))
                TagGroups[filename].Timestamp = lastWriteTime;
            else
                TagGroups.Add(filename, new TagGroup(filename, lastWriteTime));
        }

        public static IEnumerator<object> UpdateIndex (BlockingQueue<string> sourceFiles) {
            string langmap;
            {
                var buffer = new StringBuilder();
                var iter = new TaskIterator<TagDatabase.Filter>(Database.GetFilters());
                yield return iter.Start();
                while (!iter.Disposed) {
                    var filter = iter.Current;

                    if (buffer.Length > 0)
                        buffer.Append(",");
                    buffer.Append(filter.Language);
                    buffer.Append(":+");
                    buffer.Append(filter.Pattern.Replace("*", "").Replace("?", ""));

                    yield return iter.MoveNext();
                }
                langmap = buffer.ToString();
            }

            string lastSourceFile = null;
            TagGroup[] currentTagGroup = new TagGroup[] { null };

            var gen = new TagGenerator(
                GetCTagsPath(),
                "--filter=yes --fields=+afmikKlnsStz --sort=no --langmap=" + langmap
            );

            var onNextFile = (Func<string, object>)(
                (fn) => {
                    var lastWriteTime = System.IO.File.GetLastWriteTime(fn).ToFileTime();
                    OnNextFileHandler(fn, lastWriteTime);
                    return null;
                }
            );

            var inputLines = new BlockingQueue<string>();
            Scheduler.Start(
                gen.GenerateTags(sourceFiles, inputLines, onNextFile),
                TaskExecutionPolicy.RunAsBackgroundTask
            );

            var outputTags = new BlockingQueue<Tag>();
            Scheduler.Start(
                TagReader.ReadTags(inputLines, outputTags),
                TaskExecutionPolicy.RunAsBackgroundTask
            );

            Func<bool> shouldFlush = () => {
                return (outputTags.Count <= 0) && (inputLines.Count <= 0) && (sourceFiles.Count <= 0);
            };

            Func<TagGroup, bool> shouldCommit = (tg) => {
                return (tg != currentTagGroup[0]);
            };

            Scheduler.Start(
                FlushPendingTagGroups(shouldFlush, shouldCommit),
                TaskExecutionPolicy.RunAsBackgroundTask
            );

            while (true) {
                var f = outputTags.Dequeue();
                yield return f;

                using (new ActiveWorker("Updating index")) {
                    var tag = (Tag)f.Result;
                    if (tag.SourceFile != lastSourceFile) {
                        if (currentTagGroup[0] != null) {
                            TagGroups.Remove(currentTagGroup[0].Filename);
                            yield return currentTagGroup[0].Commit();
                        }

                        currentTagGroup[0] = TagGroups[tag.SourceFile];
                        lastSourceFile = tag.SourceFile;
                    }

                    currentTagGroup[0].Add(tag);
                }

                if ((outputTags.Count <= 0) && (inputLines.Count <= 0) && (sourceFiles.Count <= 0)) {
                    if (currentTagGroup[0] != null)
                        yield return currentTagGroup[0].Commit();
                }
            }
        }

        public static IEnumerator<object> FlushPendingTagGroups (Func<bool> shouldFlush, Func<TagGroup, bool> shouldCommit) {
            while (true) {
                if (shouldFlush()) {
                    foreach (string key in TagGroups.Keys.ToArray()) {
                        TagGroup item = null;
                        if (TagGroups.TryGetValue(key, out item)) {
                            if (shouldCommit(item)) {
                                TagGroups.Remove(key);
                                yield return item.Commit();
                            }
                        }
                    }
                } else {
                }

                yield return new Sleep(15.0);
            }
        }

        public static IEnumerator<object> DeleteSourceFiles (string[] filenames) {
            using (var transaction = Database.Connection.CreateTransaction()) {
                yield return transaction;
                foreach (string filename in filenames) {
                    yield return Database.DeleteSourceFileOrFolder(filename);
                }
                yield return transaction.Commit();
            }
        }

        public static IEnumerator<object> PeriodicGC () {
            while (true) {
                long preUsage = System.GC.GetTotalMemory(false);
                System.GC.Collect();
                long postUsage = System.GC.GetTotalMemory(false);

                System.Diagnostics.Debug.WriteLine(String.Format("Periodic GC complete. Usage {0} -> {1}.", preUsage, postUsage));

                yield return new Sleep(60.0 * 5);
            }
        }

        public static IEnumerator<object> MonitorForChanges (BlockingQueue<string> sourceFiles) {
            Scheduler.Start(
                PeriodicGC(),
                TaskExecutionPolicy.RunAsBackgroundTask
            );

            var rtc = new RunToCompletion(Database.GetFilterPatterns());
            yield return rtc;
            var filters = (string[])rtc.Result;

            rtc = new RunToCompletion(Database.GetFolderPaths());
            yield return rtc;
            var folders = (string[])rtc.Result;

            DiskMonitor monitor = new DiskMonitor(
                folders,
                filters,
                new string[] {
                    System.Text.RegularExpressions.Regex.Escape(@"\.svn\")
                }
            );
            monitor.Monitoring = true;

            var changedFiles = new List<string>();
            var deletedFiles = new List<string>();
            long lastDiskChange = 0;
            long updateInterval = TimeSpan.FromSeconds(30).Ticks;

            while (true) {
                long now = DateTime.Now.Ticks;
                if ((changedFiles.Count > 0) && ((now - lastDiskChange) > updateInterval)) {
                    foreach (string filename in changedFiles)
                        sourceFiles.Enqueue(filename);
                    changedFiles.Clear();
                }
                if ((deletedFiles.Count > 0) && ((now - lastDiskChange) > updateInterval)) {
                    using (new ActiveWorker(String.Format("Pruning {0} item(s) from index", deletedFiles.Count))) {
                        string[] filenames = deletedFiles.ToArray();
                        deletedFiles.Clear();
                        yield return DeleteSourceFiles(filenames);
                    }
                }

                using (new ActiveWorker(String.Format("Reading disk change history"))) {
                    now = DateTime.Now.Ticks;
                    foreach (string filename in monitor.GetChangedFiles().Distinct()) {
                        lastDiskChange = now;
                        changedFiles.Add(filename);
                    }
                    foreach (string filename in monitor.GetDeletedFiles().Distinct()) {
                        lastDiskChange = now;
                        deletedFiles.Add(filename);
                    }
                }

                yield return new Sleep(1.0);
            }
        }
    }
}
