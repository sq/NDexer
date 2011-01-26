namespace Ndexer {
#if !MONO
    partial class ConfigurationDialog {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationDialog));
            this.ilFileTypes = new System.Windows.Forms.ImageList(this.components);
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.tcTabs = new System.Windows.Forms.TabControl();
            this.tabIndex = new System.Windows.Forms.TabPage();
            this.txtIndexLocation = new System.Windows.Forms.TextBox();
            this.lblIndexLocation = new System.Windows.Forms.Label();
            this.gbFileTypes = new System.Windows.Forms.GroupBox();
            this.cmdAddFileType = new System.Windows.Forms.Button();
            this.cmdRemoveFileType = new System.Windows.Forms.Button();
            this.lvFileTypes = new System.Windows.Forms.ListView();
            this.colFilter = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbFolders = new System.Windows.Forms.GroupBox();
            this.dgFolders = new System.Windows.Forms.DataGridView();
            this.colFolderPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colExcludeFolder = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cmdAddFolder = new System.Windows.Forms.Button();
            this.cmdRemoveFolder = new System.Windows.Forms.Button();
            this.tabUI = new System.Windows.Forms.TabPage();
            this.gbHotkeys = new System.Windows.Forms.GroupBox();
            this.lblSearchFiles = new System.Windows.Forms.Label();
            this.hkSearchFiles = new exscape.HotkeyControl();
            this.gbEditor = new System.Windows.Forms.GroupBox();
            this.cmdBrowseForEditor = new System.Windows.Forms.Button();
            this.txtEditorLocation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbTextEditor = new System.Windows.Forms.ComboBox();
            this.tcTabs.SuspendLayout();
            this.tabIndex.SuspendLayout();
            this.gbFileTypes.SuspendLayout();
            this.gbFolders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgFolders)).BeginInit();
            this.tabUI.SuspendLayout();
            this.gbHotkeys.SuspendLayout();
            this.gbEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // ilFileTypes
            // 
            this.ilFileTypes.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilFileTypes.ImageSize = new System.Drawing.Size(16, 16);
            this.ilFileTypes.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(401, 484);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(4);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(120, 31);
            this.cmdCancel.TabIndex = 2;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOK.Location = new System.Drawing.Point(279, 484);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(4);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(120, 31);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // tcTabs
            // 
            this.tcTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcTabs.Controls.Add(this.tabIndex);
            this.tcTabs.Controls.Add(this.tabUI);
            this.tcTabs.Location = new System.Drawing.Point(4, 4);
            this.tcTabs.Margin = new System.Windows.Forms.Padding(4);
            this.tcTabs.Name = "tcTabs";
            this.tcTabs.SelectedIndex = 0;
            this.tcTabs.Size = new System.Drawing.Size(517, 475);
            this.tcTabs.TabIndex = 7;
            // 
            // tabIndex
            // 
            this.tabIndex.Controls.Add(this.txtIndexLocation);
            this.tabIndex.Controls.Add(this.lblIndexLocation);
            this.tabIndex.Controls.Add(this.gbFileTypes);
            this.tabIndex.Controls.Add(this.gbFolders);
            this.tabIndex.Location = new System.Drawing.Point(4, 25);
            this.tabIndex.Margin = new System.Windows.Forms.Padding(4);
            this.tabIndex.Name = "tabIndex";
            this.tabIndex.Padding = new System.Windows.Forms.Padding(4);
            this.tabIndex.Size = new System.Drawing.Size(509, 446);
            this.tabIndex.TabIndex = 0;
            this.tabIndex.Text = "Indexing";
            this.tabIndex.UseVisualStyleBackColor = true;
            // 
            // txtIndexLocation
            // 
            this.txtIndexLocation.Location = new System.Drawing.Point(123, 10);
            this.txtIndexLocation.Margin = new System.Windows.Forms.Padding(4);
            this.txtIndexLocation.Name = "txtIndexLocation";
            this.txtIndexLocation.ReadOnly = true;
            this.txtIndexLocation.Size = new System.Drawing.Size(376, 22);
            this.txtIndexLocation.TabIndex = 5;
            // 
            // lblIndexLocation
            // 
            this.lblIndexLocation.AutoSize = true;
            this.lblIndexLocation.Location = new System.Drawing.Point(8, 14);
            this.lblIndexLocation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIndexLocation.Name = "lblIndexLocation";
            this.lblIndexLocation.Size = new System.Drawing.Size(103, 17);
            this.lblIndexLocation.TabIndex = 4;
            this.lblIndexLocation.Text = "Index Location:";
            // 
            // gbFileTypes
            // 
            this.gbFileTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFileTypes.Controls.Add(this.cmdAddFileType);
            this.gbFileTypes.Controls.Add(this.cmdRemoveFileType);
            this.gbFileTypes.Controls.Add(this.lvFileTypes);
            this.gbFileTypes.Location = new System.Drawing.Point(4, 42);
            this.gbFileTypes.Margin = new System.Windows.Forms.Padding(4);
            this.gbFileTypes.Name = "gbFileTypes";
            this.gbFileTypes.Padding = new System.Windows.Forms.Padding(4);
            this.gbFileTypes.Size = new System.Drawing.Size(499, 230);
            this.gbFileTypes.TabIndex = 3;
            this.gbFileTypes.TabStop = false;
            this.gbFileTypes.Text = "File Types";
            // 
            // cmdAddFileType
            // 
            this.cmdAddFileType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAddFileType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAddFileType.Location = new System.Drawing.Point(248, 192);
            this.cmdAddFileType.Margin = new System.Windows.Forms.Padding(4);
            this.cmdAddFileType.Name = "cmdAddFileType";
            this.cmdAddFileType.Size = new System.Drawing.Size(120, 31);
            this.cmdAddFileType.TabIndex = 7;
            this.cmdAddFileType.Text = "Add...";
            this.cmdAddFileType.UseVisualStyleBackColor = true;
            this.cmdAddFileType.Click += new System.EventHandler(this.cmdAddFileType_Click);
            // 
            // cmdRemoveFileType
            // 
            this.cmdRemoveFileType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRemoveFileType.Enabled = false;
            this.cmdRemoveFileType.Location = new System.Drawing.Point(371, 192);
            this.cmdRemoveFileType.Margin = new System.Windows.Forms.Padding(4);
            this.cmdRemoveFileType.Name = "cmdRemoveFileType";
            this.cmdRemoveFileType.Size = new System.Drawing.Size(120, 31);
            this.cmdRemoveFileType.TabIndex = 6;
            this.cmdRemoveFileType.Text = "Remove";
            this.cmdRemoveFileType.UseVisualStyleBackColor = true;
            this.cmdRemoveFileType.Click += new System.EventHandler(this.cmdRemoveFileType_Click);
            // 
            // lvFileTypes
            // 
            this.lvFileTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFileTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFilter});
            this.lvFileTypes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvFileTypes.HideSelection = false;
            this.lvFileTypes.Location = new System.Drawing.Point(8, 23);
            this.lvFileTypes.Margin = new System.Windows.Forms.Padding(4);
            this.lvFileTypes.Name = "lvFileTypes";
            this.lvFileTypes.Size = new System.Drawing.Size(481, 164);
            this.lvFileTypes.SmallImageList = this.ilFileTypes;
            this.lvFileTypes.TabIndex = 0;
            this.lvFileTypes.UseCompatibleStateImageBehavior = false;
            this.lvFileTypes.View = System.Windows.Forms.View.SmallIcon;
            this.lvFileTypes.SelectedIndexChanged += new System.EventHandler(this.lvFileTypes_SelectedIndexChanged);
            // 
            // colFilter
            // 
            this.colFilter.Text = "Filter";
            // 
            // gbFolders
            // 
            this.gbFolders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFolders.Controls.Add(this.dgFolders);
            this.gbFolders.Controls.Add(this.cmdAddFolder);
            this.gbFolders.Controls.Add(this.cmdRemoveFolder);
            this.gbFolders.Location = new System.Drawing.Point(4, 279);
            this.gbFolders.Margin = new System.Windows.Forms.Padding(4);
            this.gbFolders.Name = "gbFolders";
            this.gbFolders.Padding = new System.Windows.Forms.Padding(4);
            this.gbFolders.Size = new System.Drawing.Size(499, 159);
            this.gbFolders.TabIndex = 2;
            this.gbFolders.TabStop = false;
            this.gbFolders.Text = "Folders";
            // 
            // dgFolders
            // 
            this.dgFolders.AllowUserToAddRows = false;
            this.dgFolders.AllowUserToDeleteRows = false;
            this.dgFolders.AllowUserToResizeColumns = false;
            this.dgFolders.AllowUserToResizeRows = false;
            this.dgFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgFolders.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgFolders.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgFolders.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgFolders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgFolders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFolderPath,
            this.colExcludeFolder});
            this.dgFolders.GridColor = System.Drawing.SystemColors.Window;
            this.dgFolders.Location = new System.Drawing.Point(8, 22);
            this.dgFolders.Name = "dgFolders";
            this.dgFolders.RowHeadersVisible = false;
            this.dgFolders.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dgFolders.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgFolders.RowTemplate.Height = 24;
            this.dgFolders.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgFolders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgFolders.ShowCellErrors = false;
            this.dgFolders.ShowEditingIcon = false;
            this.dgFolders.ShowRowErrors = false;
            this.dgFolders.Size = new System.Drawing.Size(481, 92);
            this.dgFolders.StandardTab = true;
            this.dgFolders.TabIndex = 6;
            this.dgFolders.VirtualMode = true;
            this.dgFolders.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgFolders_CellValueNeeded);
            this.dgFolders.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgFolders_CellValuePushed);
            // 
            // colFolderPath
            // 
            this.colFolderPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.colFolderPath.DefaultCellStyle = dataGridViewCellStyle1;
            this.colFolderPath.HeaderText = "Folder Path";
            this.colFolderPath.Name = "colFolderPath";
            // 
            // colExcludeFolder
            // 
            this.colExcludeFolder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colExcludeFolder.HeaderText = "Exclude";
            this.colExcludeFolder.Name = "colExcludeFolder";
            this.colExcludeFolder.Width = 63;
            // 
            // cmdAddFolder
            // 
            this.cmdAddFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAddFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAddFolder.Location = new System.Drawing.Point(248, 121);
            this.cmdAddFolder.Margin = new System.Windows.Forms.Padding(4);
            this.cmdAddFolder.Name = "cmdAddFolder";
            this.cmdAddFolder.Size = new System.Drawing.Size(120, 31);
            this.cmdAddFolder.TabIndex = 5;
            this.cmdAddFolder.Text = "Add...";
            this.cmdAddFolder.UseVisualStyleBackColor = true;
            this.cmdAddFolder.Click += new System.EventHandler(this.cmdAddFolder_Click);
            // 
            // cmdRemoveFolder
            // 
            this.cmdRemoveFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRemoveFolder.Enabled = false;
            this.cmdRemoveFolder.Location = new System.Drawing.Point(371, 121);
            this.cmdRemoveFolder.Margin = new System.Windows.Forms.Padding(4);
            this.cmdRemoveFolder.Name = "cmdRemoveFolder";
            this.cmdRemoveFolder.Size = new System.Drawing.Size(120, 31);
            this.cmdRemoveFolder.TabIndex = 4;
            this.cmdRemoveFolder.Text = "Remove";
            this.cmdRemoveFolder.UseVisualStyleBackColor = true;
            this.cmdRemoveFolder.Click += new System.EventHandler(this.cmdRemoveFolder_Click);
            // 
            // tabUI
            // 
            this.tabUI.Controls.Add(this.gbHotkeys);
            this.tabUI.Controls.Add(this.gbEditor);
            this.tabUI.Location = new System.Drawing.Point(4, 25);
            this.tabUI.Margin = new System.Windows.Forms.Padding(4);
            this.tabUI.Name = "tabUI";
            this.tabUI.Padding = new System.Windows.Forms.Padding(4);
            this.tabUI.Size = new System.Drawing.Size(509, 446);
            this.tabUI.TabIndex = 1;
            this.tabUI.Text = "UI";
            this.tabUI.UseVisualStyleBackColor = true;
            // 
            // gbHotkeys
            // 
            this.gbHotkeys.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbHotkeys.Controls.Add(this.lblSearchFiles);
            this.gbHotkeys.Controls.Add(this.hkSearchFiles);
            this.gbHotkeys.Location = new System.Drawing.Point(4, 107);
            this.gbHotkeys.Margin = new System.Windows.Forms.Padding(4);
            this.gbHotkeys.Name = "gbHotkeys";
            this.gbHotkeys.Padding = new System.Windows.Forms.Padding(4);
            this.gbHotkeys.Size = new System.Drawing.Size(499, 57);
            this.gbHotkeys.TabIndex = 6;
            this.gbHotkeys.TabStop = false;
            this.gbHotkeys.Text = "Hotkeys";
            // 
            // lblSearchFiles
            // 
            this.lblSearchFiles.AutoSize = true;
            this.lblSearchFiles.Location = new System.Drawing.Point(12, 27);
            this.lblSearchFiles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSearchFiles.Name = "lblSearchFiles";
            this.lblSearchFiles.Size = new System.Drawing.Size(57, 17);
            this.lblSearchFiles.TabIndex = 11;
            this.lblSearchFiles.Text = "Search:";
            // 
            // hkSearchFiles
            // 
            this.hkSearchFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hkSearchFiles.Hotkey = System.Windows.Forms.Keys.None;
            this.hkSearchFiles.HotkeyModifiers = System.Windows.Forms.Keys.None;
            this.hkSearchFiles.Location = new System.Drawing.Point(115, 23);
            this.hkSearchFiles.Margin = new System.Windows.Forms.Padding(4);
            this.hkSearchFiles.Name = "hkSearchFiles";
            this.hkSearchFiles.Size = new System.Drawing.Size(375, 22);
            this.hkSearchFiles.TabIndex = 10;
            this.hkSearchFiles.Text = "None";
            // 
            // gbEditor
            // 
            this.gbEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbEditor.Controls.Add(this.cmdBrowseForEditor);
            this.gbEditor.Controls.Add(this.txtEditorLocation);
            this.gbEditor.Controls.Add(this.label1);
            this.gbEditor.Controls.Add(this.cbTextEditor);
            this.gbEditor.Location = new System.Drawing.Point(4, 10);
            this.gbEditor.Margin = new System.Windows.Forms.Padding(4);
            this.gbEditor.Name = "gbEditor";
            this.gbEditor.Padding = new System.Windows.Forms.Padding(4);
            this.gbEditor.Size = new System.Drawing.Size(499, 90);
            this.gbEditor.TabIndex = 5;
            this.gbEditor.TabStop = false;
            this.gbEditor.Text = "Text Editor";
            // 
            // cmdBrowseForEditor
            // 
            this.cmdBrowseForEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowseForEditor.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdBrowseForEditor.Location = new System.Drawing.Point(444, 55);
            this.cmdBrowseForEditor.Margin = new System.Windows.Forms.Padding(4);
            this.cmdBrowseForEditor.Name = "cmdBrowseForEditor";
            this.cmdBrowseForEditor.Size = new System.Drawing.Size(47, 27);
            this.cmdBrowseForEditor.TabIndex = 3;
            this.cmdBrowseForEditor.Text = "…";
            this.cmdBrowseForEditor.UseVisualStyleBackColor = true;
            this.cmdBrowseForEditor.Click += new System.EventHandler(this.cmdBrowseForEditor_Click);
            // 
            // txtEditorLocation
            // 
            this.txtEditorLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEditorLocation.Location = new System.Drawing.Point(88, 57);
            this.txtEditorLocation.Margin = new System.Windows.Forms.Padding(4);
            this.txtEditorLocation.Name = "txtEditorLocation";
            this.txtEditorLocation.Size = new System.Drawing.Size(347, 22);
            this.txtEditorLocation.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 60);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Location:";
            // 
            // cbTextEditor
            // 
            this.cbTextEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTextEditor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTextEditor.FormattingEnabled = true;
            this.cbTextEditor.Location = new System.Drawing.Point(8, 23);
            this.cbTextEditor.Margin = new System.Windows.Forms.Padding(4);
            this.cbTextEditor.Name = "cbTextEditor";
            this.cbTextEditor.Size = new System.Drawing.Size(481, 24);
            this.cbTextEditor.TabIndex = 0;
            this.cbTextEditor.SelectedIndexChanged += new System.EventHandler(this.cbTextEditor_SelectedIndexChanged);
            // 
            // ConfigurationDialog
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(525, 518);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.tcTabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NDexer Configuration";
            this.tcTabs.ResumeLayout(false);
            this.tabIndex.ResumeLayout(false);
            this.tabIndex.PerformLayout();
            this.gbFileTypes.ResumeLayout(false);
            this.gbFolders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgFolders)).EndInit();
            this.tabUI.ResumeLayout(false);
            this.gbHotkeys.ResumeLayout(false);
            this.gbHotkeys.PerformLayout();
            this.gbEditor.ResumeLayout(false);
            this.gbEditor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.ImageList ilFileTypes;
        private System.Windows.Forms.TabControl tcTabs;
        private System.Windows.Forms.TabPage tabIndex;
        private System.Windows.Forms.GroupBox gbFileTypes;
        private System.Windows.Forms.Button cmdAddFileType;
        private System.Windows.Forms.Button cmdRemoveFileType;
        private System.Windows.Forms.ListView lvFileTypes;
        private System.Windows.Forms.ColumnHeader colFilter;
        private System.Windows.Forms.GroupBox gbFolders;
        private System.Windows.Forms.Button cmdAddFolder;
        private System.Windows.Forms.Button cmdRemoveFolder;
        private System.Windows.Forms.TabPage tabUI;
        private System.Windows.Forms.GroupBox gbEditor;
        private System.Windows.Forms.Button cmdBrowseForEditor;
        private System.Windows.Forms.TextBox txtEditorLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbTextEditor;
        private System.Windows.Forms.TextBox txtIndexLocation;
        private System.Windows.Forms.Label lblIndexLocation;
        private System.Windows.Forms.GroupBox gbHotkeys;
        private System.Windows.Forms.Label lblSearchFiles;
        private exscape.HotkeyControl hkSearchFiles;
        private System.Windows.Forms.DataGridView dgFolders;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFolderPath;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colExcludeFolder;
    }
#endif
}