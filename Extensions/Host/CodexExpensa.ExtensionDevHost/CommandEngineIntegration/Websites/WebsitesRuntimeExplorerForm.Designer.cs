namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

partial class WebsitesRuntimeExplorerForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayoutPanel;
    private TableLayoutPanel controlLayoutPanel;
    private Label searchLabel;
    private TextBox searchTextBox;
    private CheckBox includeDisabledCheckBox;
    private Label maximumRowsLabel;
    private NumericUpDown maximumRowsNumericUpDown;
    private Button loadButton;
    private Label statusLabel;
    private SplitContainer splitContainer;
    private TreeView websitesTreeView;
    private TextBox detailsTextBox;
    private FlowLayoutPanel buttonPanel;
    private Button closeButton;

    protected override void Dispose(
        bool disposing)
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
        controlLayoutPanel = new TableLayoutPanel();
        searchLabel = new Label();
        searchTextBox = new TextBox();
        includeDisabledCheckBox = new CheckBox();
        maximumRowsLabel = new Label();
        maximumRowsNumericUpDown = new NumericUpDown();
        loadButton = new Button();
        statusLabel = new Label();
        splitContainer = new SplitContainer();
        websitesTreeView = new TreeView();
        detailsTextBox = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        closeButton = new Button();

        rootLayoutPanel.SuspendLayout();
        controlLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)maximumRowsNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();

        rootLayoutPanel.ColumnCount = 1;
        rootLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayoutPanel.Controls.Add(controlLayoutPanel, 0, 0);
        rootLayoutPanel.Controls.Add(statusLabel, 0, 1);
        rootLayoutPanel.Controls.Add(splitContainer, 0, 2);
        rootLayoutPanel.Controls.Add(buttonPanel, 0, 3);
        rootLayoutPanel.Dock = DockStyle.Fill;
        rootLayoutPanel.RowCount = 4;
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

        controlLayoutPanel.ColumnCount = 6;
        controlLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
        controlLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        controlLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
        controlLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        controlLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        controlLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        controlLayoutPanel.Controls.Add(searchLabel, 0, 0);
        controlLayoutPanel.Controls.Add(searchTextBox, 1, 0);
        controlLayoutPanel.Controls.Add(includeDisabledCheckBox, 2, 0);
        controlLayoutPanel.Controls.Add(maximumRowsLabel, 3, 0);
        controlLayoutPanel.Controls.Add(maximumRowsNumericUpDown, 4, 0);
        controlLayoutPanel.Controls.Add(loadButton, 5, 0);
        controlLayoutPanel.Dock = DockStyle.Fill;
        controlLayoutPanel.RowCount = 1;
        controlLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

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

        loadButton.Dock = DockStyle.Fill;
        loadButton.Text = "Load Websites";
        loadButton.Click += loadButton_Click;

        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Text = "Ready.";
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;

        splitContainer.Dock = DockStyle.Fill;
        splitContainer.SplitterDistance = 360;
        splitContainer.Panel1.Controls.Add(websitesTreeView);
        splitContainer.Panel2.Controls.Add(detailsTextBox);

        websitesTreeView.Dock = DockStyle.Fill;
        websitesTreeView.HideSelection = false;
        websitesTreeView.AfterSelect += websitesTreeView_AfterSelect;
        websitesTreeView.DoubleClick += websitesTreeView_DoubleClick;

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

        AcceptButton = loadButton;
        CancelButton = closeButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 650);
        Controls.Add(rootLayoutPanel);
        MinimizeBox = false;
        MaximizeBox = false;
        Name = "WebsitesRuntimeExplorerForm";
        Padding = new Padding(8);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Websites Runtime Explorer";

        rootLayoutPanel.ResumeLayout(false);
        controlLayoutPanel.ResumeLayout(false);
        controlLayoutPanel.PerformLayout();
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
