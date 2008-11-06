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
        public AddFilterDialog (string[] languages) 
            : base () {

            InitializeComponent();

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
    }
}
