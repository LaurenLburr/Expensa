namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration
{
    partial class DatabasePanelTemplate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
        diagnosticsToolStrip = new ToolStrip();
            linkUpdate_from_Dev = new LinkLabel();
            linkUpdate_from_Prod = new LinkLabel();
            linkDb_filename = new LinkLabel();
            labelDatabase_file_Name = new Label();
            label1 = new Label();
            labelAdd_in_Name = new Label();
            splitContainer1 = new SplitContainer();
            text_Data_ = new TextBox();
            gridDataView = new DataGridView();
            statusStrip1 = new StatusStrip();
            labelNumRows = new ToolStripStatusLabel();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridDataView).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(linkUpdate_from_Dev);
            panel1.Controls.Add(linkUpdate_from_Prod);
            panel1.Controls.Add(linkDb_filename);
            panel1.Controls.Add(labelDatabase_file_Name);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(labelAdd_in_Name);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 87);
            panel1.TabIndex = 0;
            // 
            // linkUpdate_from_Dev
            // 
            linkUpdate_from_Dev.AutoSize = true;
            linkUpdate_from_Dev.Location = new Point(264, 62);
            linkUpdate_from_Dev.Name = "linkUpdate_from_Dev";
            linkUpdate_from_Dev.Size = new Size(124, 15);
            linkUpdate_from_Dev.TabIndex = 5;
            linkUpdate_from_Dev.TabStop = true;
            linkUpdate_from_Dev.Text = "Update Data from Dev";
            linkUpdate_from_Dev.LinkClicked += linkUpdate_from_Dev_LinkClicked;
            // 
            // linkUpdate_from_Prod
            // 
            linkUpdate_from_Prod.AutoSize = true;
            linkUpdate_from_Prod.Location = new Point(129, 62);
            linkUpdate_from_Prod.Name = "linkUpdate_from_Prod";
            linkUpdate_from_Prod.Size = new Size(129, 15);
            linkUpdate_from_Prod.TabIndex = 4;
            linkUpdate_from_Prod.TabStop = true;
            linkUpdate_from_Prod.Text = "Update Data from Prod";
            linkUpdate_from_Prod.LinkClicked += linkUpdate_from_Prod_LinkClicked;
            // 
            // linkDb_filename
            // 
            linkDb_filename.AutoSize = true;
            linkDb_filename.Location = new Point(129, 36);
            linkDb_filename.Name = "linkDb_filename";
            linkDb_filename.Size = new Size(133, 15);
            linkDb_filename.TabIndex = 3;
            linkDb_filename.TabStop = true;
            linkDb_filename.Text = "Open Database location";
            linkDb_filename.TextAlign = ContentAlignment.TopCenter;
            linkDb_filename.LinkClicked += linkDb_filename_LinkClicked;
            // 
            // labelDatabase_file_Name
            // 
            labelDatabase_file_Name.AutoSize = true;
            labelDatabase_file_Name.Font = new Font("Segoe UI", 9F);
            labelDatabase_file_Name.Location = new Point(129, 36);
            labelDatabase_file_Name.Name = "labelDatabase_file_Name";
            labelDatabase_file_Name.Size = new Size(0, 15);
            labelDatabase_file_Name.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(24, 36);
            label1.Name = "label1";
            label1.Size = new Size(99, 15);
            label1.TabIndex = 1;
            label1.Text = "Payees Database";
            // 
            // labelAdd_in_Name
            // 
            labelAdd_in_Name.AutoSize = true;
            labelAdd_in_Name.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelAdd_in_Name.Location = new Point(22, 4);
            labelAdd_in_Name.Name = "labelAdd_in_Name";
            labelAdd_in_Name.Size = new Size(138, 21);
            labelAdd_in_Name.TabIndex = 0;
            labelAdd_in_Name.Text = "Payees Database";
            // 
            // splitContainer1
            // 
            diagnosticsToolStrip.Dock = DockStyle.Top;
        diagnosticsToolStrip.GripStyle = ToolStripGripStyle.Hidden;
        diagnosticsToolStrip.Name = "diagnosticsToolStrip";
        diagnosticsToolStrip.Size = new Size(900, 25);
        diagnosticsToolStrip.TabIndex = 1;
        diagnosticsToolStrip.Text = "Diagnostics";

        splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 112);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(text_Data_);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(statusStrip1);
            splitContainer1.Panel2.Controls.Add(gridDataView);
            splitContainer1.Size = new Size(800, 363);
            splitContainer1.SplitterDistance = 131;
            splitContainer1.TabIndex = 1;
            // 
            // text_Data_
            // 
            text_Data_.BackColor = SystemColors.Control;
            text_Data_.Dock = DockStyle.Fill;
            text_Data_.Location = new Point(0, 0);
            text_Data_.Multiline = true;
            text_Data_.Name = "text_Data_";
            text_Data_.Size = new Size(800, 131);
            text_Data_.TabIndex = 0;
            // 
            // gridDataView
            // 
            gridDataView.AllowUserToOrderColumns = true;
            gridDataView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridDataView.ReadOnly = true;
            gridDataView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridDataView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridDataView.Dock = DockStyle.Fill;
            gridDataView.Location = new Point(0, 0);
            gridDataView.Name = "gridDataView";
            gridDataView.Size = new Size(800, 228);
            gridDataView.TabIndex = 0;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { labelNumRows });
            statusStrip1.Location = new Point(0, 206);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // labelNumRows
            // 
            labelNumRows.Name = "labelNumRows";
            labelNumRows.Size = new Size(80, 17);
            labelNumRows.Text = "Loaded  rows ";
            // 
            // DatabasePanelTemplate
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Controls.Add(panel1);
            Name = "DatabasePanelTemplate";
            Text = "DatabasePanelTemplate";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridDataView).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        protected Panel panel1;
    protected ToolStrip diagnosticsToolStrip;
        protected Label label1;
        protected Label labelAdd_in_Name;
        protected LinkLabel linkDb_filename;
        protected Label labelDatabase_file_Name;
        protected LinkLabel linkUpdate_from_Prod;
        protected LinkLabel linkUpdate_from_Dev;
        protected SplitContainer splitContainer1;
        protected TextBox text_Data_;
        protected DataGridView gridDataView;
        protected StatusStrip statusStrip1;
        protected ToolStripStatusLabel labelNumRows;
    }
}