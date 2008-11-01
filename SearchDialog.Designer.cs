namespace Ndexer {
    partial class SearchDialog {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchDialog));
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.sbStatus = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tcFilter = new System.Windows.Forms.TabControl();
            this.tpFindTags = new System.Windows.Forms.TabPage();
            this.tpFindFiles = new System.Windows.Forms.TabPage();
            this.lvResults = new Ndexer.SearchResultListView();
            this.sbStatus.SuspendLayout();
            this.tcFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.Location = new System.Drawing.Point(2, 23);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(341, 23);
            this.txtFilter.TabIndex = 0;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            this.txtFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFilter_KeyDown);
            // 
            // sbStatus
            // 
            this.sbStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.pbProgress});
            this.sbStatus.Location = new System.Drawing.Point(0, 406);
            this.sbStatus.Name = "sbStatus";
            this.sbStatus.Size = new System.Drawing.Size(345, 22);
            this.sbStatus.TabIndex = 2;
            this.sbStatus.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.lblStatus.Size = new System.Drawing.Size(268, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "0 result(s) found";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbProgress
            // 
            this.pbProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.pbProgress.MarqueeAnimationSpeed = 50;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.pbProgress.Size = new System.Drawing.Size(60, 16);
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // tcFilter
            // 
            this.tcFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcFilter.Controls.Add(this.tpFindTags);
            this.tcFilter.Controls.Add(this.tpFindFiles);
            this.tcFilter.Location = new System.Drawing.Point(2, 2);
            this.tcFilter.Name = "tcFilter";
            this.tcFilter.SelectedIndex = 0;
            this.tcFilter.Size = new System.Drawing.Size(341, 44);
            this.tcFilter.TabIndex = 2;
            this.tcFilter.SelectedIndexChanged += new System.EventHandler(this.tcFilter_SelectedIndexChanged);
            // 
            // tpFindTags
            // 
            this.tpFindTags.Location = new System.Drawing.Point(4, 22);
            this.tpFindTags.Name = "tpFindTags";
            this.tpFindTags.Padding = new System.Windows.Forms.Padding(3);
            this.tpFindTags.Size = new System.Drawing.Size(333, 18);
            this.tpFindTags.TabIndex = 0;
            this.tpFindTags.Text = "Find Tags";
            this.tpFindTags.UseVisualStyleBackColor = true;
            // 
            // tpFindFiles
            // 
            this.tpFindFiles.Location = new System.Drawing.Point(4, 22);
            this.tpFindFiles.Name = "tpFindFiles";
            this.tpFindFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tpFindFiles.Size = new System.Drawing.Size(333, 18);
            this.tpFindFiles.TabIndex = 1;
            this.tpFindFiles.Text = "Find Files";
            this.tpFindFiles.UseVisualStyleBackColor = true;
            // 
            // lvResults
            // 
            this.lvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvResults.AutoArrange = false;
            this.lvResults.FullRowSelect = true;
            this.lvResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvResults.HideSelection = false;
            this.lvResults.LabelWrap = false;
            this.lvResults.Location = new System.Drawing.Point(2, 48);
            this.lvResults.MultiSelect = false;
            this.lvResults.Name = "lvResults";
            this.lvResults.OwnerDraw = true;
            this.lvResults.ShowGroups = false;
            this.lvResults.Size = new System.Drawing.Size(341, 356);
            this.lvResults.TabIndex = 1;
            this.lvResults.UseCompatibleStateImageBehavior = false;
            this.lvResults.View = System.Windows.Forms.View.Details;
            this.lvResults.VirtualMode = true;
            this.lvResults.SizeChanged += new System.EventHandler(this.lvResults_SizeChanged);
            this.lvResults.DoubleClick += new System.EventHandler(this.lvResults_DoubleClick);
            this.lvResults.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvResults_RetrieveVirtualItem);
            this.lvResults.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.lvResults_DrawSubItem);
            // 
            // SearchDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(345, 428);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.tcFilter);
            this.Controls.Add(this.lvResults);
            this.Controls.Add(this.sbStatus);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SearchDialog";
            this.Text = "Search";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchDialog_FormClosing);
            this.sbStatus.ResumeLayout(false);
            this.sbStatus.PerformLayout();
            this.tcFilter.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.StatusStrip sbStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar pbProgress;
        private SearchResultListView lvResults;
        private System.Windows.Forms.TabControl tcFilter;
        private System.Windows.Forms.TabPage tpFindTags;
        private System.Windows.Forms.TabPage tpFindFiles;
    }
}

