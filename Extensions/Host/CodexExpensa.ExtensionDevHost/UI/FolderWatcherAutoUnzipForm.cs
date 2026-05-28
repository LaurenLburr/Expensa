using System.Text.Json;
using CodexExpensa.ExtensionDevHost.Services;

namespace CodexExpensa.ExtensionDevHost.UI;

public partial class FolderWatcherAutoUnzipForm : Form
{
    private const string DefaultWatchFolder = @"D:\Git\CodexExpensa\Extensions\AI_Replies";
    private const string DefaultExtractFolder = @"D:\Git\CodexExpensa";

    private readonly FolderWatcherAutoUnzipService _watcher = new();
    private readonly string _settingsPath;

    private bool _settingsLoaded;

    public FolderWatcherAutoUnzipForm()
    {
        InitializeComponent();

        _settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "FolderWatcherAutoUnzipSettings.json");

        WirePersistenceEvents();
        LoadSettings();

        _watcher.ZipImported += Watcher_ZipImported;
        _watcher.ZipImportFailed += Watcher_ZipImportFailed;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SaveSettings();
            _watcher.Dispose();
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void WirePersistenceEvents()
    {
        watchFolderTextBox.TextChanged += (_, _) => SaveSettingsIfReady();
        extractFolderTextBox.TextChanged += (_, _) => SaveSettingsIfReady();

        watchFolderTextBox.Leave += (_, _) => SaveSettings();
        extractFolderTextBox.Leave += (_, _) => SaveSettings();

        FormClosing += (_, _) => SaveSettings();
    }

    private void LoadSettings()
    {
        FolderWatcherAutoUnzipSettings settings = ReadSettings();

        watchFolderTextBox.Text = string.IsNullOrWhiteSpace(settings.WatchFolder)
            ? DefaultWatchFolder
            : settings.WatchFolder;

        extractFolderTextBox.Text = string.IsNullOrWhiteSpace(settings.ExtractFolder)
            ? DefaultExtractFolder
            : settings.ExtractFolder;

        _settingsLoaded = true;
        SaveSettings();

        AppendLog($"Settings loaded from: {_settingsPath}");
    }

    private FolderWatcherAutoUnzipSettings ReadSettings()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return new FolderWatcherAutoUnzipSettings();
            }

            string json = File.ReadAllText(_settingsPath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new FolderWatcherAutoUnzipSettings();
            }

            return JsonSerializer.Deserialize<FolderWatcherAutoUnzipSettings>(json)
                ?? new FolderWatcherAutoUnzipSettings();
        }
        catch
        {
            return new FolderWatcherAutoUnzipSettings();
        }
    }

    private void SaveSettingsIfReady()
    {
        if (!_settingsLoaded)
        {
            return;
        }

        SaveSettings();
    }

    private void SaveSettings()
    {
        if (!_settingsLoaded)
        {
            return;
        }

        string watchFolder = watchFolderTextBox.Text.Trim();
        string extractFolder = extractFolderTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(watchFolder))
        {
            watchFolder = DefaultWatchFolder;
            watchFolderTextBox.Text = watchFolder;
        }

        if (string.IsNullOrWhiteSpace(extractFolder))
        {
            extractFolder = DefaultExtractFolder;
            extractFolderTextBox.Text = extractFolder;
        }

        try
        {
            string? folder = Path.GetDirectoryName(_settingsPath);
            if (!string.IsNullOrWhiteSpace(folder))
            {
                Directory.CreateDirectory(folder);
            }

            FolderWatcherAutoUnzipSettings settings = new()
            {
                WatchFolder = watchFolder,
                ExtractFolder = extractFolder
            };

            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_settingsPath, json);
        }
        catch (Exception ex)
        {
            AppendLog($"Settings save failed: {ex.Message}");
        }
    }

    private void StartButton_Click(object? sender, EventArgs e)
    {
        try
        {
            SaveSettings();
            _watcher.Start(watchFolderTextBox.Text, extractFolderTextBox.Text);
            SetRunningState(true);
            AppendLog("Watcher started.");
            AppendLog($"Watch folder:   {watchFolderTextBox.Text}");
            AppendLog($"Extract folder: {extractFolderTextBox.Text}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Folder Watcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void StopButton_Click(object? sender, EventArgs e)
    {
        _watcher.Stop();
        SaveSettings();
        SetRunningState(false);
        AppendLog("Watcher stopped.");
    }

    private void ImportExistingButton_Click(object? sender, EventArgs e)
    {
        try
        {
            SaveSettings();
            _watcher.ImportExistingZips(watchFolderTextBox.Text, extractFolderTextBox.Text);
            AppendLog("Queued existing zip files for import.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Folder Watcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenWatchFolderButton_Click(object? sender, EventArgs e)
    {
        SaveSettings();
        OpenFolder(watchFolderTextBox.Text);
    }

    private void OpenExtractFolderButton_Click(object? sender, EventArgs e)
    {
        SaveSettings();
        OpenFolder(extractFolderTextBox.Text);
    }

    private void OpenSettingsFolderButton_Click(object? sender, EventArgs e)
    {
        SaveSettings();

        string? settingsFolder = Path.GetDirectoryName(_settingsPath);
        if (string.IsNullOrWhiteSpace(settingsFolder))
        {
            return;
        }

        OpenFolder(settingsFolder);
    }

    private void BrowseWatchFolderButton_Click(object? sender, EventArgs e)
    {
        BrowseFolder(watchFolderTextBox);
        SaveSettings();
    }

    private void BrowseExtractFolderButton_Click(object? sender, EventArgs e)
    {
        BrowseFolder(extractFolderTextBox);
        SaveSettings();
    }

    private void Watcher_ZipImported(object? sender, ZipImportedEventArgs e)
    {
        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        BeginInvoke(new MethodInvoker(() =>
        {
            AppendLog($"Imported and moved zip to: {e.ZipPath}");
            AppendLog($"Extracted to: {e.OutputFolder}");
        }));
    }

    private void Watcher_ZipImportFailed(object? sender, ZipImportFailedEventArgs e)
    {
        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        BeginInvoke(new MethodInvoker(() =>
        {
            AppendLog($"FAILED and moved zip to: {e.ZipPath}");
            AppendLog($"Error: {e.ErrorMessage}");
        }));
    }

    private void SetRunningState(bool isRunning)
    {
        startButton.Enabled = !isRunning;
        stopButton.Enabled = isRunning;
        statusLabel.Text = isRunning ? "Running" : "Stopped";
    }

    private void AppendLog(string message)
    {
        if (logTextBox.IsDisposed)
        {
            return;
        }

        logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }

    private static void BrowseFolder(TextBox target)
    {
        using FolderBrowserDialog dialog = new()
        {
            SelectedPath = Directory.Exists(target.Text)
                ? target.Text
                : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            target.Text = dialog.SelectedPath;
        }
    }

    private static void OpenFolder(string folder)
    {
        Directory.CreateDirectory(folder);

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = folder,
            UseShellExecute = true
        });
    }

    private sealed class FolderWatcherAutoUnzipSettings
    {
        public string WatchFolder { get; set; } = string.Empty;

        public string ExtractFolder { get; set; } = string.Empty;
    }
}
