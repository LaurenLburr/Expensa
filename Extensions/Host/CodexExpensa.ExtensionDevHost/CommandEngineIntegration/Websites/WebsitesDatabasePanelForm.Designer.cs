namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

partial class WebsitesDatabasePanelForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayoutPanel;
    private TableLayoutPanel filterLayoutPanel;
    private Label searchLabel;
    private TextBox searchTextBox;
    private CheckBox includeDisabledCheckBox;
    private Label maximumRowsLabel;
    private NumericUpDown maximumRowsNumericUpDown;
    private Button refreshButton;
    private Label statusLabel;
    private SplitContainer splitContainer;
    private ListView websitesListView;
    private TextBox detailsTextBox;
    private FlowLayoutPanel buttonPanel;
    private Button closeButton;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        rootLayoutPanel = new TableLayoutPanel();
        filterLayoutPanel = new TableLayoutPanel();
        searchLabel = new Label();
        searchTextBox = new TextBox();
        includeDisabledCheckBox = new CheckBox();
        maximumRowsLabel = new Label();
        maximumRowsNumericUpDown = new NumericUpDown();
        refreshButton = new Button();
        statusLabel = new Label();
        splitContainer = new SplitContainer();
        websitesListView = new ListView();
        detailsTextBox = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        closeButton = new Button();
        rootLayoutPanel.SuspendLayout();
        filterLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)maximumRowsNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // rootLayoutPanel
        // 
        rootLayoutPanel.ColumnCount = 1;
        rootLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayoutPanel.Controls.Add(filterLayoutPanel, 0, 0);
        rootLayoutPanel.Controls.Add(statusLabel, 0, 1);
        rootLayoutPanel.Controls.Add(splitContainer, 0, 2);
        rootLayoutPanel.Controls.Add(buttonPanel, 0, 3);
        rootLayoutPanel.Dock = DockStyle.Fill;
        rootLayoutPanel.Location = new Point(8, 8);
        rootLayoutPanel.Name = "rootLayoutPanel";
        rootLayoutPanel.RowCount = 4;
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        rootLayoutPanel.Size = new Size(1084, 634);
        rootLayoutPanel.TabIndex = 0;
        // 
        // filterLayoutPanel
        // 
        filterLayoutPanel.ColumnCount = 6;
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        filterLayoutPanel.Controls.Add(searchLabel, 0, 0);
        filterLayoutPanel.Controls.Add(searchTextBox, 1, 0);
        filterLayoutPanel.Controls.Add(includeDisabledCheckBox, 2, 0);
        filterLayoutPanel.Controls.Add(maximumRowsLabel, 3, 0);
        filterLayoutPanel.Controls.Add(maximumRowsNumericUpDown, 4, 0);
        filterLayoutPanel.Controls.Add(refreshButton, 5, 0);
        filterLayoutPanel.Dock = DockStyle.Fill;
        filterLayoutPanel.Location = new Point(3, 3);
        filterLayoutPanel.Name = "filterLayoutPanel";
        filterLayoutPanel.RowCount = 1;
        filterLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        filterLayoutPanel.Size = new Size(1078, 36);
        filterLayoutPanel.TabIndex = 0;
        // 
        // searchLabel
        // 
        searchLabel.Dock = DockStyle.Fill;
        searchLabel.Location = new Point(3, 0);
        searchLabel.Name = "searchLabel";
        searchLabel.Size = new Size(74, 36);
        searchLabel.TabIndex = 0;
        searchLabel.Text = "Search:";
        searchLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // searchTextBox
        // 
        searchTextBox.Dock = DockStyle.Fill;
        searchTextBox.Location = new Point(83, 3);
        searchTextBox.Name = "searchTextBox";
        searchTextBox.Size = new Size(542, 23);
        searchTextBox.TabIndex = 1;
        // 
        // includeDisabledCheckBox
        // 
        includeDisabledCheckBox.Dock = DockStyle.Fill;
        includeDisabledCheckBox.Location = new Point(631, 3);
        includeDisabledCheckBox.Name = "includeDisabledCheckBox";
        includeDisabledCheckBox.Size = new Size(134, 30);
        includeDisabledCheckBox.TabIndex = 2;
        includeDisabledCheckBox.Text = "Include disabled";
        // 
        // maximumRowsLabel
        // 
        maximumRowsLabel.Dock = DockStyle.Fill;
        maximumRowsLabel.Location = new Point(771, 0);
        maximumRowsLabel.Name = "maximumRowsLabel";
        maximumRowsLabel.Size = new Size(94, 36);
        maximumRowsLabel.TabIndex = 3;
        maximumRowsLabel.Text = "Max rows:";
        maximumRowsLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // maximumRowsNumericUpDown
        // 
        maximumRowsNumericUpDown.Dock = DockStyle.Left;
        maximumRowsNumericUpDown.Location = new Point(871, 3);
        maximumRowsNumericUpDown.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        maximumRowsNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        maximumRowsNumericUpDown.Name = "maximumRowsNumericUpDown";
        maximumRowsNumericUpDown.Size = new Size(90, 23);
        maximumRowsNumericUpDown.TabIndex = 4;
        maximumRowsNumericUpDown.Value = new decimal(new int[] { 500, 0, 0, 0 });
        // 
        // refreshButton
        // 
        refreshButton.Dock = DockStyle.Fill;
        refreshButton.Location = new Point(981, 3);
        refreshButton.Name = "refreshButton";
        refreshButton.Size = new Size(94, 30);
        refreshButton.TabIndex = 5;
        refreshButton.Text = "Refresh";
        refreshButton.Click += refreshButton_Click;
        // 
        // statusLabel
        // 
        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Location = new Point(3, 42);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(1078, 28);
        statusLabel.TabIndex = 1;
        statusLabel.Text = "Ready.";
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // splitContainer
        // 
        splitContainer.Dock = DockStyle.Fill;
        splitContainer.Location = new Point(3, 73);
        splitContainer.Name = "splitContainer";
        // 
        // splitContainer.Panel1
        // 
        splitContainer.Panel1.Controls.Add(websitesListView);
        // 
        // splitContainer.Panel2
        // 
        splitContainer.Panel2.Controls.Add(detailsTextBox);
        splitContainer.Size = new Size(1078, 514);
        splitContainer.SplitterDistance = 869;
        splitContainer.TabIndex = 2;
        // 
        // websitesListView
        // 
        websitesListView.Dock = DockStyle.Fill;
        websitesListView.FullRowSelect = true;
        websitesListView.Location = new Point(0, 0);
        websitesListView.MultiSelect = false;
        websitesListView.Name = "websitesListView";
        websitesListView.Size = new Size(869, 514);
        websitesListView.TabIndex = 0;
        websitesListView.UseCompatibleStateImageBehavior = false;
        websitesListView.View = View.Details;
        websitesListView.SelectedIndexChanged += websitesListView_SelectedIndexChanged;
        websitesListView.DoubleClick += websitesListView_DoubleClick;
        // 
        // detailsTextBox
        // 
        detailsTextBox.Dock = DockStyle.Fill;
        detailsTextBox.Location = new Point(0, 0);
        detailsTextBox.Multiline = true;
        detailsTextBox.Name = "detailsTextBox";
        detailsTextBox.ReadOnly = true;
        detailsTextBox.ScrollBars = ScrollBars.Both;
        detailsTextBox.Size = new Size(205, 514);
        detailsTextBox.TabIndex = 0;
        detailsTextBox.WordWrap = false;
        // 
        // buttonPanel
        // 
        buttonPanel.Controls.Add(closeButton);
        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonPanel.Location = new Point(3, 593);
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Size = new Size(1078, 38);
        buttonPanel.TabIndex = 3;
        // 
        // closeButton
        // 
        closeButton.Location = new Point(975, 3);
        closeButton.Name = "closeButton";
        closeButton.Size = new Size(100, 23);
        closeButton.TabIndex = 0;
        closeButton.Text = "Close";
        closeButton.Click += closeButton_Click;
        // 
        // WebsitesDatabasePanelForm
        // 
        AcceptButton = refreshButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = closeButton;
        ClientSize = new Size(1100, 650);
        Controls.Add(rootLayoutPanel);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "WebsitesDatabasePanelForm";
        Padding = new Padding(8);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Websites Database";
        rootLayoutPanel.ResumeLayout(false);
        filterLayoutPanel.ResumeLayout(false);
        filterLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)maximumRowsNumericUpDown).EndInit();
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        splitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
