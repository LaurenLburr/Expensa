namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

partial class CommandPaletteDialog
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayoutPanel;
    private Label searchLabel;
    private TextBox searchTextBox;
    private SplitContainer splitContainer;
    private ListView commandListView;
    private ColumnHeader commandNameColumnHeader;
    private ColumnHeader displayNameColumnHeader;
    private ColumnHeader categoryColumnHeader;
    private ColumnHeader enabledColumnHeader;
    private TextBox commandPreviewTextBox;
    private FlowLayoutPanel buttonPanel;
    private CheckBox executeWithParametersCheckBox;
    private Button executeButton;
    private Button cancelButton;

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
        searchLabel = new Label();
        searchTextBox = new TextBox();
        splitContainer = new SplitContainer();
        commandListView = new ListView();
        commandNameColumnHeader = new ColumnHeader();
        displayNameColumnHeader = new ColumnHeader();
        categoryColumnHeader = new ColumnHeader();
        enabledColumnHeader = new ColumnHeader();
        commandPreviewTextBox = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        executeWithParametersCheckBox = new CheckBox();
        executeButton = new Button();
        cancelButton = new Button();

        rootLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();

        rootLayoutPanel.ColumnCount = 1;
        rootLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayoutPanel.Controls.Add(searchLabel, 0, 0);
        rootLayoutPanel.Controls.Add(searchTextBox, 0, 1);
        rootLayoutPanel.Controls.Add(splitContainer, 0, 2);
        rootLayoutPanel.Controls.Add(buttonPanel, 0, 3);
        rootLayoutPanel.Dock = DockStyle.Fill;
        rootLayoutPanel.Location = new Point(8, 8);
        rootLayoutPanel.Name = "rootLayoutPanel";
        rootLayoutPanel.RowCount = 4;
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        rootLayoutPanel.Size = new Size(884, 584);
        rootLayoutPanel.TabIndex = 0;

        searchLabel.Dock = DockStyle.Fill;
        searchLabel.Text = "Search commands:";
        searchLabel.TextAlign = ContentAlignment.MiddleLeft;

        searchTextBox.Dock = DockStyle.Fill;
        searchTextBox.Name = "searchTextBox";
        searchTextBox.TabIndex = 0;
        searchTextBox.TextChanged += searchTextBox_TextChanged;

        splitContainer.Dock = DockStyle.Fill;
        splitContainer.Location = new Point(3, 59);
        splitContainer.Name = "splitContainer";
        splitContainer.Orientation = Orientation.Horizontal;
        splitContainer.SplitterDistance = 300;
        splitContainer.TabIndex = 1;

        splitContainer.Panel1.Controls.Add(commandListView);
        splitContainer.Panel2.Controls.Add(commandPreviewTextBox);

        commandListView.Columns.AddRange(new ColumnHeader[]
        {
            commandNameColumnHeader,
            displayNameColumnHeader,
            categoryColumnHeader,
            enabledColumnHeader
        });
        commandListView.Dock = DockStyle.Fill;
        commandListView.FullRowSelect = true;
        commandListView.MultiSelect = false;
        commandListView.Name = "commandListView";
        commandListView.TabIndex = 1;
        commandListView.UseCompatibleStateImageBehavior = false;
        commandListView.View = View.Details;
        commandListView.SelectedIndexChanged += commandListView_SelectedIndexChanged;
        commandListView.DoubleClick += commandListView_DoubleClick;

        commandNameColumnHeader.Text = "Command";
        commandNameColumnHeader.Width = 260;
        displayNameColumnHeader.Text = "Display Name";
        displayNameColumnHeader.Width = 240;
        categoryColumnHeader.Text = "Category";
        categoryColumnHeader.Width = 180;
        enabledColumnHeader.Text = "Enabled";
        enabledColumnHeader.Width = 80;

        commandPreviewTextBox.Dock = DockStyle.Fill;
        commandPreviewTextBox.Multiline = true;
        commandPreviewTextBox.ReadOnly = true;
        commandPreviewTextBox.ScrollBars = ScrollBars.Both;
        commandPreviewTextBox.WordWrap = false;

        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonPanel.Controls.Add(executeButton);
        buttonPanel.Controls.Add(cancelButton);
        buttonPanel.Controls.Add(executeWithParametersCheckBox);

        executeButton.Text = "Execute";
        executeButton.Width = 100;
        executeButton.Click += executeButton_Click;

        cancelButton.Text = "Cancel";
        cancelButton.Width = 100;
        cancelButton.Click += cancelButton_Click;

        executeWithParametersCheckBox.AutoSize = true;
        executeWithParametersCheckBox.Text = "With parameters";
        executeWithParametersCheckBox.Margin = new Padding(3, 10, 20, 3);

        AcceptButton = executeButton;
        CancelButton = cancelButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 600);
        Controls.Add(rootLayoutPanel);
        MinimizeBox = false;
        MaximizeBox = false;
        Name = "CommandPaletteDialog";
        Padding = new Padding(8);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Command Palette";

        rootLayoutPanel.ResumeLayout(false);
        rootLayoutPanel.PerformLayout();
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        splitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        buttonPanel.ResumeLayout(false);
        buttonPanel.PerformLayout();
        ResumeLayout(false);
    }
}
