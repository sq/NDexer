namespace Ndexer {
#if !MONO
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
            this.cmResults = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCopyFilenames = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopyFilenamesAndLineNumbers = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopyFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.ttBalloon = new System.Windows.Forms.ToolTip(this.components);
            this.tbrSearchOptions = new System.Windows.Forms.ToolStrip();
            this.btnCaseSensitive = new System.Windows.Forms.ToolStripButton();
            this.btnEnableRegex = new System.Windows.Forms.ToolStripButton();
            this.btnClearSearchField = new System.Windows.Forms.Button();
            this.sbStatus.SuspendLayout();
            this.cmResults.SuspendLayout();
            this.tbrSearchOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(2, 2);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(555, 30);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.WordWrap = false;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // sbStatus
            // 
            this.sbStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.pbProgress});
            this.sbStatus.Location = new System.Drawing.Point(0, 448);
            this.sbStatus.Name = "sbStatus";
            this.sbStatus.Size = new System.Drawing.Size(609, 25);
            this.sbStatus.TabIndex = 2;
            this.sbStatus.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.lblStatus.Size = new System.Drawing.Size(492, 20);
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
            this.pbProgress.Size = new System.Drawing.Size(100, 19);
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lbResults
            // 
            this.lbResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbResults.ContextMenuStrip = this.cmResults;
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
            this.lbResults.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbResults_DrawItem);
            this.lbResults.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbResults_KeyDown);
            this.lbResults.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbResults_MouseDoubleClick);
            // 
            // cmResults
            // 
            this.cmResults.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCopyFilenames,
            this.mnuCopyFilenamesAndLineNumbers,
            this.mnuCopyFiles});
            this.cmResults.Name = "cmResults";
            this.cmResults.Size = new System.Drawing.Size(259, 70);
            // 
            // mnuCopyFilenames
            // 
            this.mnuCopyFilenames.Image = ((System.Drawing.Image)(resources.GetObject("mnuCopyFilenames.Image")));
            this.mnuCopyFilenames.Name = "mnuCopyFilenames";
            this.mnuCopyFilenames.Size = new System.Drawing.Size(258, 22);
            this.mnuCopyFilenames.Text = "&Copy Filenames";
            this.mnuCopyFilenames.Click += new System.EventHandler(this.mnuCopyFilenames_Click);
            // 
            // mnuCopyFilenamesAndLineNumbers
            // 
            this.mnuCopyFilenamesAndLineNumbers.Image = ((System.Drawing.Image)(resources.GetObject("mnuCopyFilenamesAndLineNumbers.Image")));
            this.mnuCopyFilenamesAndLineNumbers.Name = "mnuCopyFilenamesAndLineNumbers";
            this.mnuCopyFilenamesAndLineNumbers.Size = new System.Drawing.Size(258, 22);
            this.mnuCopyFilenamesAndLineNumbers.Text = "Copy Filenames and Line &Numbers";
            this.mnuCopyFilenamesAndLineNumbers.Click += new System.EventHandler(this.mnuCopyFilenamesAndLineNumbers_Click);
            // 
            // mnuCopyFiles
            // 
            this.mnuCopyFiles.Image = ((System.Drawing.Image)(resources.GetObject("mnuCopyFiles.Image")));
            this.mnuCopyFiles.Name = "mnuCopyFiles";
            this.mnuCopyFiles.Size = new System.Drawing.Size(258, 22);
            this.mnuCopyFiles.Text = "Copy &Files";
            this.mnuCopyFiles.Click += new System.EventHandler(this.mnuCopyFiles_Click);
            // 
            // ttBalloon
            // 
            this.ttBalloon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.ttBalloon.ForeColor = System.Drawing.Color.Black;
            this.ttBalloon.IsBalloon = true;
            // 
            // tbrSearchOptions
            // 
            this.tbrSearchOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbrSearchOptions.AutoSize = false;
            this.tbrSearchOptions.CanOverflow = false;
            this.tbrSearchOptions.Dock = System.Windows.Forms.DockStyle.None;
            this.tbrSearchOptions.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tbrSearchOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCaseSensitive,
            this.btnEnableRegex});
            this.tbrSearchOptions.Location = new System.Drawing.Point(560, 3);
            this.tbrSearchOptions.Name = "tbrSearchOptions";
            this.tbrSearchOptions.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tbrSearchOptions.Size = new System.Drawing.Size(47, 25);
            this.tbrSearchOptions.TabIndex = 4;
            this.tbrSearchOptions.TabStop = true;
            this.tbrSearchOptions.Text = "toolStrip1";
            // 
            // btnCaseSensitive
            // 
            this.btnCaseSensitive.Checked = true;
            this.btnCaseSensitive.CheckOnClick = true;
            this.btnCaseSensitive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnCaseSensitive.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCaseSensitive.Image = ((System.Drawing.Image)(resources.GetObject("btnCaseSensitive.Image")));
            this.btnCaseSensitive.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCaseSensitive.Name = "btnCaseSensitive";
            this.btnCaseSensitive.Size = new System.Drawing.Size(23, 22);
            this.btnCaseSensitive.ToolTipText = "Case Sensitive";
            this.btnCaseSensitive.Click += new System.EventHandler(this.btnCaseSensitive_Click);
            // 
            // btnEnableRegex
            // 
            this.btnEnableRegex.Checked = true;
            this.btnEnableRegex.CheckOnClick = true;
            this.btnEnableRegex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnEnableRegex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEnableRegex.Image = ((System.Drawing.Image)(resources.GetObject("btnEnableRegex.Image")));
            this.btnEnableRegex.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEnableRegex.Name = "btnEnableRegex";
            this.btnEnableRegex.Size = new System.Drawing.Size(23, 22);
            this.btnEnableRegex.ToolTipText = "Enable Regular Expressions";
            this.btnEnableRegex.Click += new System.EventHandler(this.btnEnableRegex_Click);
            // 
            // btnClearSearchField
            // 
            this.btnClearSearchField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearSearchField.Image = ((System.Drawing.Image)(resources.GetObject("btnClearSearchField.Image")));
            this.btnClearSearchField.Location = new System.Drawing.Point(532, 3);
            this.btnClearSearchField.Name = "btnClearSearchField";
            this.btnClearSearchField.Size = new System.Drawing.Size(24, 24);
            this.btnClearSearchField.TabIndex = 5;
            this.btnClearSearchField.UseVisualStyleBackColor = true;
            this.btnClearSearchField.Visible = false;
            this.btnClearSearchField.Click += new System.EventHandler(this.btnClearSearchField_Click);
            // 
            // FindInFilesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 473);
            this.Controls.Add(this.btnClearSearchField);
            this.Controls.Add(this.lbResults);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.sbStatus);
            this.Controls.Add(this.tbrSearchOptions);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FindInFilesDialog";
            this.Text = "Search";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchDialog_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SearchDialog_FormClosed);
            this.sbStatus.ResumeLayout(false);
            this.sbStatus.PerformLayout();
            this.cmResults.ResumeLayout(false);
            this.tbrSearchOptions.ResumeLayout(false);
            this.tbrSearchOptions.PerformLayout();
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
        private System.Windows.Forms.ContextMenuStrip cmResults;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyFilenames;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyFilenamesAndLineNumbers;
        private System.Windows.Forms.ToolStrip tbrSearchOptions;
        private System.Windows.Forms.ToolStripButton btnCaseSensitive;
        private System.Windows.Forms.ToolStripButton btnEnableRegex;
        private System.Windows.Forms.Button btnClearSearchField;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyFiles;
    }
#endif
}

