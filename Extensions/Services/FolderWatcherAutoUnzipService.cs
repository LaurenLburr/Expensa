using System.IO.Compression;

namespace CodexExpensa.ExtensionDevHost.Services;

public sealed class FolderWatcherAutoUnzipService : IDisposable
{
    private readonly object _syncRoot = new();
    private readonly HashSet<string> _processingFiles = new(StringComparer.OrdinalIgnoreCase);

    private FileSystemWatcher? _watcher;
    private bool _disposed;

    public event EventHandler<ZipImportedEventArgs>? ZipImported;

    public event EventHandler<ZipImportFailedEventArgs>? ZipImportFailed;

    public bool IsRunning => _watcher is not null;

    public void Start(string watchFolder, string extractRootFolder)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(watchFolder);
        ArgumentException.ThrowIfNullOrWhiteSpace(extractRootFolder);

        Stop();

        Directory.CreateDirectory(watchFolder);
        Directory.CreateDirectory(extractRootFolder);

        _watcher = new FileSystemWatcher(watchFolder, "*.zip")
        {
            IncludeSubdirectories = false,
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime
        };

        _watcher.Created += (_, e) => QueueImport(e.FullPath, extractRootFolder);
        _watcher.Changed += (_, e) => QueueImport(e.FullPath, extractRootFolder);
        _watcher.Renamed += (_, e) => QueueImport(e.FullPath, extractRootFolder);

        foreach (string zipPath in Directory.GetFiles(watchFolder, "*.zip"))
        {
            QueueImport(zipPath, extractRootFolder);
        }
    }

    public void Stop()
    {
        if (_watcher is null)
        {
            return;
        }

        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        _watcher = null;
    }

    public void ImportExistingZips(string watchFolder, string extractRootFolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(watchFolder);
        ArgumentException.ThrowIfNullOrWhiteSpace(extractRootFolder);

        Directory.CreateDirectory(watchFolder);
        Directory.CreateDirectory(extractRootFolder);

        foreach (string zipPath in Directory.GetFiles(watchFolder, "*.zip"))
        {
            QueueImport(zipPath, extractRootFolder);
        }
    }

    private void QueueImport(string zipPath, string extractRootFolder)
    {
        if (string.IsNullOrWhiteSpace(zipPath) || !File.Exists(zipPath))
        {
            return;
        }

        lock (_syncRoot)
        {
            if (!_processingFiles.Add(zipPath))
            {
                return;
            }
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await WaitForFileReadyAsync(zipPath).ConfigureAwait(false);
                string outputFolder = ImportZip(zipPath, extractRootFolder);

                ZipImported?.Invoke(
                    this,
                    new ZipImportedEventArgs(zipPath, outputFolder, DateTime.Now));
            }
            catch (Exception ex)
            {
                ZipImportFailed?.Invoke(
                    this,
                    new ZipImportFailedEventArgs(zipPath, ex.Message, DateTime.Now));
            }
            finally
            {
                lock (_syncRoot)
                {
                    _processingFiles.Remove(zipPath);
                }
            }
        });
    }

    private static async Task WaitForFileReadyAsync(string path)
    {
        const int maxAttempts = 20;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using FileStream stream = new(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None);

                if (stream.Length > 0)
                {
                    return;
                }
            }
            catch (IOException)
            {
                // File is still being copied.
            }
            catch (UnauthorizedAccessException)
            {
                // File is still being copied or locked.
            }

            await Task.Delay(250).ConfigureAwait(false);
        }

        throw new IOException($"File was not ready for import: {path}");
    }

    private static string ImportZip(string zipPath, string extractRootFolder)
    {
        string zipName = Path.GetFileNameWithoutExtension(zipPath);
        string safeFolderName = SanitizeFolderName(zipName);

        string destinationFolder = Path.Combine(extractRootFolder, safeFolderName);

        if (Directory.Exists(destinationFolder))
        {
            Directory.Delete(destinationFolder, recursive: true);
        }

        Directory.CreateDirectory(destinationFolder);

        ZipFile.ExtractToDirectory(zipPath, destinationFolder, overwriteFiles: true);

        return destinationFolder;
    }

    private static string SanitizeFolderName(string name)
    {
        char[] invalid = Path.GetInvalidFileNameChars();

        string cleaned = new(name
            .Select(ch => invalid.Contains(ch) ? '_' : ch)
            .ToArray());

        return string.IsNullOrWhiteSpace(cleaned)
            ? "ImportedZip"
            : cleaned.Trim();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Stop();
        _disposed = true;
    }
}

public sealed record ZipImportedEventArgs(
    string ZipPath,
    string OutputFolder,
    DateTime ImportedAt);

public sealed record ZipImportFailedEventArgs(
    string ZipPath,
    string ErrorMessage,
    DateTime FailedAt)
{

};

