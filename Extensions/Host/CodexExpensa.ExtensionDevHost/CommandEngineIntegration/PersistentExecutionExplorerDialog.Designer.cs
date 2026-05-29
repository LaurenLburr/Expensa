namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

partial class PersistentExecutionExplorerDialog
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayoutPanel;
    private TableLayoutPanel filterLayoutPanel;
    private Label sourceFilterLabel;
    private ComboBox sourceFilterComboBox;
    private Label statusFilterLabel;
    private ComboBox statusFilterComboBox;
    private Label searchLabel;
    private TextBox searchTextBox;
    private Label sortModeLabel;
    private ComboBox sortModeComboBox;
    private Label maximumRowsLabel;
    private NumericUpDown maximumRowsNumericUpDown;
    private Button refreshButton;
    private Label summaryLabel;
    private SplitContainer splitContainer;
    private ListView recordsListView;
    private TextBox detailsTextBox;
    private FlowLayoutPanel buttonPanel;
    private Button exportJsonButton;
    private Button exportMarkdownButton;
    private Button copyParametersButton;
    private Button copyOutputButton;
    private Button selectButton;
    private Button closeButton;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
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
        sourceFilterLabel = new Label();
        sourceFilterComboBox = new ComboBox();
        statusFilterLabel = new Label();
        statusFilterComboBox = new ComboBox();
        searchLabel = new Label();
        searchTextBox = new TextBox();
        sortModeLabel = new Label();
        sortModeComboBox = new ComboBox();
        maximumRowsLabel = new Label();
        maximumRowsNumericUpDown = new NumericUpDown();
        refreshButton = new Button();
        summaryLabel = new Label();
        splitContainer = new SplitContainer();
        recordsListView = new ListView();
        detailsTextBox = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        exportJsonButton = new Button();
        exportMarkdownButton = new Button();
        copyParametersButton = new Button();
        copyOutputButton = new Button();
        selectButton = new Button();
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
        rootLayoutPanel.Controls.Add(summaryLabel, 0, 1);
        rootLayoutPanel.Controls.Add(splitContainer, 0, 2);
        rootLayoutPanel.Controls.Add(buttonPanel, 0, 3);
        rootLayoutPanel.Dock = DockStyle.Fill;
        rootLayoutPanel.RowCount = 4;
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

        filterLayoutPanel.ColumnCount = 6;
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
        filterLayoutPanel.Dock = DockStyle.Fill;
        filterLayoutPanel.RowCount = 2;
        filterLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        filterLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        filterLayoutPanel.Controls.Add(sourceFilterLabel, 0, 0);
        filterLayoutPanel.Controls.Add(sourceFilterComboBox, 1, 0);
        filterLayoutPanel.Controls.Add(statusFilterLabel, 2, 0);
        filterLayoutPanel.Controls.Add(statusFilterComboBox, 3, 0);
        filterLayoutPanel.Controls.Add(searchLabel, 4, 0);
        filterLayoutPanel.Controls.Add(searchTextBox, 5, 0);
        filterLayoutPanel.Controls.Add(sortModeLabel, 0, 1);
        filterLayoutPanel.Controls.Add(sortModeComboBox, 1, 1);
        filterLayoutPanel.Controls.Add(maximumRowsLabel, 2, 1);
        filterLayoutPanel.Controls.Add(maximumRowsNumericUpDown, 3, 1);
        filterLayoutPanel.Controls.Add(refreshButton, 5, 1);

        sourceFilterLabel.Dock = DockStyle.Fill;
        sourceFilterLabel.Text = "Source:";
        sourceFilterLabel.TextAlign = ContentAlignment.MiddleLeft;
        sourceFilterComboBox.Dock = DockStyle.Fill;
        sourceFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        statusFilterLabel.Dock = DockStyle.Fill;
        statusFilterLabel.Text = "Status:";
        statusFilterLabel.TextAlign = ContentAlignment.MiddleLeft;
        statusFilterComboBox.Dock = DockStyle.Fill;
        statusFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        searchLabel.Dock = DockStyle.Fill;
        searchLabel.Text = "Search:";
        searchLabel.TextAlign = ContentAlignment.MiddleLeft;
        searchTextBox.Dock = DockStyle.Fill;

        sortModeLabel.Dock = DockStyle.Fill;
        sortModeLabel.Text = "Sort:";
        sortModeLabel.TextAlign = ContentAlignment.MiddleLeft;
        sortModeComboBox.Dock = DockStyle.Fill;
        sortModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        maximumRowsLabel.Dock = DockStyle.Fill;
        maximumRowsLabel.Text = "Max rows:";
        maximumRowsLabel.TextAlign = ContentAlignment.MiddleLeft;
        maximumRowsNumericUpDown.Minimum = 1;
        maximumRowsNumericUpDown.Maximum = 100000;
        maximumRowsNumericUpDown.Value = 500;
        maximumRowsNumericUpDown.Width = 120;

        refreshButton.Dock = DockStyle.Right;
        refreshButton.Text = "Refresh";
        refreshButton.Width = 120;
        refreshButton.Click += refreshButton_Click;

        summaryLabel.Dock = DockStyle.Fill;
        summaryLabel.Text = "Rows: 0";
        summaryLabel.TextAlign = ContentAlignment.MiddleLeft;

        splitContainer.Dock = DockStyle.Fill;
        splitContainer.Orientation = Orientation.Horizontal;
        splitContainer.SplitterDistance = 360;
        splitContainer.Panel1.Controls.Add(recordsListView);
        splitContainer.Panel2.Controls.Add(detailsTextBox);

        recordsListView.Dock = DockStyle.Fill;
        recordsListView.FullRowSelect = true;
        recordsListView.MultiSelect = false;
        recordsListView.UseCompatibleStateImageBehavior = false;
        recordsListView.View = View.Details;
        recordsListView.SelectedIndexChanged += recordsListView_SelectedIndexChanged;
        recordsListView.DoubleClick += recordsListView_DoubleClick;

        detailsTextBox.Dock = DockStyle.Fill;
        detailsTextBox.Multiline = true;
        detailsTextBox.ReadOnly = true;
        detailsTextBox.ScrollBars = ScrollBars.Both;
        detailsTextBox.WordWrap = false;

        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonPanel.Controls.Add(closeButton);
        buttonPanel.Controls.Add(selectButton);
        buttonPanel.Controls.Add(copyOutputButton);
        buttonPanel.Controls.Add(copyParametersButton);
        buttonPanel.Controls.Add(exportMarkdownButton);
        buttonPanel.Controls.Add(exportJsonButton);

        closeButton.Text = "Close";
        closeButton.Width = 100;
        closeButton.Click += closeButton_Click;
        selectButton.Text = "Select";
        selectButton.Width = 100;
        selectButton.Click += selectButton_Click;
        copyOutputButton.Text = "Copy Output";
        copyOutputButton.Width = 120;
        copyOutputButton.Click += copyOutputButton_Click;
        copyParametersButton.Text = "Copy Parameters";
        copyParametersButton.Width = 140;
        copyParametersButton.Click += copyParametersButton_Click;

        exportMarkdownButton.Text = "Export Markdown";
        exportMarkdownButton.Width = 140;
        exportMarkdownButton.Click += exportMarkdownButton_Click;
        exportJsonButton.Text = "Export JSON";
        exportJsonButton.Width = 120;
        exportJsonButton.Click += exportJsonButton_Click;
        AcceptButton = selectButton;
        CancelButton = closeButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 700);
        Controls.Add(rootLayoutPanel);
        MinimizeBox = false;
        MaximizeBox = false;
        Name = "PersistentExecutionExplorerDialog";
        Padding = new Padding(8);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Persistent Execution Explorer";

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


