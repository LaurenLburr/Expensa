namespace CodexExpensa.ExtensionDevHost.UI;

partial class FolderWatcherAutoUnzipForm
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel rootLayout;
    private Label titleLabel;
    private Label watchFolderLabel;
    private TextBox watchFolderTextBox;
    private Button browseWatchFolderButton;
    private Button openWatchFolderButton;
    private Label extractFolderLabel;
    private TextBox extractFolderTextBox;
    private Button browseExtractFolderButton;
    private Button openExtractFolderButton;
    private FlowLayoutPanel buttonPanel;
    private Button startButton;
    private Button stopButton;
    private Button importExistingButton;
    private Button openSettingsFolderButton;
    private TextBox logTextBox;
    private Label statusLabel;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        rootLayout = new TableLayoutPanel();
        titleLabel = new Label();
        watchFolderLabel = new Label();
        watchFolderTextBox = new TextBox();
        browseWatchFolderButton = new Button();
        openWatchFolderButton = new Button();
        extractFolderLabel = new Label();
        extractFolderTextBox = new TextBox();
        browseExtractFolderButton = new Button();
        openExtractFolderButton = new Button();
        buttonPanel = new FlowLayoutPanel();
        startButton = new Button();
        stopButton = new Button();
        importExistingButton = new Button();
        openSettingsFolderButton = new Button();
        logTextBox = new TextBox();
        statusLabel = new Label();

        rootLayout.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 620);
        Name = "FolderWatcherAutoUnzipForm";
        Text = "Folder Watcher / Auto Unzip";

        rootLayout.ColumnCount = 4;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Padding = new Padding(12);
        rootLayout.RowCount = 6;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));

        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        titleLabel.Text = "Folder Watcher / Auto Unzip";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        rootLayout.SetColumnSpan(titleLabel, 4);

        watchFolderLabel.Dock = DockStyle.Fill;
        watchFolderLabel.Text = "Watch Folder";
        watchFolderLabel.TextAlign = ContentAlignment.MiddleLeft;

        watchFolderTextBox.Dock = DockStyle.Fill;

        browseWatchFolderButton.Dock = DockStyle.Fill;
        browseWatchFolderButton.Text = "Browse";
        browseWatchFolderButton.Click += BrowseWatchFolderButton_Click;

        openWatchFolderButton.Dock = DockStyle.Fill;
        openWatchFolderButton.Text = "Open";
        openWatchFolderButton.Click += OpenWatchFolderButton_Click;

        extractFolderLabel.Dock = DockStyle.Fill;
        extractFolderLabel.Text = "Extract To";
        extractFolderLabel.TextAlign = ContentAlignment.MiddleLeft;

        extractFolderTextBox.Dock = DockStyle.Fill;

        browseExtractFolderButton.Dock = DockStyle.Fill;
        browseExtractFolderButton.Text = "Browse";
        browseExtractFolderButton.Click += BrowseExtractFolderButton_Click;

        openExtractFolderButton.Dock = DockStyle.Fill;
        openExtractFolderButton.Text = "Open";
        openExtractFolderButton.Click += OpenExtractFolderButton_Click;

        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        rootLayout.SetColumnSpan(buttonPanel, 4);

        startButton.AutoSize = true;
        startButton.Text = "Start Watching";
        startButton.Click += StartButton_Click;

        stopButton.AutoSize = true;
        stopButton.Enabled = false;
        stopButton.Text = "Stop";
        stopButton.Click += StopButton_Click;

        importExistingButton.AutoSize = true;
        importExistingButton.Text = "Import Existing Zips";
        importExistingButton.Click += ImportExistingButton_Click;

        openSettingsFolderButton.AutoSize = true;
        openSettingsFolderButton.Text = "Open Settings Folder";
        openSettingsFolderButton.Click += OpenSettingsFolderButton_Click;

        buttonPanel.Controls.Add(startButton);
        buttonPanel.Controls.Add(stopButton);
        buttonPanel.Controls.Add(importExistingButton);
        buttonPanel.Controls.Add(openSettingsFolderButton);

        logTextBox.Dock = DockStyle.Fill;
        logTextBox.Multiline = true;
        logTextBox.ReadOnly = true;
        logTextBox.ScrollBars = ScrollBars.Both;
        logTextBox.Font = new Font("Consolas", 10F);
        rootLayout.SetColumnSpan(logTextBox, 4);

        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Text = "Stopped";
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        rootLayout.SetColumnSpan(statusLabel, 4);

        rootLayout.Controls.Add(titleLabel, 0, 0);
        rootLayout.Controls.Add(watchFolderLabel, 0, 1);
        rootLayout.Controls.Add(watchFolderTextBox, 1, 1);
        rootLayout.Controls.Add(browseWatchFolderButton, 2, 1);
        rootLayout.Controls.Add(openWatchFolderButton, 3, 1);
        rootLayout.Controls.Add(extractFolderLabel, 0, 2);
        rootLayout.Controls.Add(extractFolderTextBox, 1, 2);
        rootLayout.Controls.Add(browseExtractFolderButton, 2, 2);
        rootLayout.Controls.Add(openExtractFolderButton, 3, 2);
        rootLayout.Controls.Add(buttonPanel, 0, 3);
        rootLayout.Controls.Add(logTextBox, 0, 4);
        rootLayout.Controls.Add(statusLabel, 0, 5);

        Controls.Add(rootLayout);

        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        buttonPanel.ResumeLayout(false);
        buttonPanel.PerformLayout();
        ResumeLayout(false);
    }
}
