using CodexExpensa.ExtensionDevHost.Services;

namespace CodexExpensa.ExtensionDevHost.UI;

public partial class FolderWatcherAutoUnzipForm : Form
{
    private readonly FolderWatcherAutoUnzipService _watcher = new();

    public FolderWatcherAutoUnzipForm()
    {
        InitializeComponent();

        watchFolderTextBox.Text = GetDefaultWatchFolder();
        extractFolderTextBox.Text = GetDefaultExtractFolder();

        _watcher.ZipImported += Watcher_ZipImported;
        _watcher.ZipImportFailed += Watcher_ZipImportFailed;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _watcher.Dispose();
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private static string GetDefaultWatchFolder()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "UploadWatch");
    }

    private static string GetDefaultExtractFolder()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "ImportedUploads");
    }

    private void StartButton_Click(object? sender, EventArgs e)
    {
        try
        {
            _watcher.Start(watchFolderTextBox.Text, extractFolderTextBox.Text);
            SetRunningState(true);
            AppendLog("Watcher started.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Folder Watcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void StopButton_Click(object? sender, EventArgs e)
    {
        _watcher.Stop();
        SetRunningState(false);
        AppendLog("Watcher stopped.");
    }

    private void ImportExistingButton_Click(object? sender, EventArgs e)
    {
        try
        {
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
        OpenFolder(watchFolderTextBox.Text);
    }

    private void OpenExtractFolderButton_Click(object? sender, EventArgs e)
    {
        OpenFolder(extractFolderTextBox.Text);
    }

    private void BrowseWatchFolderButton_Click(object? sender, EventArgs e)
    {
        BrowseFolder(watchFolderTextBox);
    }

    private void BrowseExtractFolderButton_Click(object? sender, EventArgs e)
    {
        BrowseFolder(extractFolderTextBox);
    }

    private void Watcher_ZipImported(object? sender, ZipImportedEventArgs e)
    {
        BeginInvoke(new MethodInvoker(() =>
        {
            AppendLog($"Imported: {e.ZipPath}");
            AppendLog($"Output:   {e.OutputFolder}");
        }));
    }

    private void Watcher_ZipImportFailed(object? sender, ZipImportFailedEventArgs e)
    {
        BeginInvoke(new MethodInvoker(() =>
        {
            AppendLog($"FAILED: {e.ZipPath}");
            AppendLog($"Error:  {e.ErrorMessage}");
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
}
