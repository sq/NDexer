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
            this.dgFilters = new System.Windows.Forms.DataGridView();
            this.gbFolders = new System.Windows.Forms.GroupBox();
            this.dgFolders = new System.Windows.Forms.DataGridView();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.bsFilters = new System.Windows.Forms.BindingSource(this.components);
            this.bsFolders = new System.Windows.Forms.BindingSource(this.components);
            this.gbEditor = new System.Windows.Forms.GroupBox();
            this.cmdBrowseForEditor = new System.Windows.Forms.Button();
            this.txtEditorLocation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbTextEditor = new System.Windows.Forms.ComboBox();
            this.gbFileTypes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgFilters)).BeginInit();
            this.gbFolders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgFolders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsFilters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsFolders)).BeginInit();
            this.gbEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbFileTypes
            // 
            this.gbFileTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFileTypes.Controls.Add(this.dgFilters);
            this.gbFileTypes.Location = new System.Drawing.Point(3, 3);
            this.gbFileTypes.Name = "gbFileTypes";
            this.gbFileTypes.Size = new System.Drawing.Size(388, 150);
            this.gbFileTypes.TabIndex = 0;
            this.gbFileTypes.TabStop = false;
            this.gbFileTypes.Text = "File Types";
            // 
            // dgFilters
            // 
            this.dgFilters.AllowUserToResizeColumns = false;
            this.dgFilters.AllowUserToResizeRows = false;
            this.dgFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgFilters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgFilters.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgFilters.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgFilters.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgFilters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgFilters.Location = new System.Drawing.Point(6, 19);
            this.dgFilters.MultiSelect = false;
            this.dgFilters.Name = "dgFilters";
            this.dgFilters.RowTemplate.Height = 20;
            this.dgFilters.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgFilters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgFilters.ShowCellErrors = false;
            this.dgFilters.Size = new System.Drawing.Size(376, 125);
            this.dgFilters.TabIndex = 2;
            // 
            // gbFolders
            // 
            this.gbFolders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFolders.Controls.Add(this.dgFolders);
            this.gbFolders.Location = new System.Drawing.Point(3, 156);
            this.gbFolders.Name = "gbFolders";
            this.gbFolders.Size = new System.Drawing.Size(388, 150);
            this.gbFolders.TabIndex = 1;
            this.gbFolders.TabStop = false;
            this.gbFolders.Text = "Folders";
            // 
            // dgFolders
            // 
            this.dgFolders.AllowUserToResizeColumns = false;
            this.dgFolders.AllowUserToResizeRows = false;
            this.dgFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgFolders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgFolders.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgFolders.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgFolders.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgFolders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgFolders.Location = new System.Drawing.Point(6, 19);
            this.dgFolders.MultiSelect = false;
            this.dgFolders.Name = "dgFolders";
            this.dgFolders.RowTemplate.Height = 20;
            this.dgFolders.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgFolders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgFolders.ShowCellErrors = false;
            this.dgFolders.Size = new System.Drawing.Size(376, 125);
            this.dgFolders.TabIndex = 2;
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
            // bsFilters
            // 
            this.bsFilters.AllowNew = true;
            // 
            // bsFolders
            // 
            this.bsFolders.AllowNew = true;
            // 
            // gbEditor
            // 
            this.gbEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbEditor.Controls.Add(this.cmdBrowseForEditor);
            this.gbEditor.Controls.Add(this.txtEditorLocation);
            this.gbEditor.Controls.Add(this.label1);
            this.gbEditor.Controls.Add(this.cbTextEditor);
            this.gbEditor.Location = new System.Drawing.Point(3, 312);
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
            this.cbTextEditor.Items.AddRange(new object[] {
            "SciTE"});
            this.cbTextEditor.Location = new System.Drawing.Point(6, 19);
            this.cbTextEditor.Name = "cbTextEditor";
            this.cbTextEditor.Size = new System.Drawing.Size(376, 21);
            this.cbTextEditor.TabIndex = 0;
            // 
            // ConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            ((System.ComponentModel.ISupportInitialize)(this.dgFilters)).EndInit();
            this.gbFolders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgFolders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsFilters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsFolders)).EndInit();
            this.gbEditor.ResumeLayout(false);
            this.gbEditor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbFileTypes;
        private System.Windows.Forms.DataGridView dgFilters;
        private System.Windows.Forms.GroupBox gbFolders;
        private System.Windows.Forms.DataGridView dgFolders;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.BindingSource bsFilters;
        private System.Windows.Forms.BindingSource bsFolders;
        private System.Windows.Forms.GroupBox gbEditor;
        private System.Windows.Forms.ComboBox cbTextEditor;
        private System.Windows.Forms.Button cmdBrowseForEditor;
        private System.Windows.Forms.TextBox txtEditorLocation;
        private System.Windows.Forms.Label label1;
    }
}