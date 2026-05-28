using System.IO.Compression;

namespace CodexExpensa.ExtensionDevHost.Services;

public sealed class FolderWatcherAutoUnzipService : IDisposable
{
    private readonly object _syncRoot = new();
    private readonly HashSet<string> _processingFiles = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _completedSourceFiles = new(StringComparer.OrdinalIgnoreCase);

    private FileSystemWatcher? _watcher;
    private bool _disposed;

    public event EventHandler<ZipImportedEventArgs>? ZipImported;

    public event EventHandler<ZipImportFailedEventArgs>? ZipImportFailed;

    public bool IsRunning => _watcher is not null;

    public void Start(string watchFolder, string extractFolder)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(watchFolder);
        ArgumentException.ThrowIfNullOrWhiteSpace(extractFolder);

        Stop();

        Directory.CreateDirectory(watchFolder);
        Directory.CreateDirectory(extractFolder);
        Directory.CreateDirectory(GetProcessedFolder(watchFolder));
        Directory.CreateDirectory(GetFailedFolder(watchFolder));

        _watcher = new FileSystemWatcher(watchFolder, "*.zip")
        {
            IncludeSubdirectories = false,
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime
        };

        _watcher.Created += (_, e) => QueueImport(e.FullPath, extractFolder);
        _watcher.Changed += (_, e) => QueueImport(e.FullPath, extractFolder);
        _watcher.Renamed += (_, e) => QueueImport(e.FullPath, extractFolder);
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

    public void ImportExistingZips(string watchFolder, string extractFolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(watchFolder);
        ArgumentException.ThrowIfNullOrWhiteSpace(extractFolder);

        Directory.CreateDirectory(watchFolder);
        Directory.CreateDirectory(extractFolder);
        Directory.CreateDirectory(GetProcessedFolder(watchFolder));
        Directory.CreateDirectory(GetFailedFolder(watchFolder));

        foreach (string zipPath in Directory.GetFiles(watchFolder, "*.zip", SearchOption.TopDirectoryOnly))
        {
            QueueImport(zipPath, extractFolder);
        }
    }

    private void QueueImport(string zipPath, string extractFolder)
    {
        if (string.IsNullOrWhiteSpace(zipPath))
        {
            return;
        }

        string fullZipPath = Path.GetFullPath(zipPath);

        string? parentFolder = Path.GetDirectoryName(fullZipPath);
        if (string.IsNullOrWhiteSpace(parentFolder))
        {
            return;
        }

        if (IsInsideCleanupFolder(fullZipPath, parentFolder))
        {
            return;
        }

        lock (_syncRoot)
        {
            if (_completedSourceFiles.Contains(fullZipPath))
            {
                return;
            }

            if (!_processingFiles.Add(fullZipPath))
            {
                return;
            }
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await WaitForFileReadyAsync(fullZipPath).ConfigureAwait(false);

                if (!File.Exists(fullZipPath))
                {
                    return;
                }

                ImportZip(fullZipPath, extractFolder);

                string processedPath = MoveZipToCleanupFolder(
                    fullZipPath,
                    GetProcessedFolder(parentFolder));

                lock (_syncRoot)
                {
                    _completedSourceFiles.Add(fullZipPath);
                }

                ZipImported?.Invoke(
                    this,
                    new ZipImportedEventArgs(processedPath, extractFolder, DateTime.Now));
            }
            catch (FileNotFoundException)
            {
                // Duplicate file-system event after another event already moved the zip.
            }
            catch (DirectoryNotFoundException)
            {
                // Duplicate file-system event after cleanup/move.
            }
            catch (IOException ex) when (!File.Exists(fullZipPath))
            {
                // Duplicate stale event after the zip was moved to Processed.
            }
            catch (Exception ex)
            {
                string failedPath = fullZipPath;

                try
                {
                    if (File.Exists(fullZipPath))
                    {
                        failedPath = MoveZipToCleanupFolder(
                            fullZipPath,
                            GetFailedFolder(parentFolder));
                    }
                }
                catch
                {
                    // Do not hide the original import failure with cleanup failure.
                }

                ZipImportFailed?.Invoke(
                    this,
                    new ZipImportFailedEventArgs(failedPath, ex.Message, DateTime.Now));
            }
            finally
            {
                lock (_syncRoot)
                {
                    _processingFiles.Remove(fullZipPath);
                }
            }
        });
    }

    private static async Task WaitForFileReadyAsync(string path)
    {
        const int maxAttempts = 20;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Zip file no longer exists. It may have already been processed.", path);
            }

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

    private static void ImportZip(string zipPath, string extractFolder)
    {
        Directory.CreateDirectory(extractFolder);

        string extractRoot = Path.GetFullPath(extractFolder);

        using ZipArchive archive = ZipFile.OpenRead(zipPath);

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            string relativePath = NormalizeEntryPath(entry.FullName);

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                continue;
            }

            string destinationPath = Path.GetFullPath(Path.Combine(extractRoot, relativePath));

            if (!IsPathInsideFolder(destinationPath, extractRoot))
            {
                throw new InvalidOperationException($"Zip entry would extract outside the selected folder: {entry.FullName}");
            }

            if (string.IsNullOrEmpty(entry.Name))
            {
                Directory.CreateDirectory(destinationPath);
                continue;
            }

            string? destinationDirectory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrWhiteSpace(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            entry.ExtractToFile(destinationPath, overwrite: true);
        }
    }

    private static string NormalizeEntryPath(string path)
    {
        return path
            .Replace('\\', '/')
            .TrimStart('/');
    }

    private static bool IsPathInsideFolder(string path, string folder)
    {
        string normalizedFolder = Path.GetFullPath(folder)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        string normalizedPath = Path.GetFullPath(path);

        return normalizedPath.StartsWith(normalizedFolder, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsInsideCleanupFolder(string zipPath, string watchFolder)
    {
        string processedFolder = GetProcessedFolder(watchFolder);
        string failedFolder = GetFailedFolder(watchFolder);
        string fullZipPath = Path.GetFullPath(zipPath);

        return fullZipPath.StartsWith(
                   Path.GetFullPath(processedFolder),
                   StringComparison.OrdinalIgnoreCase) ||
               fullZipPath.StartsWith(
                   Path.GetFullPath(failedFolder),
                   StringComparison.OrdinalIgnoreCase);
    }

    private static string MoveZipToCleanupFolder(string zipPath, string cleanupFolder)
    {
        if (!File.Exists(zipPath))
        {
            return zipPath;
        }

        Directory.CreateDirectory(cleanupFolder);

        string destinationPath = Path.Combine(
            cleanupFolder,
            Path.GetFileName(zipPath));

        destinationPath = GetAvailableFilePath(destinationPath);

        File.Move(zipPath, destinationPath);

        return destinationPath;
    }

    private static string GetAvailableFilePath(string destinationPath)
    {
        if (!File.Exists(destinationPath))
        {
            return destinationPath;
        }

        string folder = Path.GetDirectoryName(destinationPath) ?? string.Empty;
        string fileName = Path.GetFileNameWithoutExtension(destinationPath);
        string extension = Path.GetExtension(destinationPath);

        for (int index = 1; index < int.MaxValue; index++)
        {
            string candidate = Path.Combine(
                folder,
                $"{fileName}_{index:000}{extension}");

            if (!File.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new IOException($"Could not find an available file name for: {destinationPath}");
    }

    private static string GetProcessedFolder(string watchFolder)
    {
        return Path.Combine(watchFolder, "Processed");
    }

    private static string GetFailedFolder(string watchFolder)
    {
        return Path.Combine(watchFolder, "Failed");
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
    DateTime FailedAt);
