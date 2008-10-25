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

namespace Ndexer {
    static class Program {
        public static TaskScheduler Scheduler;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main (string[] argv) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (argv.Length < 1) {
                MessageBox.Show(
                    "NDexer cannot start without a path specified for the index database on the command line.\nFor example: ndexer.exe test.db",
                    "NDexer Error"
                );
                return;
            }

            string databasePath = System.IO.Path.GetFullPath(argv[0]);

            if (!System.IO.File.Exists(databasePath))
                System.IO.File.Copy(GetDataPath() + @"\ndexer.db", databasePath);

            Scheduler = new TaskScheduler(JobQueue.WindowsMessageBased);

            using (var db = new TagDatabase(databasePath)) {
                if (argv.Contains("--configure") || (db.GetFolders().Count() == 0) || (db.GetFilters().Count() == 0)) {
                    ShowConfiguration(db);
                }

                if (argv.Contains("--search")) {
                    ShowSearch(db);
                } else {
                    db.Compact();

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

        public static IEnumerator<object> ScanFiles (TagDatabase db, BlockingQueue<string> sourceFiles) {
            var changeQueue = new BlockingQueue<TagDatabase.Change>();
            var worker = Future.RunInThread(
                (Action<TagDatabase>)(
                    (db_) => {
                        var changeSet = db_.UpdateFileListAndGetChangeSet();
                        foreach (var change in changeSet)
                            changeQueue.Enqueue(change);
                    }
                ),
                db
            );
            
            int numDeletes = 0;
            while (true) {
                var f = changeQueue.Dequeue();

                yield return f;

                var change = (TagDatabase.Change)f.Result;
                if (change.Deleted) {
                    yield return Future.RunInThread(
                        (Action<string>)(
                            (fn) => {
                                using (var trans = db.Connection.BeginTransaction()) {
                                    db.DeleteSourceFile(fn);
                                    trans.Commit();
                                }
                            }
                        ),
                        change.Filename
                    );
                    numDeletes += 1;
                } else {
                    sourceFiles.Enqueue(change.Filename);
                }
            }

            if (numDeletes > 0)
                Console.WriteLine("Deleted {0} file(s) from database.", numDeletes);
        }

        public static IEnumerator<object> UpdateIndex (TagDatabase db, BlockingQueue<string> sourceFiles) {
            var gen = new TagGenerator(
                @"C:\program files\ctags57\ctags.exe",
                "--filter=yes --filter-terminator=[[<>]]\n --fields=+afmikKlnsStz --sort=no"
            );

            var onNextFile = (Func<string, Future>)(
                (fn_) => {
                    return Future.RunInThread(
                        (Action<string>)(
                            (fn) => {
                                var lastWriteTime = System.IO.File.GetLastWriteTime(fn).ToFileTime();
                                db.DeleteTagsForFile(fn);
                                db.UpdateSourceFileTimestamp(fn, lastWriteTime);
                            }
                        ),
                        fn_
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

            while (true) {
                var f = outputTags.Dequeue();
                yield return f;
                var tag = (Tag)f.Result;
                db.AddTag(tag);
            }
        }

        public static IEnumerator<object> MonitorForChanges (TagDatabase db, BlockingQueue<string> sourceFiles) {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;

            Icon trayIcon = Icon.FromHandle(Properties.Resources.database_monitoring.GetHicon());
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Icon = trayIcon;
            notifyIcon.Text = "NDexer (Monitoring)";
            notifyIcon.Visible = true;

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
                long now = DateTime.Now.Ticks;
                if ((changedFiles.Count > 0) && ((now - lastDiskChange) > updateInterval)) {
                    foreach (string filename in changedFiles)
                        sourceFiles.Enqueue(filename);
                    changedFiles.Clear();
                }
                if ((deletedFiles.Count > 0) && ((now - lastDiskChange) > updateInterval)) {
                    using (var trans = db.Connection.BeginTransaction()) {
                        foreach (string filename in deletedFiles)
                            db.DeleteSourceFileOrFolder(filename);
                        trans.Commit();
                        deletedFiles.Clear();
                    }
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

                yield return new Sleep(0.01);
            }
        }
    }
}
