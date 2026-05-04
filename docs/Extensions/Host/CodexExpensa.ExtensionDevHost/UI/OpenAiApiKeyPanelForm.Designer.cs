namespace CodexExpensa.ExtensionDevHost.UI;

partial class OpenAiApiKeyPanelForm
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel rootLayout;
    private Label titleLabel;
    private Label keyNameLabel;
    private TextBox keyNameTextBox;
    private Label apiKeyLabel;
    private TextBox apiKeyTextBox;
    private Button showHideButton;
    private Label settingsPathLabel;
    private TextBox settingsPathTextBox;
    private Label noteLabel;
    private FlowLayoutPanel buttonPanel;
    private Button saveButton;
    private Button reloadButton;
    private Button testButton;
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
        keyNameLabel = new Label();
        keyNameTextBox = new TextBox();
        apiKeyLabel = new Label();
        apiKeyTextBox = new TextBox();
        showHideButton = new Button();
        settingsPathLabel = new Label();
        settingsPathTextBox = new TextBox();
        noteLabel = new Label();
        buttonPanel = new FlowLayoutPanel();
        saveButton = new Button();
        reloadButton = new Button();
        testButton = new Button();
        statusLabel = new Label();

        rootLayout.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 420);
        Name = "OpenAiApiKeyPanelForm";
        Text = "OpenAI API Key";

        rootLayout.ColumnCount = 3;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Padding = new Padding(14);
        rootLayout.RowCount = 7;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));

        titleLabel.AutoSize = true;
        rootLayout.SetColumnSpan(titleLabel, 3);
        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        titleLabel.Text = "OpenAI API Key";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;

        keyNameLabel.AutoSize = true;
        keyNameLabel.Dock = DockStyle.Fill;
        keyNameLabel.Text = "Key Name";
        keyNameLabel.TextAlign = ContentAlignment.MiddleLeft;

        keyNameTextBox.Dock = DockStyle.Fill;

        apiKeyLabel.AutoSize = true;
        apiKeyLabel.Dock = DockStyle.Fill;
        apiKeyLabel.Text = "API Key";
        apiKeyLabel.TextAlign = ContentAlignment.MiddleLeft;

        apiKeyTextBox.Dock = DockStyle.Fill;
        apiKeyTextBox.UseSystemPasswordChar = true;

        showHideButton.Dock = DockStyle.Fill;
        showHideButton.Text = "Show";
        showHideButton.Click += ShowHideButton_Click;

        settingsPathLabel.AutoSize = true;
        settingsPathLabel.Dock = DockStyle.Fill;
        settingsPathLabel.Text = "Settings File";
        settingsPathLabel.TextAlign = ContentAlignment.MiddleLeft;

        settingsPathTextBox.Dock = DockStyle.Fill;
        settingsPathTextBox.ReadOnly = true;
        rootLayout.SetColumnSpan(settingsPathTextBox, 2);

        noteLabel.Dock = DockStyle.Fill;
        rootLayout.SetColumnSpan(noteLabel, 3);
        noteLabel.Text = "The key name is only a local label. The API key is stored in the existing local OpenAI settings file. This panel replaces the old popup dialog for tree navigation.";
        noteLabel.TextAlign = ContentAlignment.TopLeft;

        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        rootLayout.SetColumnSpan(buttonPanel, 3);

        saveButton.AutoSize = true;
        saveButton.Text = "Save";
        saveButton.Click += SaveButton_Click;

        reloadButton.AutoSize = true;
        reloadButton.Text = "Reload";
        reloadButton.Click += ReloadButton_Click;

        testButton.AutoSize = true;
        testButton.Text = "Test Key";
        testButton.Click += TestButton_Click;

        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(reloadButton);
        buttonPanel.Controls.Add(testButton);

        statusLabel.Dock = DockStyle.Fill;
        rootLayout.SetColumnSpan(statusLabel, 3);
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;

        rootLayout.Controls.Add(titleLabel, 0, 0);
        rootLayout.Controls.Add(keyNameLabel, 0, 1);
        rootLayout.Controls.Add(keyNameTextBox, 1, 1);
        rootLayout.SetColumnSpan(keyNameTextBox, 2);
        rootLayout.Controls.Add(apiKeyLabel, 0, 2);
        rootLayout.Controls.Add(apiKeyTextBox, 1, 2);
        rootLayout.Controls.Add(showHideButton, 2, 2);
        rootLayout.Controls.Add(settingsPathLabel, 0, 3);
        rootLayout.Controls.Add(settingsPathTextBox, 1, 3);
        rootLayout.Controls.Add(noteLabel, 0, 4);
        rootLayout.Controls.Add(buttonPanel, 0, 5);
        rootLayout.Controls.Add(statusLabel, 0, 6);

        Controls.Add(rootLayout);

        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        buttonPanel.ResumeLayout(false);
        buttonPanel.PerformLayout();
        ResumeLayout(false);
    }
}
