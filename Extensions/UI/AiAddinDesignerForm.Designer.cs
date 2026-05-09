namespace CodexExpensa.ExtensionDevHost.UI;

partial class AiAddinDesignerForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayout;
    private SplitContainer mainSplit;
    private ListBox sessionsListBox;
    private TableLayoutPanel rightLayout;
    private TextBox projectNameTextBox;
    private TabControl designerTabs;
    private TabPage conversationTab;
    private TabPage designSpecTab;
    private TabPage integrationSpecTab;
    private SplitContainer conversationSplit;
    private TextBox conversationTextBox;
    private TextBox userMessageTextBox;
    private TextBox designSpecTextBox;
    private TextBox integrationSpecTextBox;
    private FlowLayoutPanel commandPanel;
    private Button newSessionButton;
    private Button saveSessionButton;
    private Button sendButton;
    private Button exportDocsButton;
    private Label projectNameLabel;

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
        mainSplit = new SplitContainer();
        sessionsListBox = new ListBox();
        rightLayout = new TableLayoutPanel();
        projectNameTextBox = new TextBox();
        designerTabs = new TabControl();
        conversationTab = new TabPage();
        designSpecTab = new TabPage();
        integrationSpecTab = new TabPage();
        conversationSplit = new SplitContainer();
        conversationTextBox = new TextBox();
        userMessageTextBox = new TextBox();
        designSpecTextBox = new TextBox();
        integrationSpecTextBox = new TextBox();
        commandPanel = new FlowLayoutPanel();
        newSessionButton = new Button();
        saveSessionButton = new Button();
        sendButton = new Button();
        exportDocsButton = new Button();
        projectNameLabel = new Label();
        rootLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)mainSplit).BeginInit();
        mainSplit.Panel1.SuspendLayout();
        mainSplit.Panel2.SuspendLayout();
        mainSplit.SuspendLayout();
        rightLayout.SuspendLayout();
        designerTabs.SuspendLayout();
        conversationTab.SuspendLayout();
        designSpecTab.SuspendLayout();
        integrationSpecTab.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)conversationSplit).BeginInit();
        conversationSplit.Panel1.SuspendLayout();
        conversationSplit.Panel2.SuspendLayout();
        conversationSplit.SuspendLayout();
        commandPanel.SuspendLayout();
        SuspendLayout();

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 760);
        MinimumSize = new Size(900, 600);
        StartPosition = FormStartPosition.CenterParent;
        Text = "AI Add-in Designer";

        rootLayout.ColumnCount = 1;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.RowCount = 2;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));

        mainSplit.Dock = DockStyle.Fill;
        mainSplit.FixedPanel = FixedPanel.Panel1;
        mainSplit.SplitterDistance = 260;

        sessionsListBox.Dock = DockStyle.Fill;
        sessionsListBox.FormattingEnabled = true;
        mainSplit.Panel1.Controls.Add(sessionsListBox);

        rightLayout.ColumnCount = 2;
        rightLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95F));
        rightLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rightLayout.Dock = DockStyle.Fill;
        rightLayout.RowCount = 2;
        rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        projectNameLabel.Dock = DockStyle.Fill;
        projectNameLabel.Text = "Project:";
        projectNameLabel.TextAlign = ContentAlignment.MiddleLeft;

        projectNameTextBox.Dock = DockStyle.Fill;
        projectNameTextBox.Margin = new Padding(3, 6, 3, 3);

        designerTabs.Dock = DockStyle.Fill;
        rightLayout.SetColumnSpan(designerTabs, 2);

        conversationTab.Text = "Conversation";
        designSpecTab.Text = "Design Spec";
        integrationSpecTab.Text = "Expensa Integration Spec";

        conversationSplit.Dock = DockStyle.Fill;
        conversationSplit.Orientation = Orientation.Horizontal;
        conversationSplit.FixedPanel = FixedPanel.Panel2;
        conversationSplit.SplitterDistance = 480;

        conversationTextBox.Dock = DockStyle.Fill;
        conversationTextBox.Multiline = true;
        conversationTextBox.ReadOnly = true;
        conversationTextBox.ScrollBars = ScrollBars.Vertical;
        conversationTextBox.WordWrap = true;

        userMessageTextBox.Dock = DockStyle.Fill;
        userMessageTextBox.Multiline = true;
        userMessageTextBox.ScrollBars = ScrollBars.Vertical;
        userMessageTextBox.WordWrap = true;

        conversationSplit.Panel1.Controls.Add(conversationTextBox);
        conversationSplit.Panel2.Controls.Add(userMessageTextBox);
        conversationTab.Controls.Add(conversationSplit);

        designSpecTextBox.AcceptsReturn = true;
        designSpecTextBox.AcceptsTab = true;
        designSpecTextBox.Dock = DockStyle.Fill;
        designSpecTextBox.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);
        designSpecTextBox.Multiline = true;
        designSpecTextBox.ScrollBars = ScrollBars.Both;
        designSpecTextBox.WordWrap = false;
        designSpecTab.Controls.Add(designSpecTextBox);

        integrationSpecTextBox.AcceptsReturn = true;
        integrationSpecTextBox.AcceptsTab = true;
        integrationSpecTextBox.Dock = DockStyle.Fill;
        integrationSpecTextBox.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);
        integrationSpecTextBox.Multiline = true;
        integrationSpecTextBox.ScrollBars = ScrollBars.Both;
        integrationSpecTextBox.WordWrap = false;
        integrationSpecTab.Controls.Add(integrationSpecTextBox);

        designerTabs.TabPages.Add(conversationTab);
        designerTabs.TabPages.Add(designSpecTab);
        designerTabs.TabPages.Add(integrationSpecTab);

        rightLayout.Controls.Add(projectNameLabel, 0, 0);
        rightLayout.Controls.Add(projectNameTextBox, 1, 0);
        rightLayout.Controls.Add(designerTabs, 0, 1);
        mainSplit.Panel2.Controls.Add(rightLayout);

        commandPanel.Dock = DockStyle.Fill;
        commandPanel.FlowDirection = FlowDirection.RightToLeft;
        commandPanel.Padding = new Padding(4);

        sendButton.AutoSize = true;
        sendButton.Text = "Send to AI";
        saveSessionButton.AutoSize = true;
        saveSessionButton.Text = "Save Session";
        exportDocsButton.AutoSize = true;
        exportDocsButton.Text = "Export Docs";
        newSessionButton.AutoSize = true;
        newSessionButton.Text = "New Session";

        commandPanel.Controls.Add(sendButton);
        commandPanel.Controls.Add(saveSessionButton);
        commandPanel.Controls.Add(exportDocsButton);
        commandPanel.Controls.Add(newSessionButton);

        rootLayout.Controls.Add(mainSplit, 0, 0);
        rootLayout.Controls.Add(commandPanel, 0, 1);
        Controls.Add(rootLayout);

        commandPanel.ResumeLayout(false);
        commandPanel.PerformLayout();
        conversationSplit.Panel1.ResumeLayout(false);
        conversationSplit.Panel1.PerformLayout();
        conversationSplit.Panel2.ResumeLayout(false);
        conversationSplit.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)conversationSplit).EndInit();
        conversationSplit.ResumeLayout(false);
        integrationSpecTab.ResumeLayout(false);
        integrationSpecTab.PerformLayout();
        designSpecTab.ResumeLayout(false);
        designSpecTab.PerformLayout();
        conversationTab.ResumeLayout(false);
        designerTabs.ResumeLayout(false);
        rightLayout.ResumeLayout(false);
        rightLayout.PerformLayout();
        mainSplit.Panel1.ResumeLayout(false);
        mainSplit.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)mainSplit).EndInit();
        mainSplit.ResumeLayout(false);
        rootLayout.ResumeLayout(false);
        ResumeLayout(false);
    }
}
