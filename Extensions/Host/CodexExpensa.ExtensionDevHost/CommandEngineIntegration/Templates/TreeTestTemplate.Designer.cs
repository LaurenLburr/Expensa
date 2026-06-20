namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Templates
{
    partial class TreeTestTemplate
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
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            tree = new TreeView();
            gridData = new Krypton.Toolkit.Suite.Extended.TreeGridView.KryptonTreeGridView();
            panelLabel = new Panel();
            labelAdd_in_Name = new Label();
            textNotes = new TextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridData).BeginInit();
            panelLabel.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            splitContainer1.Panel1.Controls.Add(panelLabel);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(textNotes);
            splitContainer1.Size = new Size(1049, 544);
            splitContainer1.SplitterDistance = 356;
            splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 67);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(tree);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(gridData);
            splitContainer2.Size = new Size(1049, 289);
            splitContainer2.SplitterDistance = 435;
            splitContainer2.TabIndex = 0;
            // 
            // tree
            // 
            tree.Dock = DockStyle.Fill;
            tree.Location = new Point(0, 0);
            tree.Name = "tree";
            tree.Size = new Size(435, 289);
            tree.TabIndex = 0;
            // 
            // gridData
            // 
            gridData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridData.Dock = DockStyle.Fill;
            gridData.Location = new Point(0, 0);
            gridData.Name = "gridData";
            gridData.Size = new Size(610, 289);
            gridData.TabIndex = 0;
            // 
            // panelLabel
            // 
            panelLabel.Controls.Add(labelAdd_in_Name);
            panelLabel.Dock = DockStyle.Top;
            panelLabel.Location = new Point(0, 0);
            panelLabel.Name = "panelLabel";
            panelLabel.Size = new Size(1049, 67);
            panelLabel.TabIndex = 1;
            // 
            // labelAdd_in_Name
            // 
            labelAdd_in_Name.AutoSize = true;
            labelAdd_in_Name.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelAdd_in_Name.Location = new Point(15, 13);
            labelAdd_in_Name.Name = "labelAdd_in_Name";
            labelAdd_in_Name.Size = new Size(98, 21);
            labelAdd_in_Name.TabIndex = 1;
            labelAdd_in_Name.Text = "Page Name";
            // 
            // textNotes
            // 
            textNotes.BackColor = SystemColors.Control;
            textNotes.Dock = DockStyle.Fill;
            textNotes.Location = new Point(0, 0);
            textNotes.Multiline = true;
            textNotes.Name = "textNotes";
            textNotes.Size = new Size(1049, 184);
            textNotes.TabIndex = 0;
            // 
            // TreeTestTemplate
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1049, 544);
            Controls.Add(splitContainer1);
            Name = "TreeTestTemplate";
            Text = "Tree Test Template";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridData).EndInit();
            panelLabel.ResumeLayout(false);
            panelLabel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        protected SplitContainer splitContainer1;
        protected SplitContainer splitContainer2;
        protected TreeView tree;
        protected Krypton.Toolkit.Suite.Extended.TreeGridView.KryptonTreeGridView gridData;
        protected TextBox textNotes;
        private Panel panelLabel;
        protected Label labelAdd_in_Name;
    }
}
