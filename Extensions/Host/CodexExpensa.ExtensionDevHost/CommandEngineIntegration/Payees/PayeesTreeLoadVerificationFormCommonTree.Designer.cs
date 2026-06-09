namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Payees;

partial class PayeesTreeLoadVerificationFormCommonTree
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel rootTableLayoutPanel;
    private TableLayoutPanel topTableLayoutPanel;
    private FlowLayoutPanel optionsFlowLayoutPanel;
    private SplitContainer mainSplitContainer;
    private SplitContainer rightSplitContainer;
    private Label titleLabel;
    private Label databasePathLabel;
    private TextBox databasePathTextBox;
    private Button browseButton;
    private Button loadButton;
    private Label searchLabel;
    private TextBox searchTextBox;
    private CheckBox includeInactiveCheckBox;
    private CheckBox expandAllCheckBox;
    private Label maximumRowsLabel;
    private NumericUpDown maximumRowsNumericUpDown;
    private TreeView payeesTreeView;
    private TextBox resultsTextBox;
    private TextBox selectedNodeTextBox;
    private Label statusLabel;

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
        optionsFlowLayoutPanel = new FlowLayoutPanel();
        mainSplitContainer = new SplitContainer();
        rightSplitContainer = new SplitContainer();
        titleLabel = new Label();
        databasePathLabel = new Label();
        databasePathTextBox = new TextBox();
        browseButton = new Button();
        loadButton = new Button();
        searchLabel = new Label();
        searchTextBox = new TextBox();
        includeInactiveCheckBox = new CheckBox();
        expandAllCheckBox = new CheckBox();
        maximumRowsLabel = new Label();
        maximumRowsNumericUpDown = new NumericUpDown();
        payeesTreeView = new TreeView();
        resultsTextBox = new TextBox();
        selectedNodeTextBox = new TextBox();
        statusLabel = new Label();

        rootTableLayoutPanel.SuspendLayout();
        topTableLayoutPanel.SuspendLayout();
        optionsFlowLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
        mainSplitContainer.Panel1.SuspendLayout();
        mainSplitContainer.Panel2.SuspendLayout();
        mainSplitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)rightSplitContainer).BeginInit();
        rightSplitContainer.Panel1.SuspendLayout();
        rightSplitContainer.Panel2.SuspendLayout();
        rightSplitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)maximumRowsNumericUpDown).BeginInit();
        SuspendLayout();

        rootTableLayoutPanel.ColumnCount = 1;
        rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootTableLayoutPanel.Controls.Add(titleLabel, 0, 0);
        rootTableLayoutPanel.Controls.Add(topTableLayoutPanel, 0, 1);
        rootTableLayoutPanel.Controls.Add(optionsFlowLayoutPanel, 0, 2);
        rootTableLayoutPanel.Controls.Add(mainSplitContainer, 0, 3);
        rootTableLayoutPanel.Controls.Add(statusLabel, 0, 4);
        rootTableLayoutPanel.Dock = DockStyle.Fill;
        rootTableLayoutPanel.Padding = new Padding(8);
        rootTableLayoutPanel.RowCount = 5;
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));

        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font(titleLabel.Font, FontStyle.Bold);
        titleLabel.Text = "Payees Tree Verification";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;

        topTableLayoutPanel.ColumnCount = 4;
        topTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        topTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        topTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        topTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        topTableLayoutPanel.Controls.Add(databasePathLabel, 0, 0);
        topTableLayoutPanel.Controls.Add(databasePathTextBox, 1, 0);
        topTableLayoutPanel.Controls.Add(browseButton, 2, 0);
        topTableLayoutPanel.Controls.Add(loadButton, 3, 0);
        topTableLayoutPanel.Dock = DockStyle.Fill;

        databasePathLabel.Dock = DockStyle.Fill;
        databasePathLabel.Text = "Database:";
        databasePathLabel.TextAlign = ContentAlignment.MiddleLeft;

        databasePathTextBox.Dock = DockStyle.Fill;

        browseButton.Dock = DockStyle.Fill;
        browseButton.Text = "Browse...";
        browseButton.UseVisualStyleBackColor = true;
        browseButton.Click += browseButton_Click;

        loadButton.Dock = DockStyle.Fill;
        loadButton.Text = "Load";
        loadButton.UseVisualStyleBackColor = true;
        loadButton.Click += loadButton_Click;

        optionsFlowLayoutPanel.Controls.Add(searchLabel);
        optionsFlowLayoutPanel.Controls.Add(searchTextBox);
        optionsFlowLayoutPanel.Controls.Add(includeInactiveCheckBox);
        optionsFlowLayoutPanel.Controls.Add(expandAllCheckBox);
        optionsFlowLayoutPanel.Controls.Add(maximumRowsLabel);
        optionsFlowLayoutPanel.Controls.Add(maximumRowsNumericUpDown);
        optionsFlowLayoutPanel.Dock = DockStyle.Fill;
        optionsFlowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;

        searchLabel.AutoSize = true;
        searchLabel.Margin = new Padding(3, 8, 3, 0);
        searchLabel.Text = "Search:";

        searchTextBox.Width = 220;

        includeInactiveCheckBox.AutoSize = true;
        includeInactiveCheckBox.Margin = new Padding(12, 6, 3, 3);
        includeInactiveCheckBox.Text = "Include inactive";

        expandAllCheckBox.AutoSize = true;
        expandAllCheckBox.Margin = new Padding(12, 6, 3, 3);
        expandAllCheckBox.Text = "Expand all";

        maximumRowsLabel.AutoSize = true;
        maximumRowsLabel.Margin = new Padding(12, 8, 3, 0);
        maximumRowsLabel.Text = "Max:";

        maximumRowsNumericUpDown.Minimum = 1;
        maximumRowsNumericUpDown.Maximum = 100000;
        maximumRowsNumericUpDown.Width = 90;

        mainSplitContainer.Dock = DockStyle.Fill;
        mainSplitContainer.SplitterDistance = 360;
        mainSplitContainer.Panel1.Controls.Add(payeesTreeView);
        mainSplitContainer.Panel2.Controls.Add(rightSplitContainer);

        payeesTreeView.Dock = DockStyle.Fill;
        payeesTreeView.HideSelection = false;
        payeesTreeView.AfterSelect += payeesTreeView_AfterSelect;

        rightSplitContainer.Dock = DockStyle.Fill;
        rightSplitContainer.Orientation = Orientation.Horizontal;
        rightSplitContainer.SplitterDistance = 340;
        rightSplitContainer.Panel1.Controls.Add(resultsTextBox);
        rightSplitContainer.Panel2.Controls.Add(selectedNodeTextBox);

        resultsTextBox.Dock = DockStyle.Fill;
        resultsTextBox.Multiline = true;
        resultsTextBox.ReadOnly = true;
        resultsTextBox.ScrollBars = ScrollBars.Both;
        resultsTextBox.WordWrap = false;

        selectedNodeTextBox.Dock = DockStyle.Fill;
        selectedNodeTextBox.Multiline = true;
        selectedNodeTextBox.ReadOnly = true;
        selectedNodeTextBox.ScrollBars = ScrollBars.Both;
        selectedNodeTextBox.WordWrap = false;

        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Text = "Ready.";
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 720);
        Controls.Add(rootTableLayoutPanel);
        Name = "PayeesTreeLoadVerificationFormCommonTree";
        Text = "Payees Tree Verification";

        rootTableLayoutPanel.ResumeLayout(false);
        topTableLayoutPanel.ResumeLayout(false);
        topTableLayoutPanel.PerformLayout();
        optionsFlowLayoutPanel.ResumeLayout(false);
        optionsFlowLayoutPanel.PerformLayout();
        mainSplitContainer.Panel1.ResumeLayout(false);
        mainSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
        mainSplitContainer.ResumeLayout(false);
        rightSplitContainer.Panel1.ResumeLayout(false);
        rightSplitContainer.Panel1.PerformLayout();
        rightSplitContainer.Panel2.ResumeLayout(false);
        rightSplitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)rightSplitContainer).EndInit();
        rightSplitContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)maximumRowsNumericUpDown).EndInit();
        ResumeLayout(false);
    }
}
