namespace CodexExpensa.ExtensionDevHost.UI;

partial class StandardAiDocsUpdateForm
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel rootLayout;
    private Label titleLabel;
    private ListBox documentListBox;
    private TextBox pathTextBox;
    private TextBox previewTextBox;
    private TextBox promptTextBox;
    private FlowLayoutPanel buttonPanel;
    private Button updateSelectedButton;
    private Button updateAllButton;
    private Button openFolderButton;
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
        documentListBox = new ListBox();
        pathTextBox = new TextBox();
        previewTextBox = new TextBox();
        promptTextBox = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        updateSelectedButton = new Button();
        updateAllButton = new Button();
        openFolderButton = new Button();
        statusLabel = new Label();

        rootLayout.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 720);
        Name = "StandardAiDocsUpdateForm";
        Text = "Standard AI Docs";

        rootLayout.ColumnCount = 2;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Padding = new Padding(12);
        rootLayout.RowCount = 6;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));

        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        titleLabel.Text = "Standard AI Instructions / General Rules";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        rootLayout.SetColumnSpan(titleLabel, 2);

        documentListBox.Dock = DockStyle.Fill;
        documentListBox.SelectedIndexChanged += DocumentListBox_SelectedIndexChanged;
        rootLayout.SetRowSpan(documentListBox, 3);

        pathTextBox.Dock = DockStyle.Fill;
        pathTextBox.ReadOnly = true;

        previewTextBox.AcceptsReturn = true;
        previewTextBox.AcceptsTab = true;
        previewTextBox.Dock = DockStyle.Fill;
        previewTextBox.Font = new Font("Consolas", 10F);
        previewTextBox.Multiline = true;
        previewTextBox.ScrollBars = ScrollBars.Both;
        previewTextBox.WordWrap = true;

        promptTextBox.AcceptsReturn = true;
        promptTextBox.AcceptsTab = true;
        promptTextBox.Dock = DockStyle.Fill;
        promptTextBox.Font = new Font("Consolas", 10F);
        promptTextBox.Multiline = true;
        promptTextBox.ScrollBars = ScrollBars.Both;
        promptTextBox.WordWrap = true;

        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        rootLayout.SetColumnSpan(buttonPanel, 2);

        updateSelectedButton.AutoSize = true;
        updateSelectedButton.Text = "Update Selected From AI";
        updateSelectedButton.Click += UpdateSelectedButton_Click;

        updateAllButton.AutoSize = true;
        updateAllButton.Text = "Update All From AI";
        updateAllButton.Click += UpdateAllButton_Click;

        openFolderButton.AutoSize = true;
        openFolderButton.Text = "Open WorkspaceDocs Folder";
        openFolderButton.Click += OpenFolderButton_Click;

        buttonPanel.Controls.Add(updateSelectedButton);
        buttonPanel.Controls.Add(updateAllButton);
        buttonPanel.Controls.Add(openFolderButton);

        statusLabel.Dock = DockStyle.Fill;
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        rootLayout.SetColumnSpan(statusLabel, 2);

        rootLayout.Controls.Add(titleLabel, 0, 0);
        rootLayout.Controls.Add(documentListBox, 0, 1);
        rootLayout.Controls.Add(pathTextBox, 1, 1);
        rootLayout.Controls.Add(previewTextBox, 1, 2);
        rootLayout.Controls.Add(promptTextBox, 1, 3);
        rootLayout.Controls.Add(buttonPanel, 0, 4);
        rootLayout.Controls.Add(statusLabel, 0, 5);

        Controls.Add(rootLayout);

        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        buttonPanel.ResumeLayout(false);
        buttonPanel.PerformLayout();
        ResumeLayout(false);
    }
}
