namespace CodexExpensa.ExtensionDevHost.UI;

partial class DatabaseConnectionPanelForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayout;
    private Label titleLabel;
    private RadioButton sandboxCopyRadioButton;
    private RadioButton productionRadioButton;
    private RadioButton inMemoryRadioButton;
    private TextBox productionDatabasePathTextBox;
    private TextBox sandboxDatabasePathTextBox;
    private TextBox effectiveDatabasePathTextBox;
    private TextBox settingsPathTextBox;
    private Button browseProductionButton;
    private Button openProductionFolderButton;
    private Button browseSandboxButton;
    private Button openSandboxFolderButton;
    private Button saveSettingsButton;
    private Button copyProductionToSandboxButton;
    private Button testConnectionButton;
    private Button openSettingsFolderButton;
    private ListBox tableListBox;
    private Label statusLabel;

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
        rootLayout = new TableLayoutPanel();
        titleLabel = new Label();
        sandboxCopyRadioButton = new RadioButton();
        productionRadioButton = new RadioButton();
        inMemoryRadioButton = new RadioButton();
        productionDatabasePathTextBox = new TextBox();
        sandboxDatabasePathTextBox = new TextBox();
        effectiveDatabasePathTextBox = new TextBox();
        settingsPathTextBox = new TextBox();
        browseProductionButton = new Button();
        openProductionFolderButton = new Button();
        browseSandboxButton = new Button();
        openSandboxFolderButton = new Button();
        saveSettingsButton = new Button();
        copyProductionToSandboxButton = new Button();
        testConnectionButton = new Button();
        openSettingsFolderButton = new Button();
        tableListBox = new ListBox();
        statusLabel = new Label();

        SuspendLayout();

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1050, 700);
        Name = "DatabaseConnectionPanelForm";
        Text = "Database Connection";

        rootLayout.ColumnCount = 4;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Padding = new Padding(12);
        rootLayout.RowCount = 10;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));

        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        titleLabel.Text = "Database Connection";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        rootLayout.SetColumnSpan(titleLabel, 4);

        sandboxCopyRadioButton.Text = "Sandbox Copy";
        sandboxCopyRadioButton.CheckedChanged += DatabaseMode_CheckedChanged;
        productionRadioButton.Text = "Production";
        productionRadioButton.CheckedChanged += DatabaseMode_CheckedChanged;
        inMemoryRadioButton.Text = "In-Memory";
        inMemoryRadioButton.CheckedChanged += DatabaseMode_CheckedChanged;

        productionDatabasePathTextBox.Dock = DockStyle.Fill;
        sandboxDatabasePathTextBox.Dock = DockStyle.Fill;
        effectiveDatabasePathTextBox.Dock = DockStyle.Fill;
        effectiveDatabasePathTextBox.ReadOnly = true;
        settingsPathTextBox.Dock = DockStyle.Fill;
        settingsPathTextBox.ReadOnly = true;

        browseProductionButton.Text = "Browse";
        browseProductionButton.Click += BrowseProductionButton_Click;
        openProductionFolderButton.Text = "Open";
        openProductionFolderButton.Click += OpenProductionFolderButton_Click;
        browseSandboxButton.Text = "Browse";
        browseSandboxButton.Click += BrowseSandboxButton_Click;
        openSandboxFolderButton.Text = "Open";
        openSandboxFolderButton.Click += OpenSandboxFolderButton_Click;

        saveSettingsButton.Text = "Save Settings";
        saveSettingsButton.AutoSize = true;
        saveSettingsButton.Click += SaveSettingsButton_Click;
        copyProductionToSandboxButton.Text = "Copy Production → Sandbox";
        copyProductionToSandboxButton.AutoSize = true;
        copyProductionToSandboxButton.Click += CopyProductionToSandboxButton_Click;
        testConnectionButton.Text = "Test Connection";
        testConnectionButton.AutoSize = true;
        testConnectionButton.Click += TestConnectionButton_Click;
        openSettingsFolderButton.Text = "Open Settings Folder";
        openSettingsFolderButton.AutoSize = true;
        openSettingsFolderButton.Click += OpenSettingsFolderButton_Click;

        FlowLayoutPanel modePanel = new() { Dock = DockStyle.Fill };
        modePanel.Controls.Add(sandboxCopyRadioButton);
        modePanel.Controls.Add(productionRadioButton);
        modePanel.Controls.Add(inMemoryRadioButton);
        rootLayout.SetColumnSpan(modePanel, 4);

        FlowLayoutPanel buttonPanel = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        buttonPanel.Controls.Add(saveSettingsButton);
        buttonPanel.Controls.Add(copyProductionToSandboxButton);
        buttonPanel.Controls.Add(testConnectionButton);
        buttonPanel.Controls.Add(openSettingsFolderButton);
        rootLayout.SetColumnSpan(buttonPanel, 4);

        tableListBox.Dock = DockStyle.Fill;
        statusLabel.Dock = DockStyle.Fill;

        rootLayout.Controls.Add(titleLabel, 0, 0);
        rootLayout.Controls.Add(modePanel, 0, 1);
        rootLayout.Controls.Add(new Label { Text = "Production DB", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        rootLayout.Controls.Add(productionDatabasePathTextBox, 1, 2);
        rootLayout.Controls.Add(browseProductionButton, 2, 2);
        rootLayout.Controls.Add(openProductionFolderButton, 3, 2);
        rootLayout.Controls.Add(new Label { Text = "Sandbox DB", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        rootLayout.Controls.Add(sandboxDatabasePathTextBox, 1, 3);
        rootLayout.Controls.Add(browseSandboxButton, 2, 3);
        rootLayout.Controls.Add(openSandboxFolderButton, 3, 3);
        rootLayout.Controls.Add(new Label { Text = "Effective DB", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
        rootLayout.Controls.Add(effectiveDatabasePathTextBox, 1, 4);
        rootLayout.SetColumnSpan(effectiveDatabasePathTextBox, 3);
        rootLayout.Controls.Add(new Label { Text = "Settings", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 5);
        rootLayout.Controls.Add(settingsPathTextBox, 1, 5);
        rootLayout.SetColumnSpan(settingsPathTextBox, 3);
        rootLayout.Controls.Add(buttonPanel, 0, 6);
        rootLayout.Controls.Add(new Label { Text = "Tables", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 7);
        rootLayout.SetColumnSpan(tableListBox, 4);
        rootLayout.Controls.Add(tableListBox, 0, 8);
        rootLayout.SetColumnSpan(statusLabel, 4);
        rootLayout.Controls.Add(statusLabel, 0, 9);

        Controls.Add(rootLayout);
        ResumeLayout(false);
    }
}