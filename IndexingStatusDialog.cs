using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ndexer {
    public partial class IndexingStatusDialog : Form {
        public IndexingStatusDialog () {
            InitializeComponent();
        }

        public void SetStatus (string status, float progress) {
            lblStatus.Text = status;
            if (progress >= 0) {
                pbStatus.Value = (int)(progress * 1000);
                pbStatus.Style = ProgressBarStyle.Continuous;
            } else {
                pbStatus.Style = ProgressBarStyle.Marquee;
            }
            Application.DoEvents();
        }
    }
}
