namespace Ndexer {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationDialog));
            this.gbFileTypes = new System.Windows.Forms.GroupBox();
            this.cmdAddFileType = new System.Windows.Forms.Button();
            this.cmdRemoveFileType = new System.Windows.Forms.Button();
            this.lvFileTypes = new System.Windows.Forms.ListView();
            this.colFilter = new System.Windows.Forms.ColumnHeader();
            this.ilFileTypes = new System.Windows.Forms.ImageList(this.components);
            this.gbFolders = new System.Windows.Forms.GroupBox();
            this.cmdAddFolder = new System.Windows.Forms.Button();
            this.cmdRemoveFolder = new System.Windows.Forms.Button();
            this.lvFolders = new System.Windows.Forms.ListView();
            this.ilFolders = new System.Windows.Forms.ImageList(this.components);
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.gbEditor = new System.Windows.Forms.GroupBox();
            this.cmdBrowseForEditor = new System.Windows.Forms.Button();
            this.txtEditorLocation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbTextEditor = new System.Windows.Forms.ComboBox();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.gbFileTypes.SuspendLayout();
            this.gbFolders.SuspendLayout();
            this.gbEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbFileTypes
            // 
            this.gbFileTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFileTypes.Controls.Add(this.cmdAddFileType);
            this.gbFileTypes.Controls.Add(this.cmdRemoveFileType);
            this.gbFileTypes.Controls.Add(this.lvFileTypes);
            this.gbFileTypes.Location = new System.Drawing.Point(3, 3);
            this.gbFileTypes.Name = "gbFileTypes";
            this.gbFileTypes.Size = new System.Drawing.Size(388, 177);
            this.gbFileTypes.TabIndex = 0;
            this.gbFileTypes.TabStop = false;
            this.gbFileTypes.Text = "File Types";
            // 
            // cmdAddFileType
            // 
            this.cmdAddFileType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAddFileType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAddFileType.Location = new System.Drawing.Point(200, 146);
            this.cmdAddFileType.Name = "cmdAddFileType";
            this.cmdAddFileType.Size = new System.Drawing.Size(90, 25);
            this.cmdAddFileType.TabIndex = 7;
            this.cmdAddFileType.Text = "Add...";
            this.cmdAddFileType.UseVisualStyleBackColor = true;
            this.cmdAddFileType.Click += new System.EventHandler(this.cmdAddFileType_Click);
            // 
            // cmdRemoveFileType
            // 
            this.cmdRemoveFileType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRemoveFileType.Enabled = false;
            this.cmdRemoveFileType.Location = new System.Drawing.Point(292, 146);
            this.cmdRemoveFileType.Name = "cmdRemoveFileType";
            this.cmdRemoveFileType.Size = new System.Drawing.Size(90, 25);
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
            this.lvFileTypes.Location = new System.Drawing.Point(6, 19);
            this.lvFileTypes.Name = "lvFileTypes";
            this.lvFileTypes.Size = new System.Drawing.Size(376, 124);
            this.lvFileTypes.SmallImageList = this.ilFileTypes;
            this.lvFileTypes.TabIndex = 0;
            this.lvFileTypes.UseCompatibleStateImageBehavior = false;
            this.lvFileTypes.View = System.Windows.Forms.View.Details;
            this.lvFileTypes.SelectedIndexChanged += new System.EventHandler(this.lvFileTypes_SelectedIndexChanged);
            // 
            // colFilter
            // 
            this.colFilter.Text = "Filter";
            // 
            // ilFileTypes
            // 
            this.ilFileTypes.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilFileTypes.ImageSize = new System.Drawing.Size(16, 16);
            this.ilFileTypes.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // gbFolders
            // 
            this.gbFolders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFolders.Controls.Add(this.cmdAddFolder);
            this.gbFolders.Controls.Add(this.cmdRemoveFolder);
            this.gbFolders.Controls.Add(this.lvFolders);
            this.gbFolders.Location = new System.Drawing.Point(3, 186);
            this.gbFolders.Name = "gbFolders";
            this.gbFolders.Size = new System.Drawing.Size(388, 119);
            this.gbFolders.TabIndex = 1;
            this.gbFolders.TabStop = false;
            this.gbFolders.Text = "Folders";
            // 
            // cmdAddFolder
            // 
            this.cmdAddFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAddFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAddFolder.Location = new System.Drawing.Point(200, 88);
            this.cmdAddFolder.Name = "cmdAddFolder";
            this.cmdAddFolder.Size = new System.Drawing.Size(90, 25);
            this.cmdAddFolder.TabIndex = 5;
            this.cmdAddFolder.Text = "Add...";
            this.cmdAddFolder.UseVisualStyleBackColor = true;
            this.cmdAddFolder.Click += new System.EventHandler(this.cmdAddFolder_Click);
            // 
            // cmdRemoveFolder
            // 
            this.cmdRemoveFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRemoveFolder.Enabled = false;
            this.cmdRemoveFolder.Location = new System.Drawing.Point(292, 88);
            this.cmdRemoveFolder.Name = "cmdRemoveFolder";
            this.cmdRemoveFolder.Size = new System.Drawing.Size(90, 25);
            this.cmdRemoveFolder.TabIndex = 4;
            this.cmdRemoveFolder.Text = "Remove";
            this.cmdRemoveFolder.UseVisualStyleBackColor = true;
            this.cmdRemoveFolder.Click += new System.EventHandler(this.cmdRemoveFolder_Click);
            // 
            // lvFolders
            // 
            this.lvFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFolders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            this.lvFolders.FullRowSelect = true;
            this.lvFolders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvFolders.HideSelection = false;
            this.lvFolders.Location = new System.Drawing.Point(6, 19);
            this.lvFolders.Name = "lvFolders";
            this.lvFolders.Size = new System.Drawing.Size(376, 66);
            this.lvFolders.SmallImageList = this.ilFolders;
            this.lvFolders.TabIndex = 0;
            this.lvFolders.UseCompatibleStateImageBehavior = false;
            this.lvFolders.View = System.Windows.Forms.View.Details;
            this.lvFolders.SelectedIndexChanged += new System.EventHandler(this.lvFolders_SelectedIndexChanged);
            // 
            // ilFolders
            // 
            this.ilFolders.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilFolders.ImageSize = new System.Drawing.Size(16, 16);
            this.ilFolders.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(301, 388);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(90, 25);
            this.cmdCancel.TabIndex = 2;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOK.Location = new System.Drawing.Point(209, 388);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(90, 25);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // gbEditor
            // 
            this.gbEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbEditor.Controls.Add(this.cmdBrowseForEditor);
            this.gbEditor.Controls.Add(this.txtEditorLocation);
            this.gbEditor.Controls.Add(this.label1);
            this.gbEditor.Controls.Add(this.cbTextEditor);
            this.gbEditor.Location = new System.Drawing.Point(3, 311);
            this.gbEditor.Name = "gbEditor";
            this.gbEditor.Size = new System.Drawing.Size(388, 73);
            this.gbEditor.TabIndex = 4;
            this.gbEditor.TabStop = false;
            this.gbEditor.Text = "Text Editor";
            // 
            // cmdBrowseForEditor
            // 
            this.cmdBrowseForEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowseForEditor.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdBrowseForEditor.Location = new System.Drawing.Point(347, 45);
            this.cmdBrowseForEditor.Name = "cmdBrowseForEditor";
            this.cmdBrowseForEditor.Size = new System.Drawing.Size(35, 22);
            this.cmdBrowseForEditor.TabIndex = 3;
            this.cmdBrowseForEditor.Text = "…";
            this.cmdBrowseForEditor.UseVisualStyleBackColor = true;
            this.cmdBrowseForEditor.Click += new System.EventHandler(this.cmdBrowseForEditor_Click);
            // 
            // txtEditorLocation
            // 
            this.txtEditorLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEditorLocation.Location = new System.Drawing.Point(66, 46);
            this.txtEditorLocation.Name = "txtEditorLocation";
            this.txtEditorLocation.Size = new System.Drawing.Size(275, 20);
            this.txtEditorLocation.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Location:";
            // 
            // cbTextEditor
            // 
            this.cbTextEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTextEditor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTextEditor.FormattingEnabled = true;
            this.cbTextEditor.Location = new System.Drawing.Point(6, 19);
            this.cbTextEditor.Name = "cbTextEditor";
            this.cbTextEditor.Size = new System.Drawing.Size(376, 21);
            this.cbTextEditor.TabIndex = 0;
            // 
            // colName
            // 
            this.colName.Text = "Folder";
            this.colName.Width = 372;
            // 
            // ConfigurationDialog
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(394, 416);
            this.Controls.Add(this.gbEditor);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.gbFolders);
            this.Controls.Add(this.gbFileTypes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NDexer Configuration";
            this.gbFileTypes.ResumeLayout(false);
            this.gbFolders.ResumeLayout(false);
            this.gbEditor.ResumeLayout(false);
            this.gbEditor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbFileTypes;
        private System.Windows.Forms.GroupBox gbFolders;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.GroupBox gbEditor;
        private System.Windows.Forms.ComboBox cbTextEditor;
        private System.Windows.Forms.Button cmdBrowseForEditor;
        private System.Windows.Forms.TextBox txtEditorLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdAddFolder;
        private System.Windows.Forms.Button cmdRemoveFolder;
        private System.Windows.Forms.ListView lvFolders;
        private System.Windows.Forms.ImageList ilFolders;
        private System.Windows.Forms.Button cmdAddFileType;
        private System.Windows.Forms.Button cmdRemoveFileType;
        private System.Windows.Forms.ListView lvFileTypes;
        private System.Windows.Forms.ImageList ilFileTypes;
        private System.Windows.Forms.ColumnHeader colFilter;
        private System.Windows.Forms.ColumnHeader colName;
    }
}