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
    public partial class SearchDialog : Form {
        TagDatabase Tags;

        public SearchDialog (TagDatabase tags) {
            Tags = tags;

            InitializeComponent();

            DataBind();
        }

        private void txtFilter_TextChanged (object sender, EventArgs e) {
            bsResults.Filter = "Tags_Name LIKE '" + txtFilter.Text + "'";
        }

        public void DataBind () {
            var table = new DataTable("Tags");
            var adapter = new SQLiteDataAdapter("SELECT Tags_Name FROM Tags", Tags.Connection);

            adapter.Fill(table);
            bsResults.DataSource = table;
            bsResults.Sort = "Tags_Name";
            bsResults.Filter = "Tags_Name LIKE ''";
            dgResults.DataSource = bsResults;
        }
    }
}
