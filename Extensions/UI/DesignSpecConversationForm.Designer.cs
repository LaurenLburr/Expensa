namespace CodexExpensa.ExtensionDevHost.UI;

partial class DesignSpecConversationForm
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel rootLayout;
    private Label titleLabel;
    private Label projectNameLabel;
    private TextBox projectNameTextBox;
    private Label designSpecPathLabel;
    private TextBox designSpecPathTextBox;
    private Label conversationPathLabel;
    private TextBox conversationPathTextBox;
    private SplitContainer mainSplitContainer;
    private TableLayoutPanel designSpecLayout;
    private GroupBox designSpecPreviewGroupBox;
    private WebBrowser designSpecPreviewBrowser;
    private GroupBox designSpecSourceGroupBox;
    private TextBox designSpecEditor;
    private TableLayoutPanel conversationLayout;
    private GroupBox conversationGroupBox;
    private TextBox conversationEditor;
    private GroupBox promptGroupBox;
    private TextBox promptTextBox;
    private FlowLayoutPanel buttonPanel;
    private Button saveButton;
    private Button reloadButton;
    private Button addMessageButton;
    private Button addAiPlaceholderButton;
    private Button openDocsFolderButton;
    private Label statusLabel;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components is not null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        rootLayout = new TableLayoutPanel();
        titleLabel = new Label();
        projectNameLabel = new Label();
        projectNameTextBox = new TextBox();
        designSpecPathLabel = new Label();
        designSpecPathTextBox = new TextBox();
        conversationPathLabel = new Label();
        conversationPathTextBox = new TextBox();
        mainSplitContainer = new SplitContainer();
        designSpecLayout = new TableLayoutPanel();
        designSpecPreviewGroupBox = new GroupBox();
        designSpecPreviewBrowser = new WebBrowser();
        designSpecSourceGroupBox = new GroupBox();
        designSpecEditor = new TextBox();
        conversationLayout = new TableLayoutPanel();
        conversationGroupBox = new GroupBox();
        conversationEditor = new TextBox();
        promptGroupBox = new GroupBox();
        promptTextBox = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        saveButton = new Button();
        reloadButton = new Button();
        addMessageButton = new Button();
        addAiPlaceholderButton = new Button();
        openDocsFolderButton = new Button();
        statusLabel = new Label();

        rootLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
        mainSplitContainer.Panel1.SuspendLayout();
        mainSplitContainer.Panel2.SuspendLayout();
        mainSplitContainer.SuspendLayout();
        designSpecLayout.SuspendLayout();
        designSpecPreviewGroupBox.SuspendLayout();
        designSpecSourceGroupBox.SuspendLayout();
        conversationLayout.SuspendLayout();
        conversationGroupBox.SuspendLayout();
        promptGroupBox.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1250, 780);
        Name = "DesignSpecConversationForm";
        Text = "Design Spec Conversation";

        rootLayout.ColumnCount = 2;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Padding = new Padding(10);
        rootLayout.RowCount = 7;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));

        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        titleLabel.Text = "Design Spec Conversation";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        rootLayout.SetColumnSpan(titleLabel, 2);

        projectNameLabel.Text = "Project";
        projectNameLabel.Dock = DockStyle.Fill;
        projectNameLabel.TextAlign = ContentAlignment.MiddleLeft;

        projectNameTextBox.Dock = DockStyle.Fill;
        projectNameTextBox.ReadOnly = true;

        designSpecPathLabel.Text = "Design Spec";
        designSpecPathLabel.Dock = DockStyle.Fill;
        designSpecPathLabel.TextAlign = ContentAlignment.MiddleLeft;

        designSpecPathTextBox.Dock = DockStyle.Fill;
        designSpecPathTextBox.ReadOnly = true;

        conversationPathLabel.Text = "Conversation";
        conversationPathLabel.Dock = DockStyle.Fill;
        conversationPathLabel.TextAlign = ContentAlignment.MiddleLeft;

        conversationPathTextBox.Dock = DockStyle.Fill;
        conversationPathTextBox.ReadOnly = true;

        mainSplitContainer.Dock = DockStyle.Fill;
        mainSplitContainer.Orientation = Orientation.Vertical;
        mainSplitContainer.SplitterDistance = 700;

        designSpecLayout.ColumnCount = 1;
        designSpecLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designSpecLayout.Dock = DockStyle.Fill;
        designSpecLayout.RowCount = 2;
        designSpecLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
        designSpecLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));

        designSpecPreviewGroupBox.Dock = DockStyle.Fill;
        designSpecPreviewGroupBox.Text = "Rendered Design Spec";

        designSpecPreviewBrowser.Dock = DockStyle.Fill;
        designSpecPreviewBrowser.AllowWebBrowserDrop = false;
        designSpecPreviewBrowser.ScriptErrorsSuppressed = true;
        designSpecPreviewBrowser.TabStop = false;

        designSpecPreviewGroupBox.Controls.Add(designSpecPreviewBrowser);

        designSpecSourceGroupBox.Dock = DockStyle.Fill;
        designSpecSourceGroupBox.Text = "Markdown Source";

        designSpecEditor.AcceptsReturn = true;
        designSpecEditor.AcceptsTab = true;
        designSpecEditor.Dock = DockStyle.Fill;
        designSpecEditor.Font = new Font("Consolas", 10F);
        designSpecEditor.Multiline = true;
        designSpecEditor.ScrollBars = ScrollBars.Both;
        designSpecEditor.WordWrap = true;

        designSpecSourceGroupBox.Controls.Add(designSpecEditor);

        designSpecLayout.Controls.Add(designSpecPreviewGroupBox, 0, 0);
        designSpecLayout.Controls.Add(designSpecSourceGroupBox, 0, 1);

        conversationLayout.ColumnCount = 1;
        conversationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        conversationLayout.Dock = DockStyle.Fill;
        conversationLayout.RowCount = 2;
        conversationLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 72F));
        conversationLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));

        conversationGroupBox.Dock = DockStyle.Fill;
        conversationGroupBox.Text = "Ongoing Conversation";

        conversationEditor.AcceptsReturn = true;
        conversationEditor.AcceptsTab = true;
        conversationEditor.Dock = DockStyle.Fill;
        conversationEditor.Font = new Font("Consolas", 10F);
        conversationEditor.Multiline = true;
        conversationEditor.ScrollBars = ScrollBars.Both;
        conversationEditor.WordWrap = true;

        conversationGroupBox.Controls.Add(conversationEditor);

        promptGroupBox.Dock = DockStyle.Fill;
        promptGroupBox.Text = "New Message";

        promptTextBox.AcceptsReturn = true;
        promptTextBox.Dock = DockStyle.Fill;
        promptTextBox.Multiline = true;
        promptTextBox.ScrollBars = ScrollBars.Vertical;
        promptTextBox.WordWrap = true;

        promptGroupBox.Controls.Add(promptTextBox);

        conversationLayout.Controls.Add(conversationGroupBox, 0, 0);
        conversationLayout.Controls.Add(promptGroupBox, 0, 1);

        mainSplitContainer.Panel1.Controls.Add(designSpecLayout);
        mainSplitContainer.Panel2.Controls.Add(conversationLayout);

        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;

        saveButton.AutoSize = true;
        saveButton.Text = "Save";
        saveButton.Click += SaveButton_Click;

        reloadButton.AutoSize = true;
        reloadButton.Text = "Reload";
        reloadButton.Click += ReloadButton_Click;

        addMessageButton.AutoSize = true;
        addMessageButton.Text = "Add Message";
        addMessageButton.Click += AddMessageButton_Click;

        addAiPlaceholderButton.AutoSize = true;
        addAiPlaceholderButton.Text = "Add AI Placeholder";
        addAiPlaceholderButton.Click += AddAiPlaceholderButton_Click;

        openDocsFolderButton.AutoSize = true;
        openDocsFolderButton.Text = "Open Docs Folder";
        openDocsFolderButton.Click += OpenDocsFolderButton_Click;

        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(reloadButton);
        buttonPanel.Controls.Add(addMessageButton);
        buttonPanel.Controls.Add(addAiPlaceholderButton);
        buttonPanel.Controls.Add(openDocsFolderButton);

        statusLabel.Dock = DockStyle.Fill;
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;

        rootLayout.Controls.Add(titleLabel, 0, 0);
        rootLayout.Controls.Add(projectNameLabel, 0, 1);
        rootLayout.Controls.Add(projectNameTextBox, 1, 1);
        rootLayout.Controls.Add(designSpecPathLabel, 0, 2);
        rootLayout.Controls.Add(designSpecPathTextBox, 1, 2);
        rootLayout.Controls.Add(conversationPathLabel, 0, 3);
        rootLayout.Controls.Add(conversationPathTextBox, 1, 3);
        rootLayout.Controls.Add(mainSplitContainer, 0, 4);
        rootLayout.SetColumnSpan(mainSplitContainer, 2);
        rootLayout.Controls.Add(buttonPanel, 0, 5);
        rootLayout.SetColumnSpan(buttonPanel, 2);
        rootLayout.Controls.Add(statusLabel, 0, 6);
        rootLayout.SetColumnSpan(statusLabel, 2);

        Controls.Add(rootLayout);

        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        mainSplitContainer.Panel1.ResumeLayout(false);
        mainSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
        mainSplitContainer.ResumeLayout(false);
        designSpecLayout.ResumeLayout(false);
        designSpecPreviewGroupBox.ResumeLayout(false);
        designSpecSourceGroupBox.ResumeLayout(false);
        designSpecSourceGroupBox.PerformLayout();
        conversationLayout.ResumeLayout(false);
        conversationGroupBox.ResumeLayout(false);
        conversationGroupBox.PerformLayout();
        promptGroupBox.ResumeLayout(false);
        promptGroupBox.PerformLayout();
        buttonPanel.ResumeLayout(false);
        buttonPanel.PerformLayout();
        ResumeLayout(false);
    }
}
