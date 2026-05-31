namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

partial class WebsitesTreeLoadVerificationForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayoutPanel;
    private TableLayoutPanel filterLayoutPanel;
    private Label searchLabel;
    private TextBox searchTextBox;
    private CheckBox includeDisabledCheckBox;
    private CheckBox expandAllCheckBox;
    private Label maximumRowsLabel;
    private NumericUpDown maximumRowsNumericUpDown;
    private Button loadButton;
    private Label statusLabel;
    private SplitContainer splitContainer;
    private TreeView websitesTreeView;
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
        components = new System.ComponentModel.Container();
        rootLayoutPanel = new TableLayoutPanel();
        filterLayoutPanel = new TableLayoutPanel();
        searchLabel = new Label();
        searchTextBox = new TextBox();
        includeDisabledCheckBox = new CheckBox();
        expandAllCheckBox = new CheckBox();
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
        rootLayoutPanel.Controls.Add(filterLayoutPanel, 0, 0);
        rootLayoutPanel.Controls.Add(statusLabel, 0, 1);
        rootLayoutPanel.Controls.Add(splitContainer, 0, 2);
        rootLayoutPanel.Controls.Add(buttonPanel, 0, 3);
        rootLayoutPanel.Dock = DockStyle.Fill;
        rootLayoutPanel.RowCount = 4;
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

        filterLayoutPanel.ColumnCount = 7;
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        filterLayoutPanel.Controls.Add(searchLabel, 0, 0);
        filterLayoutPanel.Controls.Add(searchTextBox, 1, 0);
        filterLayoutPanel.Controls.Add(includeDisabledCheckBox, 2, 0);
        filterLayoutPanel.Controls.Add(expandAllCheckBox, 3, 0);
        filterLayoutPanel.Controls.Add(maximumRowsLabel, 4, 0);
        filterLayoutPanel.Controls.Add(maximumRowsNumericUpDown, 5, 0);
        filterLayoutPanel.Controls.Add(loadButton, 6, 0);
        filterLayoutPanel.Dock = DockStyle.Fill;

        searchLabel.Dock = DockStyle.Fill;
        searchLabel.Text = "Search:";
        searchLabel.TextAlign = ContentAlignment.MiddleLeft;

        searchTextBox.Dock = DockStyle.Fill;

        includeDisabledCheckBox.Dock = DockStyle.Fill;
        includeDisabledCheckBox.Text = "Disabled";
        includeDisabledCheckBox.TextAlign = ContentAlignment.MiddleLeft;

        expandAllCheckBox.Dock = DockStyle.Fill;
        expandAllCheckBox.Text = "Expand";
        expandAllCheckBox.TextAlign = ContentAlignment.MiddleLeft;

        maximumRowsLabel.Dock = DockStyle.Fill;
        maximumRowsLabel.Text = "Max:";
        maximumRowsLabel.TextAlign = ContentAlignment.MiddleLeft;

        maximumRowsNumericUpDown.Dock = DockStyle.Left;
        maximumRowsNumericUpDown.Minimum = 1;
        maximumRowsNumericUpDown.Maximum = 100000;
        maximumRowsNumericUpDown.Value = 500;
        maximumRowsNumericUpDown.Width = 80;

        loadButton.Dock = DockStyle.Fill;
        loadButton.Text = "Load";
        loadButton.Click += loadButton_Click;

        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Text = "Ready.";
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;

        splitContainer.Dock = DockStyle.Fill;
        splitContainer.SplitterDistance = 380;
        splitContainer.Panel1.Controls.Add(websitesTreeView);
        splitContainer.Panel2.Controls.Add(detailsTextBox);

        websitesTreeView.Dock = DockStyle.Fill;
        websitesTreeView.HideSelection = false;
        websitesTreeView.AfterSelect += websitesTreeView_AfterSelect;

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
        Name = "WebsitesTreeLoadVerificationForm";
        Padding = new Padding(8);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Websites Tree Load Verification";

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
