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

                if (argv.Contains("--configure") || (db.GetFolders().Count() == 0) || (db.GetFilters().Count() == 0)) {
                    ShowConfiguration(db);
                }

                Scheduler.Start(
                    RefreshTrayIcon(),
                    TaskExecutionPolicy.RunAsBackgroundTask
                );

                Scheduler.Start(
                    MainTask(db),
                    TaskExecutionPolicy.RunAsBackgroundTask
                );

                Application.Run();
            }
        }

        public static string GetDataPath () {
            string executablePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath)
                .ToLower().Replace(@"\bin\debug", "").Replace(@"\bin\release", "") + @"\data\";
            return executablePath;
        }

        public static void ShowSearch (TagDatabase db) {
            Application.Run(new SearchDialog(db));
        }

        public static void ShowConfiguration (TagDatabase db) {
            Application.Run(new ConfigurationDialog(db));
        }

        public static void CompactDatabase (TagDatabase db) {
            Interlocked.Increment(ref NumWorkers);
            db.Compact();
            Interlocked.Decrement(ref NumWorkers);
        }

        public static IEnumerator<object> MainTask (TagDatabase db) {
            yield return Future.RunInThread(
                (Action<TagDatabase>)CompactDatabase, db
            );

            Scheduler.Start(
                TransactionPump(db),
                TaskExecutionPolicy.RunAsBackgroundTask
            );

            var sourceFiles = new BlockingQueue<string>();

            Scheduler.Start(
                MonitorForChanges(db, sourceFiles),
                TaskExecutionPolicy.RunAsBackgroundTask
            );

            Scheduler.Start(
                ScanFiles(db, sourceFiles),
                TaskExecutionPolicy.RunAsBackgroundTask
            );

            Scheduler.Start(
                UpdateIndex(db, sourceFiles),
                TaskExecutionPolicy.RunAsBackgroundTask
            );
        }

        public static IEnumerator<object> TransactionPump (TagDatabase db) {
            while (true) {
                if (Transaction != null) {
                    Transaction.Commit();
                    Transaction.Dispose();
                }

                Transaction = db.Connection.BeginTransaction();

                yield return new Sleep(30.0);
            }
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
            var changeQueue = new BlockingQueue<TagDatabase.Change>();
            var worker = Future.RunInThread(
                (Action<TagDatabase>)(
                    (db_) => {
                        Interlocked.Increment(ref NumWorkers);

                        var changeSet = db_.UpdateFileListAndGetChangeSet();
                        foreach (var change in changeSet)
                            changeQueue.Enqueue(change);

                        Interlocked.Decrement(ref NumWorkers);
                    }
                ),
                db
            );

            Interlocked.Increment(ref NumWorkers);

            int numDeletes = 0;
            while (true) {
                Interlocked.Decrement(ref NumWorkers);

                var f = changeQueue.Dequeue();
                yield return f;

                Interlocked.Increment(ref NumWorkers);

                var change = (TagDatabase.Change)f.Result;
                if (change.Deleted) {
                    yield return Future.RunInThread(
                        (Action<string>)(
                            (fn) => {
                                db.DeleteSourceFile(fn);
                            }
                        ),
                        change.Filename
                    );
                    numDeletes += 1;
                } else {
                    sourceFiles.Enqueue(change.Filename);
                }
            }
        }

        public static IEnumerator<object> UpdateIndex (TagDatabase db, BlockingQueue<string> sourceFiles) {
            var gen = new TagGenerator(
                @"C:\program files\ctags57\ctags.exe",
                "--filter=yes --filter-terminator=[[<>]]\n --fields=+afmikKlnsStz --sort=no"
            );

            var onNextFile = (Func<string, Future>)(
                (fn) => {
                    var lastWriteTime = System.IO.File.GetLastWriteTime(fn).ToFileTime();
                    return Future.RunInThread(
                        (Action)(() => {
                            db.DeleteTagsForFile(fn);
                            db.UpdateSourceFileTimestamp(fn, lastWriteTime);
                        })
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

                yield return Future.RunInThread(
                    (Func<Tag, int>)db.AddTag,
                    tag
                );
            }
        }

        public static IEnumerator<object> DeleteSourceFiles (TagDatabase db, string[] filenames) {
            foreach (string filename in filenames) {
                yield return Future.RunInThread(
                    (Action<string>)db.DeleteSourceFileOrFolder, filename
                );
            }
        }

        public static IEnumerator<object> MonitorForChanges (TagDatabase db, BlockingQueue<string> sourceFiles) {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;

            DiskMonitor monitor = new DiskMonitor(
                (from f in db.GetFolders() select f.Path).ToArray(),
                (from f in db.GetFilters() select f.Pattern).ToArray()
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
                    foreach (string filename in changedFiles)
                        sourceFiles.Enqueue(filename);
                    changedFiles.Clear();
                }
                if ((deletedFiles.Count > 0) && ((now - lastDiskChange) > updateInterval)) {
                    string[] filenames = deletedFiles.ToArray();
                    deletedFiles.Clear();
                    Scheduler.Start(
                        DeleteSourceFiles(db, filenames),
                        TaskExecutionPolicy.RunAsBackgroundTask
                    );
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
