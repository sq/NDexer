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
    public partial class AddFilterDialog : Form {
        Dictionary<string, string> LanguageMaps;

        public AddFilterDialog (string[] languages) 
            : base () {

            InitializeComponent();

            LanguageMaps = Program.GetLanguageMaps();

            cmbLanguage.Items.Clear();
            cmbLanguage.Items.AddRange(languages);
            cmbLanguage.SelectedIndex = 0;
        }

        public string Language {
            get {
                return cmbLanguage.Text;
            }
        }

        public string Filter {
            get {
                return txtFilter.Text;
            }
        }

        private void cmbLanguage_SelectedIndexChanged (object sender, EventArgs e) {
            if ((txtFilter.Text.Length == 0) || (txtFilter.SelectionLength > 0)) {
                string filter;
                if (LanguageMaps.TryGetValue(cmbLanguage.Text, out filter)) {
                    txtFilter.SelectedText = filter;
                    txtFilter.Select(0, 0);
                    txtFilter.SelectAll();
                }
            }
        }
    }
}
