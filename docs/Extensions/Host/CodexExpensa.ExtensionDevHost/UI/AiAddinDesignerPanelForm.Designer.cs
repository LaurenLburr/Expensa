namespace CodexExpensa.ExtensionDevHost.UI;

partial class AiAddinDesignerPanelForm
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel rootLayout;
    private Label titleLabel;
    private Label addinNameLabel;
    private TextBox addinNameTextBox;
    private Label projectTypeLabel;
    private ComboBox projectTypeComboBox;
    private Label solutionRootLabel;
    private TextBox solutionRootTextBox;
    private Label projectFolderLabel;
    private TextBox projectFolderTextBox;
    private Label docsFolderLabel;
    private TextBox docsFolderTextBox;
    private Label summaryLabel;
    private TextBox summaryTextBox;
    private Label goalsLabel;
    private TextBox goalsTextBox;
    private Label aiConversationLabel;
    private RichTextBox aiConversationTextBox;
    private FlowLayoutPanel buttonPanel;
    private Button createProjectSpaceButton;
    private Button startConversationButton;
    private Button generateDocsButton;
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
        addinNameLabel = new Label();
        addinNameTextBox = new TextBox();
        projectTypeLabel = new Label();
        projectTypeComboBox = new ComboBox();
        solutionRootLabel = new Label();
        solutionRootTextBox = new TextBox();
        projectFolderLabel = new Label();
        projectFolderTextBox = new TextBox();
        docsFolderLabel = new Label();
        docsFolderTextBox = new TextBox();
        summaryLabel = new Label();
        summaryTextBox = new TextBox();
        goalsLabel = new Label();
        goalsTextBox = new TextBox();
        aiConversationLabel = new Label();
        aiConversationTextBox = new RichTextBox();
        buttonPanel = new FlowLayoutPanel();
        createProjectSpaceButton = new Button();
        startConversationButton = new Button();
        generateDocsButton = new Button();
        statusLabel = new Label();

        rootLayout.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 760);
        Name = "AiAddinDesignerPanelForm";
        Text = "AI Add-in Designer";

        rootLayout.ColumnCount = 2;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Padding = new Padding(12);
        rootLayout.RowCount = 12;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));

        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        titleLabel.Text = "AI Add-in Designer";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        rootLayout.SetColumnSpan(titleLabel, 2);

        addinNameLabel.Text = "Add-in Name";
        addinNameLabel.Dock = DockStyle.Fill;
        addinNameLabel.TextAlign = ContentAlignment.MiddleLeft;

        addinNameTextBox.Dock = DockStyle.Fill;

        projectTypeLabel.Text = "Project Type";
        projectTypeLabel.Dock = DockStyle.Fill;
        projectTypeLabel.TextAlign = ContentAlignment.MiddleLeft;

        projectTypeComboBox.Dock = DockStyle.Left;
        projectTypeComboBox.Width = 250;
        projectTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        projectTypeComboBox.Items.AddRange(new object[]
        {
            "Expensa Extension",
            "Standalone Tool",
            "Shared Service",
            "Experimental Prototype"
        });
        projectTypeComboBox.SelectedIndex = 0;

        solutionRootLabel.Text = "Solution Root";
        solutionRootLabel.Dock = DockStyle.Fill;
        solutionRootLabel.TextAlign = ContentAlignment.MiddleLeft;

        solutionRootTextBox.Dock = DockStyle.Fill;

        projectFolderLabel.Text = "Project Folder";
        projectFolderLabel.Dock = DockStyle.Fill;
        projectFolderLabel.TextAlign = ContentAlignment.MiddleLeft;

        projectFolderTextBox.Dock = DockStyle.Fill;
        projectFolderTextBox.ReadOnly = true;

        docsFolderLabel.Text = "Docs Folder";
        docsFolderLabel.Dock = DockStyle.Fill;
        docsFolderLabel.TextAlign = ContentAlignment.MiddleLeft;

        docsFolderTextBox.Dock = DockStyle.Fill;
        docsFolderTextBox.ReadOnly = true;

        summaryLabel.Text = "Summary";
        summaryLabel.Dock = DockStyle.Fill;
        summaryLabel.TextAlign = ContentAlignment.TopLeft;

        summaryTextBox.Dock = DockStyle.Fill;
        summaryTextBox.Multiline = true;
        summaryTextBox.ScrollBars = ScrollBars.Vertical;

        goalsLabel.Text = "Goals / Requirements";
        goalsLabel.Dock = DockStyle.Fill;
        goalsLabel.TextAlign = ContentAlignment.TopLeft;

        goalsTextBox.Dock = DockStyle.Fill;
        goalsTextBox.Multiline = true;
        goalsTextBox.ScrollBars = ScrollBars.Vertical;

        aiConversationLabel.Text = "AI Design Conversation";
        aiConversationLabel.Dock = DockStyle.Fill;
        aiConversationLabel.TextAlign = ContentAlignment.MiddleLeft;
        rootLayout.SetColumnSpan(aiConversationLabel, 2);

        aiConversationTextBox.Dock = DockStyle.Fill;
        aiConversationTextBox.Font = new Font("Consolas", 10F);
        aiConversationTextBox.Text =
