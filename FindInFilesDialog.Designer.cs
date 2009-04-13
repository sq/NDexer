namespace Ndexer {
    partial class FindInFilesDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindInFilesDialog));
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.sbStatus = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.lbResults = new Ndexer.SearchResultListBox();
            this.ttBalloon = new System.Windows.Forms.ToolTip(this.components);
            this.sbStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(2, 2);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(605, 26);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.WordWrap = false;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // sbStatus
            // 
            this.sbStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.pbProgress});
            this.sbStatus.Location = new System.Drawing.Point(0, 451);
            this.sbStatus.Name = "sbStatus";
            this.sbStatus.Size = new System.Drawing.Size(609, 22);
            this.sbStatus.TabIndex = 2;
            this.sbStatus.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.lblStatus.Size = new System.Drawing.Size(532, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "0 result(s) found";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbProgress
            // 
            this.pbProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.pbProgress.MarqueeAnimationSpeed = 50;
            this.pbProgress.Maximum = 1000;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.pbProgress.Size = new System.Drawing.Size(60, 16);
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lbResults
            // 
            this.lbResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbResults.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbResults.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbResults.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lbResults.HorizontalScrollbar = true;
            this.lbResults.IntegralHeight = false;
            this.lbResults.Location = new System.Drawing.Point(2, 30);
            this.lbResults.Name = "lbResults";
            this.lbResults.ScrollAlwaysVisible = true;
            this.lbResults.Size = new System.Drawing.Size(605, 420);
            this.lbResults.TabIndex = 1;
            this.lbResults.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbResults_MouseDoubleClick);
            this.lbResults.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbResults_DrawItem);
            // 
            // ttBalloon
            // 
            this.ttBalloon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.ttBalloon.ForeColor = System.Drawing.Color.Black;
            this.ttBalloon.IsBalloon = true;
            // 
            // FindInFilesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(609, 473);
            this.Controls.Add(this.lbResults);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.sbStatus);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FindInFilesDialog";
            this.Text = "Find In Files";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SearchDialog_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchDialog_FormClosing);
            this.sbStatus.ResumeLayout(false);
            this.sbStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.StatusStrip sbStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar pbProgress;
        private SearchResultListBox lbResults;
        private System.Windows.Forms.ToolTip ttBalloon;
    }
}

