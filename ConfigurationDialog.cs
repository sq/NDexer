using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using Squared.Task;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Ndexer {
    public partial class ConfigurationDialog : Form {		
        TagDatabase DB;

        string[] SupportedLanguages;

        List<string> Folders = new List<string>();
        Dictionary<string, List<string>> FileTypes = new Dictionary<string, List<string>>();

        public bool NeedRestart = false;

        public ConfigurationDialog (TagDatabase db) 
            : base() {
            DB = db;

            InitializeComponent();

            cbTextEditor.Items.Clear();
            cbTextEditor.Items.AddRange(Program.GetDirectorNames());

            SupportedLanguages = Program.GetLanguageNames();

            Program.Scheduler.WaitFor(
                Program.Scheduler.Start(
                    ReadPreferences(), 
                    TaskExecutionPolicy.RunAsBackgroundTask
            ));
        }

        private void RefreshFileTypeList () {
            int? oldIndex = null;
            if (lvFileTypes.SelectedIndices.Count > 0)
                oldIndex = lvFileTypes.SelectedIndices[0];

            lvFileTypes.BeginUpdate();
            lvFileTypes.Items.Clear();
            lvFileTypes.Groups.Clear();
            ilFileTypes.Images.Clear();

            foreach (var ft in FileTypes) {
                lvFileTypes.Groups.Add(ft.Key, ft.Key);

                foreach (var filter in ft.Value) {
                    var item = new ListViewItem(filter);
                    item.Group = lvFileTypes.Groups[ft.Key];
                    string fakeFile = filter.Replace("*", Program.GetExecutablePath() + @"\does_not_exist");
                    Icon icon = Squared.Util.IO.ExtractAssociatedIcon(fakeFile, false);
                    if (icon != null) {
                        ilFileTypes.Images.Add(filter, icon);
                        item.ImageKey = filter;
                    }
                    lvFileTypes.Items.Add(item);
                }
            }

            if ((lvFileTypes.Items.Count > 0) && (oldIndex.HasValue))
                lvFileTypes.SelectedIndices.Add(Math.Min(oldIndex.Value, lvFileTypes.Items.Count - 1));

            lvFileTypes.EndUpdate();

            if (lvFileTypes.SelectedIndices.Count > 0)
                lvFileTypes.EnsureVisible(lvFileTypes.SelectedIndices[0]);

            cmdRemoveFileType.Enabled = (lvFileTypes.SelectedIndices.Count > 0);
        }

        private void RefreshFolderList () {
            int? oldIndex = null;
            if (lvFolders.SelectedIndices.Count > 0)
                oldIndex = lvFolders.SelectedIndices[0];

            lvFolders.BeginUpdate();
            lvFolders.Items.Clear();
            ilFolders.Images.Clear();

            foreach (string folder in Folders) {
                string folderPath = System.IO.Path.GetFullPath(folder);
                Icon icon = Squared.Util.IO.ExtractAssociatedIcon(folderPath, false);
                ilFolders.Images.Add(folder, icon);
                lvFolders.Items.Add(folder, folder);
            }

            if ((lvFolders.Items.Count > 0) && (oldIndex.HasValue))
                lvFolders.SelectedIndices.Add(Math.Min(oldIndex.Value, lvFolders.Items.Count - 1));

            lvFolders.EndUpdate();

            if (lvFolders.SelectedIndices.Count > 0)
                lvFolders.EnsureVisible(lvFolders.SelectedIndices[0]);

            cmdRemoveFolder.Enabled = (lvFolders.SelectedIndices.Count > 0);
        }

        private IEnumerator<object> ReadPreferences () {
            var rtc = new RunToCompletion(DB.GetPreference("TextEditor.Name"));
            yield return rtc;
            cbTextEditor.Text = (rtc.Result as string) ?? "SciTE";

            rtc = new RunToCompletion(DB.GetPreference("TextEditor.Location"));
            yield return rtc;
            txtEditorLocation.Text = (rtc.Result as string) ?? @"C:\Program Files\SciTE\SciTE.exe";

            rtc = new RunToCompletion(DB.GetFolderPaths());
            yield return rtc;
            Folders.AddRange(rtc.Result as string[]);
            RefreshFolderList();

            var iter = new TaskIterator<TagDatabase.Filter>(DB.GetFilters());
            yield return iter.Start();
            while (!iter.Disposed) {
                var filter = iter.Current;
                if (!FileTypes.ContainsKey(filter.Language))
                    FileTypes[filter.Language] = new List<string>();

                FileTypes[filter.Language].Add(filter.Pattern);

                yield return iter.MoveNext();
            }
            RefreshFileTypeList();
        }

        private IEnumerator<object> WritePreferences () {
            var transaction = DB.Connection.CreateTransaction();
            yield return transaction;

            yield return DB.SetPreference("TextEditor.Name", cbTextEditor.Text);
            yield return DB.SetPreference("TextEditor.Location", txtEditorLocation.Text);

            yield return DB.Connection.ExecuteSQL("DELETE FROM Folders");

            using (var query = DB.Connection.BuildQuery("INSERT INTO Folders (Folders_Path) VALUES (?)"))
                foreach (string folder in Folders)
                    yield return query.ExecuteNonQuery(folder);

            yield return DB.Connection.ExecuteSQL("DELETE FROM Filters");

            using (var query = DB.Connection.BuildQuery("INSERT INTO Filters (Filters_Language, Filters_Pattern) VALUES (?, ?)"))
                foreach (var ft in FileTypes)
                    foreach (string filter in ft.Value)
                        yield return query.ExecuteNonQuery(ft.Key, filter);

            yield return transaction.Commit();
        }

        private void cmdCancel_Click (object sender, EventArgs e) {
            this.Hide();
            this.Dispose();
        }

        private void cmdOK_Click (object sender, EventArgs e) {
            this.Enabled = false;
            this.UseWaitCursor = true;

            Program.Scheduler.WaitFor(
                Program.Scheduler.Start(
                    WritePreferences(),
                    TaskExecutionPolicy.RunAsBackgroundTask
            ));

            this.Hide();
            this.Dispose();
        }

        private void cmdBrowseForEditor_Click (object sender, EventArgs e) {
            using (var ofd = new OpenFileDialog()) {
                ofd.Title = "Browse for text editor";
                ofd.FileName = txtEditorLocation.Text;
                ofd.Filter = "Executables|*.exe";
                ofd.ShowReadOnly = false;
                if (ofd.ShowDialog(this) == DialogResult.OK)
                    txtEditorLocation.Text = ofd.FileName;
            }
        }

        private void lvFolders_SelectedIndexChanged (object sender, EventArgs e) {
            cmdRemoveFolder.Enabled = (lvFolders.SelectedIndices.Count > 0);
        }

        private void cmdRemoveFolder_Click (object sender, EventArgs e) {
            Folders.RemoveAt(lvFolders.SelectedIndices[0]);
            RefreshFolderList();
            NeedRestart = true;
        }

        private void cmdAddFolder_Click (object sender, EventArgs e) {
            using (var fbd = new FolderBrowserDialog()) {
                fbd.Description = "Select folder";
                fbd.ShowNewFolderButton = false;
                if (fbd.ShowDialog(this) == DialogResult.OK) {
                    string folderPath = System.IO.Path.GetFullPath(fbd.SelectedPath);
                    if (!folderPath.EndsWith("\\"))
                        folderPath += "\\";
                    Folders.Add(folderPath);
                    RefreshFolderList();
                    NeedRestart = true;
                }
            }
        }

        private void cmdAddFileType_Click (object sender, EventArgs e) {
            using (var dlg = new AddFilterDialog(SupportedLanguages)) {
                if (dlg.ShowDialog(this) == DialogResult.OK) {
                    if (!FileTypes.ContainsKey(dlg.Language))
                        FileTypes[dlg.Language] = new List<string>();

                    string[] filters = dlg.Filter.Split(' ', ';');

                    foreach (string filter in filters)
                        FileTypes[dlg.Language].Add(filter);

                    RefreshFileTypeList();
                    NeedRestart = true;
                }
            }
        }

        private void cmdRemoveFileType_Click (object sender, EventArgs e) {
            var item = lvFileTypes.SelectedItems[0];
            FileTypes[item.Group.Name].Remove(item.Text);

            if (FileTypes[item.Group.Name].Count == 0)
                FileTypes.Remove(item.Group.Name);

            RefreshFileTypeList();
            NeedRestart = true;
        }

        private void lvFileTypes_SelectedIndexChanged (object sender, EventArgs e) {
            cmdRemoveFileType.Enabled = (lvFileTypes.SelectedIndices.Count > 0);
        }
    }
}
