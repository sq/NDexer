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
using exscape;
using Squared.Task.Data;

namespace Ndexer {
    public partial class ConfigurationDialog : Form {		
        TagDatabase DB;

        List<string> Folders = new List<string>();
        List<string> Filters = new List<string>();

        public bool NeedRestart = false;

        public ConfigurationDialog (TagDatabase db) 
            : base() {
            DB = db;

            InitializeComponent();

            cbTextEditor.Items.Clear();
            cbTextEditor.Items.AddRange(Program.GetDirectorNames());

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
            ilFileTypes.Images.Clear();

            foreach (var filter in Filters) {
                var item = new ListViewItem(filter);

                string fakeFile = filter.Replace("*", Program.GetExecutablePath() + @"\does_not_exist");

                Icon icon = Squared.Util.IO.ExtractAssociatedIcon(fakeFile, false);
                if (icon != null) {
                    ilFileTypes.Images.Add(filter, icon);
                    item.ImageKey = filter;
                }

                lvFileTypes.Items.Add(item);
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

        private IEnumerator<object> ReadHotkeyPreference (string hotkeyName, HotkeyControl hotkeyControl) {
            Future<string> f;
            yield return DB.GetPreference("Hotkeys." + hotkeyName + ".Key").Run(out f);
            hotkeyControl.Hotkey = (Keys)Enum.Parse(typeof(Keys), f.Result ?? "None", true);

            yield return DB.GetPreference("Hotkeys." + hotkeyName + ".Modifiers").Run(out f);
            hotkeyControl.HotkeyModifiers = (Keys)Enum.Parse(typeof(Keys), f.Result ?? "None", true);
        }

        private IEnumerator<object> WriteHotkeyPreference (string hotkeyName, HotkeyControl hotkeyControl) {
            yield return DB.SetPreference("Hotkeys." + hotkeyName + ".Key", hotkeyControl.Hotkey.ToString());

            yield return DB.SetPreference("Hotkeys." + hotkeyName + ".Modifiers", hotkeyControl.HotkeyModifiers.ToString());
        }

        private IEnumerator<object> ReadPreferences () {
            txtIndexLocation.Text = Program.DatabasePath;

            {
                Future<string> f;
                yield return DB.GetPreference("TextEditor.Name").Run(out f);
                cbTextEditor.Text = f.Result ?? "SciTE";

                yield return DB.GetPreference("TextEditor.Location").Run(out f);
                txtEditorLocation.Text = f.Result ?? @"C:\Program Files\SciTE\SciTE.exe";
            }

            yield return ReadHotkeyPreference("SearchFiles", hkSearchFiles);

            {
                Future<string[]> f;
                yield return DB.GetFolderPaths().Run(out f);
                Folders.AddRange(f.Result);
                RefreshFolderList();
            }

            using (var iter = new TaskEnumerator<TagDatabase.Filter>(DB.GetFilters()))
            while (!iter.Disposed) {
                yield return iter.Fetch();

                foreach (var filter in iter)
                    Filters.Add(filter.Pattern);
            }

            RefreshFileTypeList();
        }

        private IEnumerator<object> WritePreferences () {
            var errors = new List<string>();

            var transaction = DB.Connection.CreateTransaction();
            yield return transaction;

            yield return DB.SetPreference("TextEditor.Name", cbTextEditor.Text);
            yield return DB.SetPreference("TextEditor.Location", txtEditorLocation.Text);

            if (!System.IO.File.Exists(txtEditorLocation.Text))
                errors.Add(String.Format("The specified editor ('{0}') was not found.", txtEditorLocation.Text));

            yield return WriteHotkeyPreference("SearchFiles", hkSearchFiles);

            yield return DB.Connection.ExecuteSQL("DELETE FROM Folders");

            using (var query = DB.Connection.BuildQuery("INSERT INTO Folders (Folders_Path) VALUES (?)")) {
                foreach (string folder in Folders) {
                    yield return query.ExecuteNonQuery(folder);

                    if (!System.IO.Directory.Exists(folder))
                        errors.Add(String.Format("The specified folder ('{0}') was not found.", folder));
                }
            }

            yield return DB.Connection.ExecuteSQL("DELETE FROM Filters");

            using (var query = DB.Connection.BuildQuery("INSERT INTO Filters (Filters_Pattern) VALUES (?)"))
                foreach (var filter in Filters)
                    yield return query.ExecuteNonQuery(filter);

            if (errors.Count == 0) {
                yield return transaction.Commit();
                yield return Program.OnConfigurationChanged();
                yield return new Result(true);
            } else {
                yield return transaction.Rollback();
                System.Windows.Forms.MessageBox.Show(this, String.Join("\n", errors.ToArray()), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                yield return new Result(false);
            }
        }

        private void cmdCancel_Click (object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;

            this.Hide();
            this.Dispose();
        }

        private IEnumerator<object> onOK() {
            this.Enabled = false;
            this.UseWaitCursor = true;

            Future<bool> f;
            yield return WritePreferences().Run(out f);

            if (f.Result) {
                this.DialogResult = DialogResult.OK;

                this.Hide();
                this.Dispose();
            } else {
                this.Enabled = true;
                this.UseWaitCursor = false;
            }
        }

        private void cmdOK_Click (object sender, EventArgs e) {
            Program.Scheduler.Start(
                onOK(),
                TaskExecutionPolicy.RunAsBackgroundTask
            );
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

                    if (Folders.FirstOrDefault(
                            (f) => String.Compare(f, folderPath, StringComparison.CurrentCultureIgnoreCase) == 0
                        ) != null)
                        return;

                    Folders.Add(folderPath);
                    RefreshFolderList();
                    NeedRestart = true;
                }
            }
        }

        private void cmdAddFileType_Click (object sender, EventArgs e) {
            using (var dlg = new AddFilterDialog()) {
                if (dlg.ShowDialog(this) == DialogResult.OK) {
                    string[] filters = dlg.Filter.Split(' ', ';');

                    foreach (string filter in filters)
                        Filters.Add(filter);

                    RefreshFileTypeList();
                    NeedRestart = true;
                }
            }
        }

        private void cmdRemoveFileType_Click (object sender, EventArgs e) {
            var item = lvFileTypes.SelectedItems[0];
            Filters.Remove(item.Text);

            RefreshFileTypeList();
            NeedRestart = true;
        }

        private void lvFileTypes_SelectedIndexChanged (object sender, EventArgs e) {
            cmdRemoveFileType.Enabled = (lvFileTypes.SelectedIndices.Count > 0);
        }
    }
}
