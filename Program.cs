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

using ITask = System.Collections.Generic.IEnumerable<object>;
using System.Threading;
using System.Runtime.InteropServices;

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

    public static class Program {
        public static TaskScheduler Scheduler;
        public static List<ActiveWorker> ActiveWorkers = new List<ActiveWorker>();
        public static NotifyIcon NotifyIcon;
        public static Icon Icon_Monitoring;
        public static Icon Icon_Working_1, Icon_Working_2;
        public static ContextMenuStrip ContextMenu;
        public static SQLiteTransaction Transaction = null;
        public static string TrayCaption;
        public static string DatabasePath;
        public static List<ConnectionWrapper> ActiveConnections = new List<ConnectionWrapper>();

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

            using (var db = new TagDatabase(Scheduler, DatabasePath)) {
                Icon_Monitoring = Icon.FromHandle(Properties.Resources.database_monitoring.GetHicon());
                Icon_Working_1 = Icon.FromHandle(Properties.Resources.database_working_1.GetHicon());
                Icon_Working_2 = Icon.FromHandle(Properties.Resources.database_working_2.GetHicon());

                ContextMenu = new ContextMenuStrip();
                ContextMenu.Items.Add(
                    "&Search", null,
                    (e, s) => {
                        using (var dialog = new SearchDialog(db))
                            dialog.ShowDialog();
                    }
                );
                ContextMenu.Items.Add("-");
                ContextMenu.Items.Add(
                    "E&xit", null,
                    (e, s) => {
                        Scheduler.Start(ExitTask(db));
                    }
                );

                NotifyIcon = new NotifyIcon();
                NotifyIcon.ContextMenuStrip = ContextMenu;

                Scheduler.Start(
                    RefreshTrayIcon(),
                    TaskExecutionPolicy.RunAsBackgroundTask
                );

                Scheduler.Start(
                    MainTask(db, argv),
                    TaskExecutionPolicy.RunAsBackgroundTask
                );

                Application.Run();

                if (Transaction != null) {
                    Transaction.Commit();
                    Transaction.Dispose();
                }
            }
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

        public static void ShowSearch (TagDatabase db) {
            var dlg = new SearchDialog(db);
            dlg.ShowDialog();
            dlg.Dispose();
        }

        public static void ShowConfiguration (TagDatabase db) {
            var dlg = new ConfigurationDialog(db);
            dlg.ShowDialog();
            dlg.Dispose();
        }

        public static IEnumerator<object> ExitTask (TagDatabase db) {
            if (Transaction != null)
                yield return db.Connection.CommitTransaction();

            db.Dispose();

            NotifyIcon.Visible = false;
            Application.Exit();

            yield break;
        }

        public static IEnumerator<object> AutoShowConfiguration (TagDatabase db, string[] argv) {
            bool show = false;
            Future f;

            if (argv.Contains("--configure")) {
                show = true;
            } else {
                {
                    var iter = new TaskIterator<TagDatabase.Folder>(db.GetFolders());
                    yield return iter.Start();
                    f = iter.ToArray();
                }

                yield return f;

                if (((TagDatabase.Folder[])f.Result).Length == 0) {
                    show = true;
                } else {
                    {
                        var iter = new TaskIterator<TagDatabase.Filter>(db.GetFilters());
                        yield return iter.Start();
                        f = iter.ToArray();
                    }

                    yield return f;

                    if (((TagDatabase.Filter[])f.Result).Length == 0)
                        show = true;
                }
            }

            if (show)
                ShowConfiguration(db);
        }

        public static IEnumerator<object> MainTask (TagDatabase db, string[] argv) {
            yield return AutoShowConfiguration(db, argv);

            using (new ActiveWorker("Compacting index database")) {
                yield return db.Compact();
            }

            var sourceFiles = new BlockingQueue<string>();

            if (argv.Contains("--noscan"))
                Scheduler.Start(
                    MonitorForChanges(db, sourceFiles),
                    TaskExecutionPolicy.RunAsBackgroundTask
                );
            else
                Scheduler.Start(
                    ScanThenMonitor(db, sourceFiles),
                    TaskExecutionPolicy.RunAsBackgroundTask
                );

            Scheduler.Start(
                UpdateIndex(db, sourceFiles),
                TaskExecutionPolicy.RunAsBackgroundTask
            );
        }

        public static IEnumerator<object> ScanThenMonitor (TagDatabase db, BlockingQueue<string> sourceFiles) {
            yield return ScanFiles(db, sourceFiles);

            yield return MonitorForChanges(db, sourceFiles);
        }

        public static IEnumerator<object> RefreshTrayIcon () {
            bool toggle = false;

            while (true) {
                toggle = !toggle;

                try {

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
                } catch (Exception ex) {
                    Console.WriteLine("Error in RefreshTrayIcon: {0}", ex.ToString());
                    throw;
                }

                yield return new Sleep(0.5);
            }
        }

        public static IEnumerator<object> ScanFiles (TagDatabase db, BlockingQueue<string> sourceFiles) {
            using (new ActiveWorker("Scanning hard disk for changes")) {
                var changeSet = new TaskIterator<TagDatabase.Change>(
                    db.UpdateFileListAndGetChangeSet()
                );
                yield return changeSet.Start();

                int numChanges = 0;
                int numDeletes = 0;
                while (!changeSet.Disposed) {
                    var change = changeSet.Current;
                    if (change.Deleted) {
                        yield return db.DeleteSourceFile(change.Filename);
                        numDeletes += 1;
                    } else {
                        yield return db.GetSourceFileID(change.Filename);
                        sourceFiles.Enqueue(change.Filename);
                        numChanges += 1;
                    }

                    yield return changeSet.MoveNext();
                }

                Console.WriteLine("Disk scan complete. {0} change(s), {1} delete(s).", numChanges, numDeletes);
            }
        }

        private static IEnumerator<object> OnNextFileHandler (TagDatabase db, string filename, long lastWriteTime) {
            yield return db.DeleteTagsForFile(filename);
            yield return db.MakeSourceFileID(filename, lastWriteTime);
        }

        public static IEnumerator<object> UpdateIndex (TagDatabase db, BlockingQueue<string> sourceFiles) {
            var gen = new TagGenerator(
                GetCTagsPath(),
                "--filter=yes --filter-terminator=[[<>]]\n --fields=+afmikKlnsStz --sort=no"
            );

            var onNextFile = (Func<string, object>)(
                (fn) => {
                    var lastWriteTime = System.IO.File.GetLastWriteTime(fn).ToFileTime();
                    return Scheduler.Start(
                        OnNextFileHandler(db, fn, lastWriteTime),
                        TaskExecutionPolicy.RunAsBackgroundTask
                    );
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

            string lastSourceFile = null;

            while (true) {
                var f = outputTags.Dequeue();
                yield return f;

                using (new ActiveWorker("Adding tags to index database")) {
                    var tag = (Tag)f.Result;
                    if (tag.SourceFile != lastSourceFile) {
                        lastSourceFile = tag.SourceFile;
                    }

                    yield return db.AddTag(tag);

                    if ((outputTags.Count == 0) && (inputLines.Count == 0) && (sourceFiles.Count == 0)) {
                        Console.WriteLine("Committing transaction because work queues are empty.");
                        yield return db.Connection.CommitTransaction();
                        yield return db.Connection.BeginTransaction();
                    }
                }
            }
        }

        public static IEnumerator<object> DeleteSourceFiles (TagDatabase db, string[] filenames) {
            foreach (string filename in filenames) {
                yield return db.DeleteSourceFileOrFolder(filename);
            }
        }

        public static IEnumerator<object> MonitorForChanges (TagDatabase db, BlockingQueue<string> sourceFiles) {
            var rtc = new RunToCompletion(db.GetFilterPatterns());
            yield return rtc;
            var filters = (string[])rtc.Result;

            rtc = new RunToCompletion(db.GetFolderPaths());
            yield return rtc;
            var folders = (string[])rtc.Result;

            DiskMonitor monitor = new DiskMonitor(
                folders,
                filters
            );
            monitor.Monitoring = true;

            var changedFiles = new List<string>();
            var deletedFiles = new List<string>();
            long lastDiskChange = 0;
            long updateInterval = TimeSpan.FromSeconds(15).Ticks;

            while (true) {
                long now = DateTime.Now.Ticks;
                if ((changedFiles.Count > 0) && ((now - lastDiskChange) > updateInterval)) {
                    using (new ActiveWorker(String.Format("Updating index of {0} changed file(s)", changedFiles.Count))) {
                        foreach (string filename in changedFiles)
                            sourceFiles.Enqueue(filename);
                        changedFiles.Clear();
                    }
                }
                if ((deletedFiles.Count > 0) && ((now - lastDiskChange) > updateInterval)) {
                    using (new ActiveWorker(String.Format("Pruning {0} deleted file(s)/folder(s) from index", deletedFiles.Count))) {
                        string[] filenames = deletedFiles.ToArray();
                        deletedFiles.Clear();
                        yield return DeleteSourceFiles(db, filenames);
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
