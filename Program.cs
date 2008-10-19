using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Data.SQLite;
using Squared.Task;
using System.Drawing;

namespace Ndexer {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main (string[] argv) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string output = @"C:\imvu\ndexer.db";
            using (var db = new TagDatabase(output)) {
                if (argv.Contains("--configure")) {
                    ShowConfiguration(db);
                } 
                
                if (argv.Contains("--search")) {
                    ShowSearch(db);
                } else {
                    UpdateIndex(db, ScanFiles(db));
                    db.Compact();
                }

                if (argv.Contains("--monitor")) {
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
                            UpdateIndex(db, changedFiles.ToArray());
                            changedFiles.Clear();
                        }
                        if ((deletedFiles.Count > 0) && ((now - lastDiskChange) > updateInterval)) {
                            using (var trans = db.Connection.BeginTransaction()) {
                                foreach (string filename in deletedFiles)
                                    db.DeleteSourceFileOrFolder(filename);
                                trans.Commit();
                            }
                            deletedFiles.Clear();
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

                        Application.DoEvents();
                        System.Threading.Thread.Sleep(1);
                    }
                }
            }
        }

        public static void ShowSearch (TagDatabase db) {
            Application.Run(new SearchDialog(db));
        }

        public static void ShowConfiguration (TagDatabase db) {
            Application.Run(new ConfigurationDialog(db));
        }

        public static string[] ScanFiles (TagDatabase db) {
            var status = new IndexingStatusDialog();
            status.Show();
            status.SetStatus("Scanning files...", -1);

            var changeSet = db.UpdateFileListAndGetChangeSet();
            var sourceFiles = new List<string>();
            int numDeletes = 0;
            using (var trans = db.Connection.BeginTransaction()) {
                foreach (var change in changeSet) {
                    if (change.Deleted) {
                        db.DeleteSourceFile(change.Filename);
                        numDeletes += 1;
                    } else {
                        sourceFiles.Add(change.Filename);
                    }
                    Application.DoEvents();
                }
                trans.Commit();
            }

            if (numDeletes > 0)
                Console.WriteLine("Deleted {0} file(s) from database.", numDeletes);

            status.Hide();
            status.Dispose();

            return sourceFiles.ToArray();
        }

        public static void UpdateIndex (TagDatabase db, string[] sourceFiles) {
            if (sourceFiles.Length == 0)
                return;

            var status = new IndexingStatusDialog();
            status.Show();
            status.SetStatus("Updating index...", -1);

            using (var trans = db.Connection.BeginTransaction()) {
                foreach (string filename in sourceFiles) {
                    db.DeleteTagsForFile(filename);
                    db.UpdateSourceFileTimestamp(filename, System.IO.File.GetLastWriteTime(filename).ToFileTime());
                }

                var gen = new TagGenerator(
                    @"C:\program files\ctags57\ctags.exe",
                    "--filter=yes --filter-terminator=[[<>]]\n --fields=+afmikKlnsStz --sort=no"
                );

                var inputLines = gen.GenerateTags(sourceFiles, sourceFiles.Length, status);
                foreach (Tag tag in TagReader.ReadTags(inputLines)) {
                    db.AddTag(tag);
                    if (!status.Visible)
                        break;
                }

                trans.Commit();
            }

            status.Hide();
            status.Dispose();
        }
    }
}
