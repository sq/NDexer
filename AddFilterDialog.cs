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
        public AddFilterDialog () 
            : base () {

            InitializeComponent();
        }

        public string Filter {
            get {
                return txtFilter.Text;
            }
        }
    }
}
