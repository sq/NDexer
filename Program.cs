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

namespace Ndexer {
    static class Program {
        public static TaskScheduler Scheduler;
        public static int NumWorkers = 0;
        public static NotifyIcon NotifyIcon;
        public static Icon Icon_Monitoring;
        public static Icon Icon_Working_1, Icon_Working_2;
        public static ContextMenuStrip ContextMenu;
        public static SQLiteTransaction Transaction = null;

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

            string databasePath = System.IO.Path.GetFullPath(argv[0]);

            if (!System.IO.File.Exists(databasePath))
                System.IO.File.Copy(GetDataPath() + @"\ndexer.db", databasePath);

            using (var db = new TagDatabase(databasePath)) {
                Scheduler = new TaskScheduler(JobQueue.WindowsMessageBased);

                Icon_Monitoring = Icon.FromHandle(Properties.Resources.database_monitoring.GetHicon());
                Icon_Working_1 = Icon.FromHandle(Properties.Resources.database_working_1.GetHicon());
                Icon_Working_2 = Icon.FromHandle(Properties.Resources.database_working_2.GetHicon());

                ContextMenu = new ContextMenuStrip();
                ContextMenu.Items.Add(
                    "&Search", null,
                    (e, s) => {
                        var dialog = new SearchDialog(db);
                        dialog.ShowDialog();
                        dialog.Dispose();
                    }
                );
                ContextMenu.Items.Add("-");
                ContextMenu.Items.Add(
                    "E&xit", null,
                    (e, s) => {
                        NotifyIcon.Visible = false;
                        Application.Exit();
                    }
                );

                NotifyIcon = new NotifyIcon();
                NotifyIcon.ContextMenuStrip = ContextMenu;
                NotifyIcon.Icon = Icon_Monitoring;
                NotifyIcon.Text = String.Format("NDexer ({0})", databasePath);
                NotifyIcon.Visible = true;

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

        public static string GetDataPath () {
            string executablePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath)
                .ToLower().Replace(@"\bin\debug", "").Replace(@"\bin\release", "") + @"\data\";
            return executablePath;
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

        public static void CompactDatabase (TagDatabase db) {
            Interlocked.Increment(ref NumWorkers);
            db.Compact();
            Interlocked.Decrement(ref NumWorkers);
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

            yield return Future.RunInThread(
                (Action<TagDatabase>)CompactDatabase, db
            );

            Transaction = db.Connection.BeginTransaction();

            var sourceFiles = new BlockingQueue<string>();

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

            Transaction.Commit();
            Transaction.Dispose();
            Transaction = db.Connection.BeginTransaction();

            yield return MonitorForChanges(db, sourceFiles);
        }

        public static IEnumerator<object> RefreshTrayIcon () {
            bool toggle = false;

            while (true) {
                toggle = !toggle;

                Icon theIcon;
                if (NumWorkers > 0) {
                    theIcon = (toggle) ? Icon_Working_1 : Icon_Working_2;
                } else {
                    theIcon = Icon_Monitoring;
                }

                if (NotifyIcon.Icon != theIcon)
                    NotifyIcon.Icon = theIcon;

                yield return new Sleep(0.5);
            }
        }

        public static IEnumerator<object> ScanFiles (TagDatabase db, BlockingQueue<string> sourceFiles) {
            var changeSet = new TaskIterator<TagDatabase.Change>(
                db.UpdateFileListAndGetChangeSet()
            );
            yield return changeSet.Start();

            int numChanges = 0;
            int numDeletes = 0;
            while (!changeSet.Disposed) {
                Interlocked.Increment(ref NumWorkers);

                var change = changeSet.Current;
                if (change.Deleted) {
                    yield return db.DeleteSourceFile(change.Filename);
                    numDeletes += 1;
                } else {
                    yield return db.GetSourceFileID(change.Filename);
                    sourceFiles.Enqueue(change.Filename);
                    numChanges += 1;
                }

                Interlocked.Decrement(ref NumWorkers);

                yield return changeSet.MoveNext();
            }

            Console.WriteLine("Disk scan complete. {0} change(s), {1} delete(s).", numChanges, numDeletes);
        }

        private static IEnumerator<object> OnNextFileHandler (TagDatabase db, string filename, long lastWriteTime) {
            yield return db.DeleteTagsForFile(filename);
            yield return db.MakeSourceFileID(filename, lastWriteTime);
        }

        public static IEnumerator<object> UpdateIndex (TagDatabase db, BlockingQueue<string> sourceFiles) {
            var gen = new TagGenerator(
                @"C:\program files\ctags57\ctags.exe",
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

            Interlocked.Increment(ref NumWorkers);

            while (true) {
                Interlocked.Decrement(ref NumWorkers);

                var f = outputTags.Dequeue();
                yield return f;

                Interlocked.Increment(ref NumWorkers);

                var tag = (Tag)f.Result;
                if (tag.SourceFile != lastSourceFile) {
                    lastSourceFile = tag.SourceFile;
                }

                yield return db.AddTag(tag);
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
                Interlocked.Increment(ref NumWorkers);
                long now = DateTime.Now.Ticks;
                if ((changedFiles.Count > 0) && ((now - lastDiskChange) > updateInterval)) {
                    Console.WriteLine("Detected {0} changed file(s). Starting index update...", changedFiles.Count);
                    foreach (string filename in changedFiles)
                        sourceFiles.Enqueue(filename);
                    changedFiles.Clear();
                }
                if ((deletedFiles.Count > 0) && ((now - lastDiskChange) > updateInterval)) {
                    string[] filenames = deletedFiles.ToArray();
                    deletedFiles.Clear();
                    yield return DeleteSourceFiles(db, filenames);
                    Console.WriteLine("Pruned {0} deleted file(s)/folder(s) from index.", filenames.Length);
                }

                now = DateTime.Now.Ticks;
                foreach (string filename in monitor.GetChangedFiles().Distinct()) {
                    lastDiskChange = now;
                    changedFiles.Add(filename);
                }
                foreach (string filename in monitor.GetDeletedFiles().Distinct()) {
                    lastDiskChange = now;
                    deletedFiles.Add(filename);
                }
                Interlocked.Decrement(ref NumWorkers);

                yield return new Sleep(1.0);
            }
        }
    }
}
