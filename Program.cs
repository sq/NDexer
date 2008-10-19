using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Data.SQLite;
using Squared.Task;

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
                DiskMonitor monitor = new DiskMonitor(
                    (from f in db.GetFolders() select f.Path).ToArray(),
                    (from f in db.GetFilters() select f.Pattern).ToArray()
                );
                monitor.Monitoring = true;

                ShowConfiguration(db);
                UpdateIndex(db);

                while (true) {
                    foreach (string filename in monitor.GetChangedFiles().Distinct())
                        Console.Out.WriteLine(filename);
                    foreach (string filename in monitor.GetDeletedFiles().Distinct())
                        Console.Out.WriteLine("{0} deleted", filename);

                    Application.DoEvents();
                    System.Threading.Thread.Sleep(1);
                }

            }
        }

        public static void ShowSearch (TagDatabase db) {
            Application.Run(new SearchDialog(db));
        }

        public static void ShowConfiguration (TagDatabase db) {
            Application.Run(new ConfigurationDialog(db));
        }

        public static void UpdateIndex (TagDatabase db) {
            var status = new IndexingStatusDialog();
            status.Show();
            status.SetStatus("Scanning files...", -1);

            var changeSet = db.UpdateFileListAndGetChangeSet();
            var sourceFiles = new List<string>();
            using (var trans = db.Connection.BeginTransaction()) {
                foreach (var change in changeSet) {
                    if (change.Deleted) {
                        db.DeleteSourceFile(change.Filename);
                    } else {
                        sourceFiles.Add(change.Filename);
                    }
                }
                trans.Commit();
            }

            string[] sf = new string[Math.Min(50, sourceFiles.Count)];
            sourceFiles.CopyTo(0, sf, 0, Math.Min(50, sourceFiles.Count));
            Console.WriteLine("{0} file(s): {1}", sourceFiles.Count, String.Join(",", sf));

            if (sourceFiles.Count > 0) {
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
                    var inputLines = gen.GenerateTags(sourceFiles, sourceFiles.Count, status);
                    foreach (Tag tag in TagReader.ReadTags(inputLines)) {
                        db.AddTag(tag);
                        if (!status.Visible)
                            break;
                    }

                    trans.Commit();
                }
            }

            status.Hide();
            status.Dispose();
        }
    }
}
