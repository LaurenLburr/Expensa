namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

partial class WebsitesDatabasePanelForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayoutPanel;
    private GroupBox databaseGroupBox;
    private TableLayoutPanel databaseLayoutPanel;
    private Label databaseLabel;
    private LinkLabel databaseNameLinkLabel;
    private TextBox databasePathTextBox;
    private LinkLabel copyFromDevTemplateLinkLabel;
    private LinkLabel copyFromSandboxLinkLabel;
    private Label copiedDatabaseLabel;
    private TextBox copiedDatabasePathTextBox;
    private LinkLabel openCopiedDatabaseFolderLinkLabel;
    private Button activateSelectedDatabaseButton;
    private ListView copiedDatabasesListView;
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
    private ToolStrip diagnosticsToolStrip;
    private ToolStripButton toggleControlNamesButton;

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
        components = new System.ComponentModel.Container();
        rootLayoutPanel = new TableLayoutPanel();
        databaseGroupBox = new GroupBox();
        databaseLayoutPanel = new TableLayoutPanel();
        databaseLabel = new Label();
        databaseNameLinkLabel = new LinkLabel();
        databasePathTextBox = new TextBox();
        copyFromDevTemplateLinkLabel = new LinkLabel();
        copyFromSandboxLinkLabel = new LinkLabel();
        copiedDatabaseLabel = new Label();
        copiedDatabasePathTextBox = new TextBox();
        openCopiedDatabaseFolderLinkLabel = new LinkLabel();
        copiedDatabasesListView = new ListView();
        activateSelectedDatabaseButton = new Button();
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
        diagnosticsToolStrip = new ToolStrip();
        toggleControlNamesButton = new ToolStripButton();

        rootLayoutPanel.SuspendLayout();
        databaseGroupBox.SuspendLayout();
        databaseLayoutPanel.SuspendLayout();
        filterLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)maximumRowsNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();

        rootLayoutPanel.ColumnCount = 1;
        rootLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayoutPanel.Controls.Add(databaseGroupBox, 0, 0);
        rootLayoutPanel.Controls.Add(diagnosticsToolStrip, 0, 0);
        rootLayoutPanel.Controls.Add(filterLayoutPanel, 0, 1);
        rootLayoutPanel.Controls.Add(statusLabel, 0, 2);
        rootLayoutPanel.Controls.Add(splitContainer, 0, 3);
        rootLayoutPanel.Controls.Add(buttonPanel, 0, 4);
        rootLayoutPanel.Dock = DockStyle.Fill;
        rootLayoutPanel.RowCount = 5;
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 190F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

        diagnosticsToolStrip.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        diagnosticsToolStrip.Dock = DockStyle.None;
        diagnosticsToolStrip.GripStyle = ToolStripGripStyle.Hidden;
        diagnosticsToolStrip.RenderMode = ToolStripRenderMode.System;
        diagnosticsToolStrip.Items.AddRange(new ToolStripItem[]
        {
            toggleControlNamesButton
        });
        diagnosticsToolStrip.Location = new Point(1160, 0);
        diagnosticsToolStrip.Size = new Size(32, 25);
        diagnosticsToolStrip.TabIndex = 99;
        toggleControlNamesButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        toggleControlNamesButton.Text = "?";
        toggleControlNamesButton.ToolTipText = "Show control names";
        toggleControlNamesButton.Click += toggleControlNamesButton_Click;

        databaseGroupBox.Dock = DockStyle.Fill;
        databaseGroupBox.Text = "Add-in Database";
        databaseGroupBox.Controls.Add(databaseLayoutPanel);

        databaseLayoutPanel.ColumnCount = 4;
        databaseLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        databaseLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
        databaseLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        databaseLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));
        databaseLayoutPanel.Controls.Add(databaseLabel, 0, 0);
        databaseLayoutPanel.Controls.Add(databaseNameLinkLabel, 1, 0);
        databaseLayoutPanel.Controls.Add(databasePathTextBox, 2, 0);
        databaseLayoutPanel.Controls.Add(copyFromDevTemplateLinkLabel, 3, 0);
        databaseLayoutPanel.Controls.Add(copyFromSandboxLinkLabel, 3, 1);
        databaseLayoutPanel.Controls.Add(copiedDatabaseLabel, 0, 2);
        databaseLayoutPanel.Controls.Add(copiedDatabasePathTextBox, 1, 2);
        databaseLayoutPanel.Controls.Add(openCopiedDatabaseFolderLinkLabel, 3, 2);
        databaseLayoutPanel.Controls.Add(copiedDatabasesListView, 0, 3);
        databaseLayoutPanel.Controls.Add(activateSelectedDatabaseButton, 3, 2);
        databaseLayoutPanel.Dock = DockStyle.Fill;
        databaseLayoutPanel.RowCount = 4;
        databaseLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        databaseLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        databaseLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        databaseLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        databaseLayoutPanel.SetColumnSpan(copiedDatabasePathTextBox, 2);
        databaseLayoutPanel.SetColumnSpan(copiedDatabasesListView, 4);

        databaseLabel.Dock = DockStyle.Fill;
        databaseLabel.Text = "Database:";
        databaseLabel.TextAlign = ContentAlignment.MiddleLeft;

        databaseNameLinkLabel.AutoEllipsis = true;
        databaseNameLinkLabel.Dock = DockStyle.Fill;
        databaseNameLinkLabel.MaximumSize = new Size(0, 22);
        databaseNameLinkLabel.Text = "websitesaddin.db";
        databaseNameLinkLabel.TextAlign = ContentAlignment.MiddleLeft;
        databaseNameLinkLabel.LinkClicked += databaseNameLinkLabel_LinkClicked;

        databasePathTextBox.Dock = DockStyle.Fill;
        databasePathTextBox.ReadOnly = true;

        copyFromDevTemplateLinkLabel.Dock = DockStyle.Fill;
        copyFromDevTemplateLinkLabel.Text = "Copy new from dev template";
        copyFromDevTemplateLinkLabel.TextAlign = ContentAlignment.MiddleLeft;
        copyFromDevTemplateLinkLabel.LinkClicked += copyFromDevTemplateLinkLabel_LinkClicked;

        copyFromSandboxLinkLabel.Dock = DockStyle.Fill;
        copyFromSandboxLinkLabel.Text = "Copy new from Sandbox DB";
        copyFromSandboxLinkLabel.TextAlign = ContentAlignment.MiddleLeft;
        copyFromSandboxLinkLabel.LinkClicked += copyFromSandboxLinkLabel_LinkClicked;

        copiedDatabaseLabel.Dock = DockStyle.Fill;
        copiedDatabaseLabel.Text = "Copied DB:";
        copiedDatabaseLabel.TextAlign = ContentAlignment.MiddleLeft;

        copiedDatabasePathTextBox.Dock = DockStyle.Fill;
        copiedDatabasePathTextBox.ReadOnly = true;

        openCopiedDatabaseFolderLinkLabel.Dock = DockStyle.Fill;
        openCopiedDatabaseFolderLinkLabel.Text = "Open copied DB folder";

        activateSelectedDatabaseButton.Dock = DockStyle.Right;
        activateSelectedDatabaseButton.Width = 120;
        activateSelectedDatabaseButton.Text = "Make Active";
        activateSelectedDatabaseButton.Click += activateSelectedDatabaseButton_Click;
        openCopiedDatabaseFolderLinkLabel.TextAlign = ContentAlignment.MiddleLeft;
        openCopiedDatabaseFolderLinkLabel.LinkClicked += openCopiedDatabaseFolderLinkLabel_LinkClicked;

        copiedDatabasesListView.Dock = DockStyle.Fill;
        copiedDatabasesListView.FullRowSelect = true;
        copiedDatabasesListView.HideSelection = false;
        copiedDatabasesListView.MultiSelect = false;
        copiedDatabasesListView.UseCompatibleStateImageBehavior = false;
        copiedDatabasesListView.View = View.Details;
        copiedDatabasesListView.Columns.Add("Database", 260);
        copiedDatabasesListView.Columns.Add("Source", 160);
        copiedDatabasesListView.Columns.Add("Created", 160);
        copiedDatabasesListView.Columns.Add("Path", 520);
        copiedDatabasesListView.SelectedIndexChanged += copiedDatabasesListView_SelectedIndexChanged;

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
        filterLayoutPanel.RowCount = 1;
        filterLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        searchLabel.Dock = DockStyle.Fill;
        searchLabel.Text = "Search:";
        searchLabel.TextAlign = ContentAlignment.MiddleLeft;

        searchTextBox.Dock = DockStyle.Fill;

        includeDisabledCheckBox.Dock = DockStyle.Fill;
        includeDisabledCheckBox.Text = "Include disabled";
        includeDisabledCheckBox.TextAlign = ContentAlignment.MiddleLeft;

        maximumRowsLabel.Dock = DockStyle.Fill;
        maximumRowsLabel.Text = "Max rows:";
        maximumRowsLabel.TextAlign = ContentAlignment.MiddleLeft;

        maximumRowsNumericUpDown.Dock = DockStyle.Left;
        maximumRowsNumericUpDown.Minimum = 1;
        maximumRowsNumericUpDown.Maximum = 100000;
        maximumRowsNumericUpDown.Value = 500;
        maximumRowsNumericUpDown.Width = 90;

        refreshButton.Dock = DockStyle.Fill;
        refreshButton.Text = "Refresh";
        refreshButton.Click += refreshButton_Click;

        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Text = "Ready.";
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;

        splitContainer.Dock = DockStyle.Fill;
        splitContainer.SplitterDistance = 540;
        splitContainer.Panel1.Controls.Add(websitesListView);
        splitContainer.Panel2.Controls.Add(detailsTextBox);

        websitesListView.Dock = DockStyle.Fill;
        websitesListView.FullRowSelect = true;
        websitesListView.HideSelection = false;
        websitesListView.MultiSelect = false;
        websitesListView.UseCompatibleStateImageBehavior = false;
        websitesListView.View = View.Details;
        websitesListView.Columns.Add("Name", 180);
        websitesListView.Columns.Add("Category", 120);
        websitesListView.Columns.Add("Url", 260);
        websitesListView.Columns.Add("Enabled", 80);
        websitesListView.Columns.Add("NodeId", 160);
        websitesListView.SelectedIndexChanged += websitesListView_SelectedIndexChanged;
        websitesListView.DoubleClick += websitesListView_DoubleClick;

        detailsTextBox.Dock = DockStyle.Fill;
        detailsTextBox.Multiline = true;
        detailsTextBox.ReadOnly = true;
        detailsTextBox.ScrollBars = ScrollBars.Both;
        detailsTextBox.WordWrap = false;

        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonPanel.Controls.Add(closeButton);

        closeButton.Text = "Close";
        closeButton.Width = 100;
        closeButton.Click += closeButton_Click;

        AcceptButton = refreshButton;
        CancelButton = closeButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 820);
        Controls.Add(rootLayoutPanel);
        MinimizeBox = false;
        MaximizeBox = false;
        Name = "WebsitesDatabasePanelForm";
        Padding = new Padding(8);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Websites Database";

        rootLayoutPanel.ResumeLayout(false);
        databaseGroupBox.ResumeLayout(false);
        databaseLayoutPanel.ResumeLayout(false);
        databaseLayoutPanel.PerformLayout();
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
