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

namespace Ndexer {
    public partial class ConfigurationDialog : Form {
        TagDatabase DB;
        DataTable FilterTable, FolderTable;
        SQLiteDataAdapter FilterAdapter, FolderAdapter;
        SQLiteCommandBuilder FilterBuilder, FolderBuilder;

        public ConfigurationDialog (TagDatabase db) {
            DB = db;

            InitializeComponent();

            FilterTable = new DataTable("Filters");
            FolderTable = new DataTable("Folders");
            FilterAdapter = new SQLiteDataAdapter("SELECT Filters_ID, Filters_Pattern, Filters_Language FROM Filters", DB.NativeConnection);
            FolderAdapter = new SQLiteDataAdapter("SELECT Folders_ID, Folders_Path FROM Folders", DB.NativeConnection);
            FilterBuilder = new SQLiteCommandBuilder(FilterAdapter);
            FolderBuilder = new SQLiteCommandBuilder(FolderAdapter);
            FilterAdapter.Fill(FilterTable);
            FolderAdapter.Fill(FolderTable);
            bsFilters.DataSource = FilterTable;
            bsFolders.DataSource = FolderTable;
            dgFilters.DataSource = bsFilters;
            dgFolders.DataSource = bsFolders;

            cbTextEditor.Items.Clear();
            cbTextEditor.Items.AddRange(Program.GetDirectorNames());

            Program.Scheduler.WaitFor(
                Program.Scheduler.Start(
                    ReadPreferences(), 
                    TaskExecutionPolicy.RunAsBackgroundTask
            ));
        }

        public void DataBind (DataGridView dataGrid, BindingSource bindingSource, string tableName) {
            var table = new DataTable(tableName);
            var adapter = new SQLiteDataAdapter("SELECT * FROM " + tableName, DB.NativeConnection);

            adapter.Fill(table);
            bindingSource.DataSource = table;
            dataGrid.DataSource = bindingSource;
        }

        private IEnumerator<object> ReadPreferences () {
            var rtc = new RunToCompletion(DB.GetPreference("TextEditor.Name"));
            yield return rtc;
            cbTextEditor.Text = (rtc.Result as string) ?? "SciTE";
            rtc = new RunToCompletion(DB.GetPreference("TextEditor.Location"));
            yield return rtc;
            txtEditorLocation.Text = (rtc.Result as string) ?? @"C:\Program Files\SciTE\SciTE.exe";
        }

        private IEnumerator<object> WritePreferences () {
            yield return DB.SetPreference("TextEditor.Name", cbTextEditor.Text);
            yield return DB.SetPreference("TextEditor.Location", txtEditorLocation.Text);
        }

        private void cmdCancel_Click (object sender, EventArgs e) {
            this.Enabled = false;
            this.UseWaitCursor = true;

            this.Hide();
            this.Dispose();
        }

        private void cmdOK_Click (object sender, EventArgs e) {
            this.Enabled = false;
            this.UseWaitCursor = true;

            FilterAdapter.Update(FilterTable);
            FolderAdapter.Update(FolderTable);
            bsFilters.EndEdit();
            bsFolders.EndEdit();

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
    }
}
