namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;

partial class ExpensaAddinLoaderTestForm
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel rootTableLayoutPanel;
    private TableLayoutPanel topTableLayoutPanel;
    private FlowLayoutPanel commandFlowLayoutPanel;
    private SplitContainer mainSplitContainer;
    private ComboBox addinComboBox;
    private TextBox databasePathTextBox;
    private TextBox searchTextBox;
    private CheckBox includeInactiveCheckBox;
    private CheckBox expandAllCheckBox;
    private NumericUpDown maximumRowsNumericUpDown;
    private Button browseDatabaseButton;
    private Button loadSelectedButton;
    private Button loadAllButton;
    private Label addinLabel;
    private Label databaseLabel;
    private Label searchLabel;
    private Label maximumRowsLabel;
    private Label statusLabel;
    private TreeView treeView;
    private TextBox detailsTextBox;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        rootTableLayoutPanel = new TableLayoutPanel();
        topTableLayoutPanel = new TableLayoutPanel();
        commandFlowLayoutPanel = new FlowLayoutPanel();
        mainSplitContainer = new SplitContainer();
        addinComboBox = new ComboBox();
        databasePathTextBox = new TextBox();
        searchTextBox = new TextBox();
        includeInactiveCheckBox = new CheckBox();
        expandAllCheckBox = new CheckBox();
        maximumRowsNumericUpDown = new NumericUpDown();
        browseDatabaseButton = new Button();
        loadSelectedButton = new Button();
        loadAllButton = new Button();
        addinLabel = new Label();
        databaseLabel = new Label();
        searchLabel = new Label();
        maximumRowsLabel = new Label();
        statusLabel = new Label();
        treeView = new TreeView();
        detailsTextBox = new TextBox();

        ((System.ComponentModel.ISupportInitialize)maximumRowsNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
        mainSplitContainer.Panel1.SuspendLayout();
        mainSplitContainer.Panel2.SuspendLayout();
        mainSplitContainer.SuspendLayout();
        rootTableLayoutPanel.SuspendLayout();
        topTableLayoutPanel.SuspendLayout();
        commandFlowLayoutPanel.SuspendLayout();
        SuspendLayout();

        rootTableLayoutPanel.ColumnCount = 1;
        rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootTableLayoutPanel.Controls.Add(topTableLayoutPanel, 0, 0);
        rootTableLayoutPanel.Controls.Add(commandFlowLayoutPanel, 0, 1);
        rootTableLayoutPanel.Controls.Add(statusLabel, 0, 2);
        rootTableLayoutPanel.Controls.Add(mainSplitContainer, 0, 3);
        rootTableLayoutPanel.Dock = DockStyle.Fill;
        rootTableLayoutPanel.Location = new Point(0, 0);
        rootTableLayoutPanel.Name = "rootTableLayoutPanel";
        rootTableLayoutPanel.Padding = new Padding(8);
        rootTableLayoutPanel.RowCount = 4;
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootTableLayoutPanel.Size = new Size(1200, 780);
        rootTableLayoutPanel.TabIndex = 0;

        topTableLayoutPanel.ColumnCount = 4;
        topTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        topTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
        topTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        topTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        topTableLayoutPanel.Controls.Add(addinLabel, 0, 0);
        topTableLayoutPanel.Controls.Add(addinComboBox, 1, 0);
        topTableLayoutPanel.Controls.Add(browseDatabaseButton, 2, 0);
        topTableLayoutPanel.Controls.Add(databaseLabel, 0, 1);
        topTableLayoutPanel.Controls.Add(databasePathTextBox, 1, 1);
        topTableLayoutPanel.Dock = DockStyle.Fill;
        topTableLayoutPanel.Location = new Point(11, 11);
        topTableLayoutPanel.Name = "topTableLayoutPanel";
        topTableLayoutPanel.RowCount = 2;
        topTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        topTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        topTableLayoutPanel.Size = new Size(1178, 84);
        topTableLayoutPanel.TabIndex = 0;
        topTableLayoutPanel.SetColumnSpan(databasePathTextBox, 3);

        addinLabel.Dock = DockStyle.Fill;
        addinLabel.Location = new Point(3, 0);
        addinLabel.Name = "addinLabel";
        addinLabel.Size = new Size(104, 34);
        addinLabel.TabIndex = 0;
        addinLabel.Text = "Add-in:";
        addinLabel.TextAlign = ContentAlignment.MiddleLeft;

        addinComboBox.Dock = DockStyle.Fill;
        addinComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        addinComboBox.FormattingEnabled = true;
        addinComboBox.Items.AddRange(new object[] { "Websites", "Budgets" });
        addinComboBox.Location = new Point(113, 3);
        addinComboBox.Name = "addinComboBox";
        addinComboBox.Size = new Size(214, 23);
        addinComboBox.TabIndex = 1;

        browseDatabaseButton.Dock = DockStyle.Fill;
        browseDatabaseButton.Location = new Point(333, 3);
        browseDatabaseButton.Name = "browseDatabaseButton";
        browseDatabaseButton.Size = new Size(94, 28);
        browseDatabaseButton.TabIndex = 2;
        browseDatabaseButton.Text = "Browse...";
        browseDatabaseButton.UseVisualStyleBackColor = true;
        browseDatabaseButton.Click += browseDatabaseButton_Click;

        databaseLabel.Dock = DockStyle.Fill;
        databaseLabel.Location = new Point(3, 34);
        databaseLabel.Name = "databaseLabel";
        databaseLabel.Size = new Size(104, 34);
        databaseLabel.TabIndex = 3;
        databaseLabel.Text = "Database:";
        databaseLabel.TextAlign = ContentAlignment.MiddleLeft;

        databasePathTextBox.Dock = DockStyle.Fill;
        databasePathTextBox.Location = new Point(113, 37);
        databasePathTextBox.Name = "databasePathTextBox";
        databasePathTextBox.Size = new Size(1062, 23);
        databasePathTextBox.TabIndex = 4;

        commandFlowLayoutPanel.Controls.Add(searchLabel);
        commandFlowLayoutPanel.Controls.Add(searchTextBox);
        commandFlowLayoutPanel.Controls.Add(includeInactiveCheckBox);
        commandFlowLayoutPanel.Controls.Add(expandAllCheckBox);
        commandFlowLayoutPanel.Controls.Add(maximumRowsLabel);
        commandFlowLayoutPanel.Controls.Add(maximumRowsNumericUpDown);
        commandFlowLayoutPanel.Controls.Add(loadAllButton);
        commandFlowLayoutPanel.Controls.Add(loadSelectedButton);
        commandFlowLayoutPanel.Dock = DockStyle.Fill;
        commandFlowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
        commandFlowLayoutPanel.Location = new Point(11, 101);
        commandFlowLayoutPanel.Name = "commandFlowLayoutPanel";
        commandFlowLayoutPanel.Size = new Size(1178, 34);
        commandFlowLayoutPanel.TabIndex = 1;

        searchLabel.AutoSize = true;
        searchLabel.Location = new Point(3, 8);
        searchLabel.Margin = new Padding(3, 8, 3, 0);
        searchLabel.Name = "searchLabel";
        searchLabel.Size = new Size(45, 15);
        searchLabel.TabIndex = 0;
        searchLabel.Text = "Search:";

        searchTextBox.Location = new Point(54, 3);
        searchTextBox.Name = "searchTextBox";
        searchTextBox.Size = new Size(220, 23);
        searchTextBox.TabIndex = 1;

        includeInactiveCheckBox.AutoSize = true;
        includeInactiveCheckBox.Location = new Point(280, 5);
        includeInactiveCheckBox.Margin = new Padding(3, 5, 3, 3);
        includeInactiveCheckBox.Name = "includeInactiveCheckBox";
        includeInactiveCheckBox.Size = new Size(143, 19);
        includeInactiveCheckBox.TabIndex = 2;
        includeInactiveCheckBox.Text = "Include inactive/closed";
        includeInactiveCheckBox.UseVisualStyleBackColor = true;

        expandAllCheckBox.AutoSize = true;
        expandAllCheckBox.Checked = true;
        expandAllCheckBox.CheckState = CheckState.Checked;
        expandAllCheckBox.Location = new Point(429, 5);
        expandAllCheckBox.Margin = new Padding(3, 5, 3, 3);
        expandAllCheckBox.Name = "expandAllCheckBox";
        expandAllCheckBox.Size = new Size(64, 19);
        expandAllCheckBox.TabIndex = 3;
        expandAllCheckBox.Text = "Expand";
        expandAllCheckBox.UseVisualStyleBackColor = true;

        maximumRowsLabel.AutoSize = true;
        maximumRowsLabel.Location = new Point(499, 8);
        maximumRowsLabel.Margin = new Padding(3, 8, 3, 0);
        maximumRowsLabel.Name = "maximumRowsLabel";
        maximumRowsLabel.Size = new Size(34, 15);
        maximumRowsLabel.TabIndex = 4;
        maximumRowsLabel.Text = "Max:";

        maximumRowsNumericUpDown.Location = new Point(539, 3);
        maximumRowsNumericUpDown.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        maximumRowsNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        maximumRowsNumericUpDown.Name = "maximumRowsNumericUpDown";
        maximumRowsNumericUpDown.Size = new Size(90, 23);
        maximumRowsNumericUpDown.TabIndex = 5;
        maximumRowsNumericUpDown.Value = new decimal(new int[] { 500, 0, 0, 0 });

        loadAllButton.Font = new Font(loadAllButton.Font, FontStyle.Bold);
        loadAllButton.Location = new Point(635, 3);
        loadAllButton.Name = "loadAllButton";
        loadAllButton.Size = new Size(120, 27);
        loadAllButton.TabIndex = 6;
        loadAllButton.Text = "Load All";
        loadAllButton.UseVisualStyleBackColor = true;
        loadAllButton.Click += loadAllButton_Click;

        loadSelectedButton.Location = new Point(761, 3);
        loadSelectedButton.Name = "loadSelectedButton";
        loadSelectedButton.Size = new Size(130, 27);
        loadSelectedButton.TabIndex = 7;
        loadSelectedButton.Text = "Load Selected";
        loadSelectedButton.UseVisualStyleBackColor = true;
        loadSelectedButton.Click += loadSelectedButton_Click;

        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Location = new Point(11, 138);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(1178, 28);
        statusLabel.TabIndex = 2;
        statusLabel.Text = "Ready.";
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;

        mainSplitContainer.Dock = DockStyle.Fill;
        mainSplitContainer.Location = new Point(11, 169);
        mainSplitContainer.Name = "mainSplitContainer";
        mainSplitContainer.Size = new Size(1178, 600);
        mainSplitContainer.SplitterDistance = 430;
        mainSplitContainer.TabIndex = 3;

        mainSplitContainer.Panel1.Controls.Add(treeView);
        mainSplitContainer.Panel2.Controls.Add(detailsTextBox);

        treeView.Dock = DockStyle.Fill;
        treeView.Location = new Point(0, 0);
        treeView.Name = "treeView";
        treeView.Size = new Size(430, 600);
        treeView.TabIndex = 0;
        treeView.AfterSelect += treeView_AfterSelect;

        detailsTextBox.Dock = DockStyle.Fill;
        detailsTextBox.Location = new Point(0, 0);
        detailsTextBox.Multiline = true;
        detailsTextBox.Name = "detailsTextBox";
        detailsTextBox.ReadOnly = true;
        detailsTextBox.ScrollBars = ScrollBars.Both;
        detailsTextBox.Size = new Size(744, 600);
        detailsTextBox.TabIndex = 0;
        detailsTextBox.WordWrap = false;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 780);
        Controls.Add(rootTableLayoutPanel);
        Name = "ExpensaAddinLoaderTestForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Expensa Add-in Loader Tree Test";

        ((System.ComponentModel.ISupportInitialize)maximumRowsNumericUpDown).EndInit();
        mainSplitContainer.Panel1.ResumeLayout(false);
        mainSplitContainer.Panel2.ResumeLayout(false);
        mainSplitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
        mainSplitContainer.ResumeLayout(false);
        rootTableLayoutPanel.ResumeLayout(false);
        topTableLayoutPanel.ResumeLayout(false);
        topTableLayoutPanel.PerformLayout();
        commandFlowLayoutPanel.ResumeLayout(false);
        commandFlowLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }
}
