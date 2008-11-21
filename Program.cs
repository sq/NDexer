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
            var textReader = Future.RunInThread((Func<string>)(() => {
                return System.IO.File.ReadAllText(Filename);
            }));

            var transaction = Program.Database.Connection.CreateTransaction();
            yield return transaction;

            yield return Program.Database.DeleteTagsForFile(Filename);

            yield return Program.Database.MakeSourceFileID(Filename, Timestamp);

            foreach (var tag in this)
                yield return Program.Database.AddTag(tag);

            yield return textReader;
            string content = textReader.Result as string;

            yield return Program.Database.SetFullTextContentForFile(Filename, content);

            yield return transaction.Commit();
        }

        public override string ToString () {
            return String.Format("TagGroup(fn={0}, ts={1}) {2} item(s)", Filename, Timestamp, Count);
        }
    }

    public static class Program {
        public const string RevisionString = "$Rev$";
        public static int Revision;

        public static TaskScheduler Scheduler;
        public static TagDatabase Database;
        public static List<ActiveWorker> ActiveWorkers = new List<ActiveWorker>();
        public static NotifyIcon NotifyIcon;
        public static Icon Icon_Monitoring;
        public static Icon Icon_Working_1, Icon_Working_2;
        public static ContextMenuStrip ContextMenu;
        public static string TrayCaption;
        public static string DatabasePath;
        public static string LanguageMap;

        private const int BatchSize = 128;

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

            Revision = int.Parse(System.Text.RegularExpressions.Regex.Match(RevisionString, @"\$Rev\: ([0-9]*)").Groups[1].Value);

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
                "Search &Tags", null,
                (e, s) => {
                    Scheduler.Start(ShowSearchTask(), TaskExecutionPolicy.RunAsBackgroundTask);
                }
            );
            ContextMenu.Items.Add(
                "Search &Files", null,
                (e, s) => {
                    Scheduler.Start(ShowFullTextSearchTask(), TaskExecutionPolicy.RunAsBackgroundTask);
                }
            );
            ContextMenu.Items.Add("-");
            ContextMenu.Items.Add(
                "&Configure", null,
                (e, s) => {
                    using (var dialog = new ConfigurationDialog(Database))
                        if ((dialog.ShowDialog() == DialogResult.OK) && (dialog.NeedRestart))
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
                yield return Database.Connection.ExecuteSQL("DELETE FROM Tags; DELETE FROM SourceFiles; DELETE FROM TagContexts; DELETE FROM TagKinds; DELETE FROM TagLanguages; DELETE FROM FullText");
                yield return trans.Commit();
                yield return RestartTask();
            }
        }

        public static IEnumerable<string> GetProcessOutput (string filename, string arguments) {
            var info = new ProcessStartInfo(filename, arguments);
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.ErrorDialog = false;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;

            using (var process = Process.Start(info)) {
                process.WaitForExit();

                while (!process.StandardOutput.EndOfStream)
                    yield return process.StandardOutput.ReadLine();
            }
        }

        public static string[] GetLanguageNames () {
            return GetProcessOutput(
                GetCTagsPath(), 
                "--list-languages"
            ).ToArray();
        }

        public static Dictionary<string, string> GetLanguageMaps () {
            var result = new Dictionary<string, string>();

            foreach (string line in GetProcessOutput(
                GetCTagsPath(),
                "--list-maps"
            )) {
                string[] parts = line.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                    result.Add(parts[0], parts[1]);
            }

            return result;
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
            var dialog = new SearchDialog();
            dialog.Show();

            var rtc = new RunToCompletion(Database.OpenReadConnection());
            yield return rtc;
            var conn = (ConnectionWrapper)rtc.Result;
            dialog.SetConnection(conn);
        }

        public static IEnumerator<object> ShowFullTextSearchTask () {
            var dialog = new FindInFilesDialog();
            dialog.Show();

            var rtc = new RunToCompletion(Database.OpenReadConnection());
            yield return rtc;
            var conn = (ConnectionWrapper)rtc.Result;
            dialog.SetConnection(conn);
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

            if (show) {
                using (var dialog = new ConfigurationDialog(Database))
                    if (dialog.ShowDialog() != DialogResult.OK)
                        yield return ExitTask();
            }
        }

        public static IEnumerator<object> MainTask (string[] argv) {
            yield return Database.Initialize();

            yield return AutoShowConfiguration(argv);

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
                LanguageMap = buffer.ToString();
            }

            using (new ActiveWorker("Compacting index")) {
                yield return Database.Compact();
            }

            Scheduler.Start(
                MonitorForChanges(),
                TaskExecutionPolicy.RunAsBackgroundTask
            );

            if (!argv.Contains("--noscan"))
                Scheduler.Start(
                    ScanFiles(),
                    TaskExecutionPolicy.RunAsBackgroundTask
                );
        }

        public static IEnumerator<object> RefreshTrayIcon () {
            bool toggle = false;

            while (true) {
                toggle = !toggle;

                Icon theIcon;
                string statusMessage = "";
                int numWorkers = 0;
                lock (ActiveWorkers)
                    numWorkers = ActiveWorkers.Count;
                if (numWorkers > 0) {
                    theIcon = (toggle) ? Icon_Working_1 : Icon_Working_2;
                    lock (ActiveWorkers)
                        statusMessage = ": " + ActiveWorkers[0].Description;
                } else {
                    theIcon = Icon_Monitoring;
                }

                TrayCaption = String.Format("NDexer r{2} ({0}){1}", System.IO.Path.GetFileName(DatabasePath), statusMessage, Revision);
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

        public static IEnumerator<object> CommitBatches (BlockingQueue<IEnumerable<string>> batches, Future completion) {
            while (batches.Count > 0 || !completion.Completed) {
                var f = batches.Dequeue();                
                yield return f;

                var batch = f.Result as IEnumerable<string>;
                if (batch != null)
                    yield return UpdateIndex(batch);
            }
        }

        public static IEnumerator<object> ScanFiles () {
            var time_start = DateTime.UtcNow.Ticks;

            var completion = new Future();
            var batchQueue = new BlockingQueue<IEnumerable<string>>();
            var changedFiles = new List<string>();
            var deletedFiles = new List<string>();

            for (int i = 0; i < System.Environment.ProcessorCount; i++)
                Scheduler.Start(
                    CommitBatches(batchQueue, completion),
                    TaskExecutionPolicy.RunAsBackgroundTask
                );

            using (new ActiveWorker("Scanning folders for changes")) {
                var changeSet = new BlockingQueue<TagDatabase.Change>();
                
                var changeGenerator = Scheduler.Start(
                     Database.UpdateFileListAndGetChangeSet(changeSet),
                     TaskExecutionPolicy.RunAsBackgroundTask
                );
                changeGenerator.RegisterOnComplete((f, r, e) => {
                    changeSet.Enqueue(
                        new TagDatabase.Change()
                    );
                });
                
                int numChanges = 0;
                int numDeletes = 0;

                while (!changeGenerator.Completed || (changeSet.Count > 0)) {
                    var f = changeSet.Dequeue();
                    yield return f;
                    var change = (TagDatabase.Change)f.Result;

                    if (change.Filename == null)
                        continue;

                    if (change.Deleted) {
                        deletedFiles.Add(change.Filename);
                        numDeletes += 1;
                    } else {
                        yield return Database.GetSourceFileID(change.Filename);
                        changedFiles.Add(change.Filename);
                        numChanges += 1;
                    }

                    if (deletedFiles.Count >= BatchSize) {
                        var transaction = Database.Connection.CreateTransaction();
                        yield return transaction;

                        foreach (string filename in deletedFiles)
                            yield return Database.DeleteSourceFile(filename);

                        deletedFiles.Clear();
                        yield return transaction.Commit();
                    }

                    if (changedFiles.Count >= BatchSize) {
                        string[] batch = changedFiles.ToArray();
                        changedFiles.Clear();

                        batchQueue.Enqueue(batch);
                    }
                }

                if (deletedFiles.Count > 0) {
                    var transaction = Database.Connection.CreateTransaction();
                    yield return transaction;

                    foreach (string filename in deletedFiles)
                        yield return Database.DeleteSourceFile(filename);

                    deletedFiles.Clear();
                    yield return transaction.Commit();
                }

                if (changedFiles.Count > 0) {
                    string[] batch = changedFiles.ToArray();
                    batchQueue.Enqueue(batch);
                }

                completion.Complete();

                while (batchQueue.Count < 0)
                    batchQueue.Enqueue(null);

                var time_end = DateTime.UtcNow.Ticks;
                var elapsed = TimeSpan.FromTicks(time_end - time_start).TotalSeconds;

                System.Diagnostics.Debug.WriteLine(String.Format("Disk scan complete after {2:00000.00} seconds. {0} change(s), {1} delete(s).", numChanges, numDeletes, elapsed));
            }
        }

        public static IEnumerator<object> UpdateIndex (IEnumerable<string> filenames) {
            var gen = new TagGenerator(
                GetCTagsPath(),
                LanguageMap
            );

            var tagIterator = new TaskIterator<TagGroup>(
                gen.GenerateTags(filenames)
            );

            using (new ActiveWorker("Updating index")) {
                yield return tagIterator.Start();

                while (!tagIterator.Disposed) {
                    var group = tagIterator.Current;
                    yield return group.Commit();

                    yield return tagIterator.MoveNext();
                }
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

        public static IEnumerator<object> MonitorForChanges () {
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
            long updateInterval = TimeSpan.FromSeconds(10).Ticks;

            while (true) {
                long now = DateTime.Now.Ticks;
                if ((changedFiles.Count > 0) && ((now - lastDiskChange) > updateInterval)) {
                    var filenames = changedFiles.ToArray();
                    changedFiles.Clear();

                    yield return UpdateIndex(filenames);
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

                yield return new Sleep(2.5);
            }
        }
    }
}