@"This panel is the foundation for the conversational AI add-in workflow.

Current workflow:
1. Enter add-in name
2. Enter summary/goals
3. Create project space
4. Required docs are created
5. Project is registered
6. Main tree can refresh and show the new project

Future workflow:
1. Discuss the add-in with AI
2. Refine behavior and architecture
3. Generate or update design docs
4. Generate scaffold projects
5. Iterate on generated code";
        rootLayout.SetColumnSpan(aiConversationTextBox, 2);

        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        rootLayout.SetColumnSpan(buttonPanel, 2);

        createProjectSpaceButton.AutoSize = true;
        createProjectSpaceButton.Text = "Create Project Space";
        createProjectSpaceButton.Click += CreateProjectSpaceButton_Click;

        startConversationButton.AutoSize = true;
        startConversationButton.Text = "Start AI Conversation";
        startConversationButton.Click += StartConversationButton_Click;

        generateDocsButton.AutoSize = true;
        generateDocsButton.Text = "Generate Docs";
        generateDocsButton.Click += GenerateDocsButton_Click;

        buttonPanel.Controls.Add(startConversationButton);
        buttonPanel.Controls.Add(generateDocsButton);
        buttonPanel.Controls.Add(createProjectSpaceButton);

        statusLabel.Dock = DockStyle.Fill;
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        statusLabel.Text = "Ready.";
        rootLayout.SetColumnSpan(statusLabel, 2);

        rootLayout.Controls.Add(titleLabel, 0, 0);
        rootLayout.Controls.Add(addinNameLabel, 0, 1);
        rootLayout.Controls.Add(addinNameTextBox, 1, 1);
        rootLayout.Controls.Add(projectTypeLabel, 0, 2);
        rootLayout.Controls.Add(projectTypeComboBox, 1, 2);
        rootLayout.Controls.Add(solutionRootLabel, 0, 3);
        rootLayout.Controls.Add(solutionRootTextBox, 1, 3);
        rootLayout.Controls.Add(projectFolderLabel, 0, 4);
        rootLayout.Controls.Add(projectFolderTextBox, 1, 4);
        rootLayout.Controls.Add(docsFolderLabel, 0, 5);
        rootLayout.Controls.Add(docsFolderTextBox, 1, 5);
        rootLayout.Controls.Add(summaryLabel, 0, 6);
        rootLayout.Controls.Add(summaryTextBox, 1, 6);
        rootLayout.Controls.Add(goalsLabel, 0, 7);
        rootLayout.Controls.Add(goalsTextBox, 1, 7);
        rootLayout.Controls.Add(aiConversationLabel, 0, 8);
        rootLayout.Controls.Add(aiConversationTextBox, 0, 9);
        rootLayout.Controls.Add(buttonPanel, 0, 10);
        rootLayout.Controls.Add(statusLabel, 0, 11);

        Controls.Add(rootLayout);

        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        buttonPanel.ResumeLayout(false);
        buttonPanel.PerformLayout();
        ResumeLayout(false);
    }
}
