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
    private FlowLayoutPanel modePanel;
    private FlowLayoutPanel buttonPanel;
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
        rootLayout = new TableLayoutPanel();
        titleLabel = new Label();
        modePanel = new FlowLayoutPanel();
        sandboxCopyRadioButton = new RadioButton();
        productionRadioButton = new RadioButton();
        inMemoryRadioButton = new RadioButton();
        productionDatabasePathTextBox = new TextBox();
        browseProductionButton = new Button();
        openProductionFolderButton = new Button();
        sandboxDatabasePathTextBox = new TextBox();
        browseSandboxButton = new Button();
        openSandboxFolderButton = new Button();
        effectiveDatabasePathTextBox = new TextBox();
        settingsPathTextBox = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        saveSettingsButton = new Button();
        copyProductionToSandboxButton = new Button();
        testConnectionButton = new Button();
        openSettingsFolderButton = new Button();
        tableListBox = new ListBox();
        statusLabel = new Label();
        rootLayout.SuspendLayout();
        modePanel.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // rootLayout
        // 
        rootLayout.ColumnCount = 4;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        rootLayout.Controls.Add(titleLabel, 0, 0);
        rootLayout.Controls.Add(modePanel, 0, 1);
        rootLayout.Controls.Add(productionDatabasePathTextBox, 1, 2);
        rootLayout.Controls.Add(browseProductionButton, 2, 2);
        rootLayout.Controls.Add(openProductionFolderButton, 3, 2);
        rootLayout.Controls.Add(sandboxDatabasePathTextBox, 1, 3);
        rootLayout.Controls.Add(browseSandboxButton, 2, 3);
        rootLayout.Controls.Add(openSandboxFolderButton, 3, 3);
        rootLayout.Controls.Add(effectiveDatabasePathTextBox, 1, 4);
        rootLayout.Controls.Add(settingsPathTextBox, 1, 5);
        rootLayout.Controls.Add(buttonPanel, 0, 6);
        rootLayout.Controls.Add(tableListBox, 0, 8);
        rootLayout.Controls.Add(statusLabel, 0, 9);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Location = new Point(0, 0);
        rootLayout.Name = "rootLayout";
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
        rootLayout.Size = new Size(1050, 700);
        rootLayout.TabIndex = 0;
        // 
        // titleLabel
        // 
        rootLayout.SetColumnSpan(titleLabel, 4);
        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        titleLabel.Location = new Point(15, 12);
        titleLabel.Name = "titleLabel";
        titleLabel.Size = new Size(1020, 42);
        titleLabel.TabIndex = 0;
        titleLabel.Text = "Database Connection";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // modePanel
        // 
        rootLayout.SetColumnSpan(modePanel, 4);
        modePanel.Controls.Add(sandboxCopyRadioButton);
        modePanel.Controls.Add(productionRadioButton);
        modePanel.Controls.Add(inMemoryRadioButton);
        modePanel.Dock = DockStyle.Fill;
        modePanel.Location = new Point(15, 57);
        modePanel.Name = "modePanel";
        modePanel.Size = new Size(1020, 28);
        modePanel.TabIndex = 1;
        // 
        // sandboxCopyRadioButton
        // 
        sandboxCopyRadioButton.Location = new Point(3, 3);
        sandboxCopyRadioButton.Name = "sandboxCopyRadioButton";
        sandboxCopyRadioButton.Size = new Size(104, 24);
        sandboxCopyRadioButton.TabIndex = 0;
        sandboxCopyRadioButton.Text = "Sandbox Copy";
        sandboxCopyRadioButton.CheckedChanged += DatabaseMode_CheckedChanged;
        // 
        // productionRadioButton
        // 
        productionRadioButton.Location = new Point(113, 3);
        productionRadioButton.Name = "productionRadioButton";
        productionRadioButton.Size = new Size(104, 24);
        productionRadioButton.TabIndex = 1;
        productionRadioButton.Text = "Production";
        productionRadioButton.CheckedChanged += DatabaseMode_CheckedChanged;
        // 
        // inMemoryRadioButton
        // 
        inMemoryRadioButton.Location = new Point(223, 3);
        inMemoryRadioButton.Name = "inMemoryRadioButton";
        inMemoryRadioButton.Size = new Size(104, 24);
        inMemoryRadioButton.TabIndex = 2;
        inMemoryRadioButton.Text = "In-Memory";
        inMemoryRadioButton.CheckedChanged += DatabaseMode_CheckedChanged;
        // 
        // productionDatabasePathTextBox
        // 
        productionDatabasePathTextBox.Dock = DockStyle.Fill;
        productionDatabasePathTextBox.Location = new Point(165, 91);
        productionDatabasePathTextBox.Name = "productionDatabasePathTextBox";
        productionDatabasePathTextBox.Size = new Size(690, 23);
        productionDatabasePathTextBox.TabIndex = 3;
        // 
        // browseProductionButton
        // 
        browseProductionButton.Location = new Point(861, 91);
        browseProductionButton.Name = "browseProductionButton";
        browseProductionButton.Size = new Size(75, 23);
        browseProductionButton.TabIndex = 4;
        browseProductionButton.Text = "Browse";
        browseProductionButton.Click += BrowseProductionButton_Click;
        // 
        // openProductionFolderButton
        // 
        openProductionFolderButton.Location = new Point(951, 91);
        openProductionFolderButton.Name = "openProductionFolderButton";
        openProductionFolderButton.Size = new Size(75, 23);
        openProductionFolderButton.TabIndex = 5;
        openProductionFolderButton.Text = "Open";
        openProductionFolderButton.Click += OpenProductionFolderButton_Click;
        // 
        // sandboxDatabasePathTextBox
        // 
        sandboxDatabasePathTextBox.Dock = DockStyle.Fill;
        sandboxDatabasePathTextBox.Location = new Point(165, 125);
        sandboxDatabasePathTextBox.Name = "sandboxDatabasePathTextBox";
        sandboxDatabasePathTextBox.Size = new Size(690, 23);
        sandboxDatabasePathTextBox.TabIndex = 7;
        // 
        // browseSandboxButton
        // 
        browseSandboxButton.Location = new Point(861, 125);
        browseSandboxButton.Name = "browseSandboxButton";
        browseSandboxButton.Size = new Size(75, 23);
        browseSandboxButton.TabIndex = 8;
        browseSandboxButton.Text = "Browse";
        browseSandboxButton.Click += BrowseSandboxButton_Click;
        // 
        // openSandboxFolderButton
        // 
        openSandboxFolderButton.Location = new Point(951, 125);
        openSandboxFolderButton.Name = "openSandboxFolderButton";
        openSandboxFolderButton.Size = new Size(75, 23);
        openSandboxFolderButton.TabIndex = 9;
        openSandboxFolderButton.Text = "Open";
        openSandboxFolderButton.Click += OpenSandboxFolderButton_Click;
        // 
        // effectiveDatabasePathTextBox
        // 
        rootLayout.SetColumnSpan(effectiveDatabasePathTextBox, 3);
        effectiveDatabasePathTextBox.Dock = DockStyle.Fill;
        effectiveDatabasePathTextBox.Location = new Point(165, 159);
        effectiveDatabasePathTextBox.Name = "effectiveDatabasePathTextBox";
        effectiveDatabasePathTextBox.ReadOnly = true;
        effectiveDatabasePathTextBox.Size = new Size(870, 23);
        effectiveDatabasePathTextBox.TabIndex = 11;
        // 
        // settingsPathTextBox
        // 
        rootLayout.SetColumnSpan(settingsPathTextBox, 3);
        settingsPathTextBox.Dock = DockStyle.Fill;
        settingsPathTextBox.Location = new Point(165, 193);
        settingsPathTextBox.Name = "settingsPathTextBox";
        settingsPathTextBox.ReadOnly = true;
        settingsPathTextBox.Size = new Size(870, 23);
        settingsPathTextBox.TabIndex = 13;
        // 
        // buttonPanel
        // 
        rootLayout.SetColumnSpan(buttonPanel, 4);
        buttonPanel.Controls.Add(saveSettingsButton);
        buttonPanel.Controls.Add(copyProductionToSandboxButton);
        buttonPanel.Controls.Add(testConnectionButton);
        buttonPanel.Controls.Add(openSettingsFolderButton);
        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonPanel.Location = new Point(15, 227);
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Size = new Size(1020, 38);
        buttonPanel.TabIndex = 14;
        // 
        // saveSettingsButton
        // 
        saveSettingsButton.AutoSize = true;
        saveSettingsButton.Location = new Point(931, 3);
        saveSettingsButton.Name = "saveSettingsButton";
        saveSettingsButton.Size = new Size(86, 25);
        saveSettingsButton.TabIndex = 0;
        saveSettingsButton.Text = "Save Settings";
        saveSettingsButton.Click += SaveSettingsButton_Click;
        // 
        // copyProductionToSandboxButton
        // 
        copyProductionToSandboxButton.AutoSize = true;
        copyProductionToSandboxButton.Location = new Point(756, 3);
        copyProductionToSandboxButton.Name = "copyProductionToSandboxButton";
        copyProductionToSandboxButton.Size = new Size(169, 25);
        copyProductionToSandboxButton.TabIndex = 1;
        copyProductionToSandboxButton.Text = "Copy Production → Sandbox";
        copyProductionToSandboxButton.Click += CopyProductionToSandboxButton_Click;
        // 
        // testConnectionButton
        // 
        testConnectionButton.AutoSize = true;
        testConnectionButton.Location = new Point(648, 3);
        testConnectionButton.Name = "testConnectionButton";
        testConnectionButton.Size = new Size(102, 25);
        testConnectionButton.TabIndex = 2;
        testConnectionButton.Text = "Test Connection";
        testConnectionButton.Click += TestConnectionButton_Click;
        // 
        // openSettingsFolderButton
        // 
        openSettingsFolderButton.AutoSize = true;
        openSettingsFolderButton.Location = new Point(515, 3);
        openSettingsFolderButton.Name = "openSettingsFolderButton";
        openSettingsFolderButton.Size = new Size(127, 25);
        openSettingsFolderButton.TabIndex = 3;
        openSettingsFolderButton.Text = "Open Settings Folder";
        openSettingsFolderButton.Click += OpenSettingsFolderButton_Click;
        // 
        // tableListBox
        // 
        rootLayout.SetColumnSpan(tableListBox, 4);
        tableListBox.Dock = DockStyle.Fill;
        tableListBox.ItemHeight = 15;
        tableListBox.Location = new Point(15, 299);
        tableListBox.Name = "tableListBox";
        tableListBox.Size = new Size(1020, 358);
        tableListBox.TabIndex = 16;
        // 
        // statusLabel
        // 
        rootLayout.SetColumnSpan(statusLabel, 4);
        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Location = new Point(15, 660);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(1020, 28);
        statusLabel.TabIndex = 17;
        // 
        // DatabaseConnectionPanelForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1050, 700);
        Controls.Add(rootLayout);
        Name = "DatabaseConnectionPanelForm";
        Text = "Database Connection";
        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        modePanel.ResumeLayout(false);
        buttonPanel.ResumeLayout(false);
        buttonPanel.PerformLayout();
        ResumeLayout(false);
    }
}