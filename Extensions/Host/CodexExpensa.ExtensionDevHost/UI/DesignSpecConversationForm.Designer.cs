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
        // 
        // rootLayout
        // 
        rootLayout.ColumnCount = 2;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Controls.Add(titleLabel, 0, 0);
        rootLayout.Controls.Add(projectNameLabel, 0, 1);
        rootLayout.Controls.Add(projectNameTextBox, 1, 1);
        rootLayout.Controls.Add(designSpecPathLabel, 0, 2);
        rootLayout.Controls.Add(designSpecPathTextBox, 1, 2);
        rootLayout.Controls.Add(conversationPathLabel, 0, 3);
        rootLayout.Controls.Add(conversationPathTextBox, 1, 3);
        rootLayout.Controls.Add(mainSplitContainer, 0, 4);
        rootLayout.Controls.Add(buttonPanel, 0, 5);
        rootLayout.Controls.Add(statusLabel, 0, 6);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Location = new Point(0, 0);
        rootLayout.Name = "rootLayout";
        rootLayout.Padding = new Padding(10);
        rootLayout.RowCount = 7;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        rootLayout.Size = new Size(1250, 780);
        rootLayout.TabIndex = 0;
        // 
        // titleLabel
        // 
        rootLayout.SetColumnSpan(titleLabel, 2);
        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        titleLabel.Location = new Point(13, 10);
        titleLabel.Name = "titleLabel";
        titleLabel.Size = new Size(1224, 40);
        titleLabel.TabIndex = 0;
        titleLabel.Text = "Design Spec Conversation";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // projectNameLabel
        // 
        projectNameLabel.Dock = DockStyle.Fill;
        projectNameLabel.Location = new Point(13, 50);
        projectNameLabel.Name = "projectNameLabel";
        projectNameLabel.Size = new Size(144, 30);
        projectNameLabel.TabIndex = 1;
        projectNameLabel.Text = "Project";
        projectNameLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // projectNameTextBox
        // 
        projectNameTextBox.Dock = DockStyle.Fill;
        projectNameTextBox.Location = new Point(163, 53);
        projectNameTextBox.Name = "projectNameTextBox";
        projectNameTextBox.ReadOnly = true;
        projectNameTextBox.Size = new Size(1074, 23);
        projectNameTextBox.TabIndex = 2;
        // 
        // designSpecPathLabel
        // 
        designSpecPathLabel.Dock = DockStyle.Fill;
        designSpecPathLabel.Location = new Point(13, 80);
        designSpecPathLabel.Name = "designSpecPathLabel";
        designSpecPathLabel.Size = new Size(144, 30);
        designSpecPathLabel.TabIndex = 3;
        designSpecPathLabel.Text = "Design Spec";
        designSpecPathLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // designSpecPathTextBox
        // 
        designSpecPathTextBox.Dock = DockStyle.Fill;
        designSpecPathTextBox.Location = new Point(163, 83);
        designSpecPathTextBox.Name = "designSpecPathTextBox";
        designSpecPathTextBox.ReadOnly = true;
        designSpecPathTextBox.Size = new Size(1074, 23);
        designSpecPathTextBox.TabIndex = 4;
        // 
        // conversationPathLabel
        // 
        conversationPathLabel.Dock = DockStyle.Fill;
        conversationPathLabel.Location = new Point(13, 110);
        conversationPathLabel.Name = "conversationPathLabel";
        conversationPathLabel.Size = new Size(144, 30);
        conversationPathLabel.TabIndex = 5;
        conversationPathLabel.Text = "Conversation";
        conversationPathLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // conversationPathTextBox
        // 
        conversationPathTextBox.Dock = DockStyle.Fill;
        conversationPathTextBox.Location = new Point(163, 113);
        conversationPathTextBox.Name = "conversationPathTextBox";
        conversationPathTextBox.ReadOnly = true;
        conversationPathTextBox.Size = new Size(1074, 23);
        conversationPathTextBox.TabIndex = 6;
        // 
        // mainSplitContainer
        // 
        rootLayout.SetColumnSpan(mainSplitContainer, 2);
        mainSplitContainer.Dock = DockStyle.Fill;
        mainSplitContainer.Location = new Point(13, 143);
        mainSplitContainer.Name = "mainSplitContainer";
        // 
        // mainSplitContainer.Panel1
        // 
        mainSplitContainer.Panel1.Controls.Add(designSpecLayout);
        // 
        // mainSplitContainer.Panel2
        // 
        mainSplitContainer.Panel2.Controls.Add(conversationLayout);
        mainSplitContainer.Size = new Size(1224, 554);
        mainSplitContainer.SplitterDistance = 987;
        mainSplitContainer.TabIndex = 7;
        // 
        // designSpecLayout
        // 
        designSpecLayout.ColumnCount = 1;
        designSpecLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designSpecLayout.Controls.Add(designSpecPreviewGroupBox, 0, 0);
        designSpecLayout.Controls.Add(designSpecSourceGroupBox, 0, 1);
        designSpecLayout.Dock = DockStyle.Fill;
        designSpecLayout.Location = new Point(0, 0);
        designSpecLayout.Name = "designSpecLayout";
        designSpecLayout.RowCount = 2;
        designSpecLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
        designSpecLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
        designSpecLayout.Size = new Size(987, 554);
        designSpecLayout.TabIndex = 0;
        // 
        // designSpecPreviewGroupBox
        // 
        designSpecPreviewGroupBox.Controls.Add(designSpecPreviewBrowser);
        designSpecPreviewGroupBox.Dock = DockStyle.Fill;
        designSpecPreviewGroupBox.Location = new Point(3, 3);
        designSpecPreviewGroupBox.Name = "designSpecPreviewGroupBox";
        designSpecPreviewGroupBox.Size = new Size(981, 326);
        designSpecPreviewGroupBox.TabIndex = 0;
        designSpecPreviewGroupBox.TabStop = false;
        designSpecPreviewGroupBox.Text = "Rendered Design Spec";
        // 
        // designSpecPreviewBrowser
        // 
        designSpecPreviewBrowser.AllowWebBrowserDrop = false;
        designSpecPreviewBrowser.Dock = DockStyle.Fill;
        designSpecPreviewBrowser.Location = new Point(3, 19);
        designSpecPreviewBrowser.Name = "designSpecPreviewBrowser";
        designSpecPreviewBrowser.ScriptErrorsSuppressed = true;
        designSpecPreviewBrowser.Size = new Size(975, 304);
        designSpecPreviewBrowser.TabIndex = 0;
        designSpecPreviewBrowser.TabStop = false;
        // 
        // designSpecSourceGroupBox
        // 
        designSpecSourceGroupBox.Controls.Add(designSpecEditor);
        designSpecSourceGroupBox.Dock = DockStyle.Fill;
        designSpecSourceGroupBox.Location = new Point(3, 335);
        designSpecSourceGroupBox.Name = "designSpecSourceGroupBox";
        designSpecSourceGroupBox.Size = new Size(981, 216);
        designSpecSourceGroupBox.TabIndex = 1;
        designSpecSourceGroupBox.TabStop = false;
        designSpecSourceGroupBox.Text = "Markdown Source";
        // 
        // designSpecEditor
        // 
        designSpecEditor.AcceptsReturn = true;
        designSpecEditor.AcceptsTab = true;
        designSpecEditor.Dock = DockStyle.Fill;
        designSpecEditor.Font = new Font("Consolas", 10F);
        designSpecEditor.Location = new Point(3, 19);
        designSpecEditor.Multiline = true;
        designSpecEditor.Name = "designSpecEditor";
        designSpecEditor.ScrollBars = ScrollBars.Both;
        designSpecEditor.Size = new Size(975, 194);
        designSpecEditor.TabIndex = 0;
        // 
        // conversationLayout
        // 
        conversationLayout.ColumnCount = 1;
        conversationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        conversationLayout.Controls.Add(conversationGroupBox, 0, 0);
        conversationLayout.Controls.Add(promptGroupBox, 0, 1);
        conversationLayout.Dock = DockStyle.Fill;
        conversationLayout.Location = new Point(0, 0);
        conversationLayout.Name = "conversationLayout";
        conversationLayout.RowCount = 2;
        conversationLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 72F));
        conversationLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
        conversationLayout.Size = new Size(233, 554);
        conversationLayout.TabIndex = 0;
        // 
        // conversationGroupBox
        // 
        conversationGroupBox.Controls.Add(conversationEditor);
        conversationGroupBox.Dock = DockStyle.Fill;
        conversationGroupBox.Location = new Point(3, 3);
        conversationGroupBox.Name = "conversationGroupBox";
        conversationGroupBox.Size = new Size(227, 392);
        conversationGroupBox.TabIndex = 0;
        conversationGroupBox.TabStop = false;
        conversationGroupBox.Text = "Ongoing Conversation";
        // 
        // conversationEditor
        // 
        conversationEditor.AcceptsReturn = true;
        conversationEditor.AcceptsTab = true;
        conversationEditor.Dock = DockStyle.Fill;
        conversationEditor.Font = new Font("Consolas", 10F);
        conversationEditor.Location = new Point(3, 19);
        conversationEditor.Multiline = true;
        conversationEditor.Name = "conversationEditor";
        conversationEditor.ScrollBars = ScrollBars.Both;
        conversationEditor.Size = new Size(221, 370);
        conversationEditor.TabIndex = 0;
        // 
        // promptGroupBox
        // 
        promptGroupBox.Controls.Add(promptTextBox);
        promptGroupBox.Dock = DockStyle.Fill;
        promptGroupBox.Location = new Point(3, 401);
        promptGroupBox.Name = "promptGroupBox";
        promptGroupBox.Size = new Size(227, 150);
        promptGroupBox.TabIndex = 1;
        promptGroupBox.TabStop = false;
        promptGroupBox.Text = "New Message";
        // 
        // promptTextBox
        // 
        promptTextBox.AcceptsReturn = true;
        promptTextBox.Dock = DockStyle.Fill;
        promptTextBox.Location = new Point(3, 19);
        promptTextBox.Multiline = true;
        promptTextBox.Name = "promptTextBox";
        promptTextBox.ScrollBars = ScrollBars.Vertical;
        promptTextBox.Size = new Size(221, 128);
        promptTextBox.TabIndex = 0;
        // 
        // buttonPanel
        // 
        rootLayout.SetColumnSpan(buttonPanel, 2);
        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(reloadButton);
        buttonPanel.Controls.Add(addMessageButton);
        buttonPanel.Controls.Add(addAiPlaceholderButton);
        buttonPanel.Controls.Add(openDocsFolderButton);
        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonPanel.Location = new Point(13, 703);
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Size = new Size(1224, 38);
        buttonPanel.TabIndex = 8;
        // 
        // saveButton
        // 
        saveButton.AutoSize = true;
        saveButton.Location = new Point(1146, 3);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(75, 25);
        saveButton.TabIndex = 0;
        saveButton.Text = "Save";
        saveButton.Click += SaveButton_Click;
        // 
        // reloadButton
        // 
        reloadButton.AutoSize = true;
        reloadButton.Location = new Point(1065, 3);
        reloadButton.Name = "reloadButton";
        reloadButton.Size = new Size(75, 25);
        reloadButton.TabIndex = 1;
        reloadButton.Text = "Reload";
        reloadButton.Click += ReloadButton_Click;
        // 
        // addMessageButton
        // 
        addMessageButton.AutoSize = true;
        addMessageButton.Location = new Point(971, 3);
        addMessageButton.Name = "addMessageButton";
        addMessageButton.Size = new Size(88, 25);
        addMessageButton.TabIndex = 2;
        addMessageButton.Text = "Add Message";
        addMessageButton.Click += AddMessageButton_Click;
        // 
        // addAiPlaceholderButton
        // 
        addAiPlaceholderButton.AutoSize = true;
        addAiPlaceholderButton.Location = new Point(847, 3);
        addAiPlaceholderButton.Name = "addAiPlaceholderButton";
        addAiPlaceholderButton.Size = new Size(118, 25);
        addAiPlaceholderButton.TabIndex = 3;
        addAiPlaceholderButton.Text = "Add AI Placeholder";
        addAiPlaceholderButton.Click += AddAiPlaceholderButton_Click;
        // 
        // openDocsFolderButton
        // 
        openDocsFolderButton.AutoSize = true;
        openDocsFolderButton.Location = new Point(730, 3);
        openDocsFolderButton.Name = "openDocsFolderButton";
        openDocsFolderButton.Size = new Size(111, 25);
        openDocsFolderButton.TabIndex = 4;
        openDocsFolderButton.Text = "Open Docs Folder";
        openDocsFolderButton.Click += OpenDocsFolderButton_Click;
        // 
        // statusLabel
        // 
        rootLayout.SetColumnSpan(statusLabel, 2);
        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Location = new Point(13, 744);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(1224, 26);
        statusLabel.TabIndex = 9;
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // DesignSpecConversationForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1250, 780);
        Controls.Add(rootLayout);
        Name = "DesignSpecConversationForm";
        Text = "Design Spec Conversation";
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
