using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Ndexer {
    public partial class ConfigurationDialog : Form {
        TagDatabase Tags;
        DataTable FilterTable, FolderTable;
        SQLiteDataAdapter FilterAdapter, FolderAdapter;
        SQLiteCommandBuilder FilterBuilder, FolderBuilder;

        public ConfigurationDialog (TagDatabase tags) {
            Tags = tags;
            //Transaction = Tags.Connection.BeginTransaction();

            InitializeComponent();

            FilterTable = new DataTable("Filters");
            FolderTable = new DataTable("Folders");
            FilterAdapter = new SQLiteDataAdapter("SELECT Filters_ID, Filters_Pattern, Filters_Language FROM Filters", Tags.NativeConnection);
            FolderAdapter = new SQLiteDataAdapter("SELECT Folders_ID, Folders_Path FROM Folders", Tags.NativeConnection);
            FilterBuilder = new SQLiteCommandBuilder(FilterAdapter);
            FolderBuilder = new SQLiteCommandBuilder(FolderAdapter);
            FilterAdapter.Fill(FilterTable);
            FolderAdapter.Fill(FolderTable);
            bsFilters.DataSource = FilterTable;
            bsFolders.DataSource = FolderTable;
            dgFilters.DataSource = bsFilters;
            dgFolders.DataSource = bsFolders;
        }

        public void DataBind (DataGridView dataGrid, BindingSource bindingSource, string tableName) {
            var table = new DataTable(tableName);
            var adapter = new SQLiteDataAdapter("SELECT * FROM " + tableName, Tags.NativeConnection);

            adapter.Fill(table);
            bindingSource.DataSource = table;
            dataGrid.DataSource = bindingSource;
        }

        private void cmdCancel_Click (object sender, EventArgs e) {
            this.Hide();
            this.Dispose();
        }

        private void cmdOK_Click (object sender, EventArgs e) {
            FilterAdapter.Update(FilterTable);
            FolderAdapter.Update(FolderTable);
            bsFilters.EndEdit();
            bsFolders.EndEdit();
            this.Hide();
            this.Dispose();
        }
    }
}
