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
using MovablePython;
using System.Text.RegularExpressions;

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

    public class FileIndexEntry {
        public string Filename;
        public long Timestamp;

        public FileIndexEntry (string filename, long timestamp)
            : base() {
            Filename = filename;
            Timestamp = timestamp;
        }

        public IEnumerator<object> Commit () {
            var textReader = Future.RunInThread((Func<string>)(() => {
                return System.IO.File.ReadAllText(Filename);
            }));

            using (var transaction = Program.Database.Connection.CreateTransaction()) {
                yield return transaction;

                yield return Program.Database.MakeSourceFileID(Filename, Timestamp);

                yield return textReader;
                string content = "";
                try {
                    content = textReader.Result as string;
                } catch {
                }

                yield return Program.Database.SetFullTextContentForFile(Filename, content);

                yield return transaction.Commit();
            }
        }

        public override string ToString () {
            return String.Format("FileIndexEntry(fn={0}, ts={1})", Filename, Timestamp);
        }
    }

    public static partial class Program {
        public static TaskScheduler Scheduler;
        public static TagDatabase Database;
        public static List<ActiveWorker> ActiveWorkers = new List<ActiveWorker>();
        public static NotifyIcon NotifyIcon;
        public static Icon Icon_Monitoring;
        public static Icon Icon_Working_1, Icon_Working_2;
        public static Hotkey Hotkey_Search_Files;
        public static NativeWindow HotkeyWindow;
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
                using (var dlg = new SaveFileDialog()) {
                    dlg.Title = "Select Index Database";
                    dlg.Filter = "Index Databases (*.db)|*.db";
                    dlg.CheckFileExists = false;
                    dlg.CheckPathExists = true;
                    dlg.AddExtension = true;
                    dlg.AutoUpgradeEnabled = true;
                    dlg.OverwritePrompt = false;

                    if (dlg.ShowDialog() != DialogResult.OK) {
                        MessageBox.Show(
                            "NDexer cannot start without a path specified for the index database on the command line.\n" +
                            @"For example: ndexer.exe C:\mysource\index.db",
                            "NDexer Error"
                        );
                        return;
                    } else {
                        DatabasePath = dlg.FileName;
                    }
                }
            } else {
                DatabasePath = System.IO.Path.GetFullPath(argv[0]);
            }

            if (!System.IO.File.Exists(DatabasePath)) {
                System.IO.File.Copy(GetDataPath() + @"\ndexer.db", DatabasePath);
            } else {
                if (System.IO.File.Exists(DatabasePath + "_new")) {
                    System.IO.File.Move(DatabasePath, DatabasePath + "_old");
                    System.IO.File.Move(DatabasePath + "_new", DatabasePath);
                    System.IO.File.Delete(DatabasePath + "_old");
                }
            }

            Scheduler = new TaskScheduler(JobQueue.WindowsMessageBased);

            Database = new TagDatabase(Scheduler, DatabasePath);

            HotkeyWindow = new NativeWindow();
            HotkeyWindow.CreateHandle(new CreateParams {
                Caption = "NDexer Hotkey Window",
                X = 0,
                Y = 0,
                Width = 0,
                Height = 0,
                Style = 0,
                ExStyle = 0x08000000,
                Parent = new IntPtr(-3)
            });

            Icon_Monitoring = Icon.FromHandle(Properties.Resources.database_monitoring.GetHicon());
            Icon_Working_1 = Icon.FromHandle(Properties.Resources.database_working_1.GetHicon());
            Icon_Working_2 = Icon.FromHandle(Properties.Resources.database_working_2.GetHicon());

            ContextMenu = new ContextMenuStrip();
            ContextMenu.Items.Add(
                "&Search", null,
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
                Scheduler.Start(ShowFullTextSearchTask(), TaskExecutionPolicy.RunAsBackgroundTask);
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

        public static string GetDataPath () {
            return GetExecutablePath() + @"\data\";
        }

        public static IEnumerator<object> ConfirmRebuildIndexTask () {
            if (MessageBox.Show(
                        "Are you sure you want to rebuild the index? This will take a while!", "Rebuild Index",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    ) == DialogResult.Yes) {

                yield return RebuildIndexTask();
            }
        }

        public static IEnumerator<object> GetDBSchemaVersion (ConnectionWrapper cw) {
            using (var q = cw.BuildQuery("PRAGMA user_version")) {
                var f = q.ExecuteScalar();
                yield return f;
                yield return new Result(f.Result);
            }
        }

        public static string GetEmbeddedSchema () {
            string schemaText;

            using (var stream = Assembly.GetEntryAssembly().GetManifestResourceStream("Ndexer.schema.sql"))
            using (var reader = new StreamReader(stream))
                schemaText = reader.ReadToEnd();

            return schemaText;
        }

        public static long GetEmbeddedSchemaVersion () {
            var schema = GetEmbeddedSchema();
            return long.Parse(Regex.Match(schema, "PRAGMA user_version=(?'version'[0-9]*);", RegexOptions.ExplicitCapture).Groups["version"].Value);
        }

        public static IEnumerator<object> RebuildIndexTask () {
            using (new ActiveWorker("Rebuilding index...")) {
                var conn = new SQLiteConnection(String.Format("Data Source={0}", DatabasePath + "_new"));
                conn.Open();
                var cw = new ConnectionWrapper(Scheduler, conn);

                yield return cw.ExecuteSQL("PRAGMA auto_vacuum=none");

                long schemaVersion = GetEmbeddedSchemaVersion();

                cw.ExecuteSQL(GetEmbeddedSchema());

                var trans = cw.CreateTransaction();
                yield return trans;

                using (var iter = new TaskEnumerator<TagDatabase.Folder>(Database.GetFolders()))
                while (!iter.Disposed) {
                    yield return iter.Fetch();

                    foreach (var item in iter)
                        yield return cw.ExecuteSQL(
                            "INSERT INTO Folders (Folders_Path) VALUES (?)",
                            item.Path
                        );
                }

                using (var iter = new TaskEnumerator<TagDatabase.Filter>(Database.GetFilters()))
                while (!iter.Disposed) {
                    yield return iter.Fetch();

                    foreach (var item in iter)
                        yield return cw.ExecuteSQL(
                            "INSERT INTO Filters (Filters_Pattern) VALUES (?)",
                            item.Pattern
                        );
                }

                using (var iter = Database.Connection.BuildQuery(
                    "SELECT Preferences_Name, Preferences_Value FROM Preferences"
                ).Execute())
                while (!iter.Disposed) {
                    yield return iter.Fetch();

                    foreach (var item in iter)
                        yield return cw.ExecuteSQL(
                            "INSERT INTO Preferences (Preferences_Name, Preferences_Value) VALUES (?, ?)",
                            item.GetValue(0), item.GetValue(1)
                        );
                }

                yield return trans.Commit();

                yield return Database.Connection.Dispose();

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

        public static bool TryLocateEditorExecutable (string editorName, ref string result) {
            var directorType = Type.GetType(String.Format("Ndexer.{0}Director", editorName), false, true);
            if (directorType == null)
                return false;

            var method = directorType.GetMethod("LocateExecutable", BindingFlags.Static | BindingFlags.Public);
            if (method == null)
                return false;

            var handler = (LocateExecutableHandler)Delegate.CreateDelegate(typeof(LocateExecutableHandler), method);

            return handler(ref result);
        }

        public static IBasicDirector GetDirector () {
            var editorName = (string)Scheduler.WaitFor(Database.GetPreference("TextEditor.Name"));
            var editorPath = (string)Scheduler.WaitFor(Database.GetPreference("TextEditor.Location"));
            var directorType = Type.GetType(String.Format("Ndexer.{0}Director", editorName), true, true);
            var constructor = directorType.GetConstructor(new Type[] { typeof(string) });
            var director = (IBasicDirector)constructor.Invoke(new object[] { editorPath });
            return director;
        }

        public static IEnumerator<object> ShowFullTextSearchTask () {
            var dialog = new FindInFilesDialog();
            dialog.Show();

            Future<ConnectionWrapper> f;
            yield return Database.OpenReadConnection().Run(out f);
            dialog.SetConnection(f.Result);
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
            IFuture f;

            if (argv.Contains("--configure")) {
                show = true;
            } else {
                {
                    var iter = new TaskEnumerator<TagDatabase.Folder>(Database.GetFolders());
                    f = Scheduler.Start(iter.GetArray());
                }

                yield return f;

                if (((TagDatabase.Folder[])f.Result).Length == 0) {
                    show = true;
                } else {
                    {
                        var iter = new TaskEnumerator<TagDatabase.Filter>(Database.GetFilters());
                        f = Scheduler.Start(iter.GetArray());
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

            var schemaVersion = GetEmbeddedSchemaVersion();

            Future f;
            yield return GetDBSchemaVersion(Database.Connection).Run(out f);
            if (schemaVersion.CompareTo(f.Result) != 0) {
                yield return RebuildIndexTask();
                yield break;
            }

            yield return AutoShowConfiguration(argv);

            yield return OnConfigurationChanged();

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

                var dbName = Path.GetDirectoryName(DatabasePath);
                dbName = Path.Combine(dbName.Substring(dbName.LastIndexOf('\\') + 1), Path.GetFileNameWithoutExtension(DatabasePath));

                TrayCaption = String.Format("NDexer r{2} ({0}){1}", dbName, statusMessage, Revision);
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

        public static IEnumerator<object> OnConfigurationChanged () {
            if (Hotkey_Search_Files != null) {
                if (Hotkey_Search_Files.Registered)
                    Hotkey_Search_Files.Unregister();
            }

            Keys keyCode, modifiers;
            Future<string> f;

            yield return Database.GetPreference("Hotkeys.SearchFiles.Key").Run(out f);
            keyCode = (Keys)Enum.Parse(typeof(Keys), f.Result ?? "None", true);

            yield return Database.GetPreference("Hotkeys.SearchFiles.Modifiers").Run(out f);
            modifiers = (Keys)Enum.Parse(typeof(Keys), f.Result ?? "None", true);

            Hotkey_Search_Files = new Hotkey(keyCode, modifiers);
            if (!Hotkey_Search_Files.Empty) {
                Hotkey_Search_Files.Pressed += (s, e) =>
                {
                    Scheduler.Start(ShowFullTextSearchTask(), TaskExecutionPolicy.RunAsBackgroundTask);
                };
                Hotkey_Search_Files.Register(HotkeyWindow);
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
                changeGenerator.RegisterOnComplete((f) => changeSet.Enqueue(new TagDatabase.Change()));
                
                int numChanges = 0;
                int numDeletes = 0;

                while (!changeGenerator.Completed || (changeSet.Count > 0)) {
                    var f = changeSet.Dequeue();
                    yield return f;
                    var change = f.Result;

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
            long lastWriteTime = 0;

            using (new ActiveWorker("Updating index"))
            foreach (var filename in filenames) {
                yield return Future.RunInThread(
                    () => System.IO.File.GetLastWriteTimeUtc(filename).ToFileTimeUtc()
                ).Bind(() => lastWriteTime);

                yield return (new FileIndexEntry(filename, lastWriteTime).Commit());
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

            Future<string[]> f;

            yield return Database.GetFilterPatterns().Run(out f);
            var filters = f.Result;

            yield return Database.GetFolderPaths().Run(out f);
            var folders = f.Result;

            DiskMonitor monitor = new DiskMonitor(
                folders,
                filters,
                new string[] {
                    System.Text.RegularExpressions.Regex.Escape(@"\.svn\"),
                    System.Text.RegularExpressions.Regex.Escape(@"\.git\"),
                    System.Text.RegularExpressions.Regex.Escape(@"\.hg\")
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
