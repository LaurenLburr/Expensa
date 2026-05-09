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

        foreach (string zipPath in Directory.GetFiles(watchFolder, "*.zip", SearchOption.TopDirectoryOnly))
        {
            QueueImport(zipPath, extractFolder);
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
        if (string.IsNullOrWhiteSpace(zipPath) || !File.Exists(zipPath))
        {
            return;
        }

        string? parentFolder = Path.GetDirectoryName(zipPath);
        if (string.IsNullOrWhiteSpace(parentFolder))
        {
            return;
        }

        if (IsInsideCleanupFolder(zipPath, parentFolder))
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
                ImportZip(zipPath, extractFolder);

                string processedPath = MoveZipToCleanupFolder(
                    zipPath,
                    GetProcessedFolder(parentFolder));

                ZipImported?.Invoke(
                    this,
                    new ZipImportedEventArgs(processedPath, extractFolder, DateTime.Now));
            }
            catch (Exception ex)
            {
                string failedPath = zipPath;

                try
                {
                    failedPath = MoveZipToCleanupFolder(
                        zipPath,
                        GetFailedFolder(parentFolder));
                }
                catch
                {
                    // Do not hide the original import failure with a cleanup failure.
                }

                ZipImportFailed?.Invoke(
                    this,
                    new ZipImportFailedEventArgs(failedPath, ex.Message, DateTime.Now));
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

    private static void ImportZip(string zipPath, string extractFolder)
    {
        Directory.CreateDirectory(extractFolder);

        string extractRoot = Path.GetFullPath(extractFolder);
        string? rootFolderToStrip = GetSingleTopLevelFolderToStrip(zipPath, extractRoot);

        using ZipArchive archive = ZipFile.OpenRead(zipPath);

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            string relativePath = NormalizeEntryPath(entry.FullName);

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(rootFolderToStrip))
            {
                relativePath = StripTopLevelFolder(relativePath, rootFolderToStrip);

                if (string.IsNullOrWhiteSpace(relativePath))
                {
                    continue;
                }
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

    private static string? GetSingleTopLevelFolderToStrip(string zipPath, string extractRoot)
    {
        using ZipArchive archive = ZipFile.OpenRead(zipPath);

        HashSet<string> topLevelFolders = new(StringComparer.OrdinalIgnoreCase);

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            string normalized = NormalizeEntryPath(entry.FullName);

            if (string.IsNullOrWhiteSpace(normalized))
            {
                continue;
            }

            string[] parts = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                topLevelFolders.Add(parts[0]);
            }
        }

        if (topLevelFolders.Count != 1)
        {
            return null;
        }

        string singleTopLevel = topLevelFolders.Single();
        string extractFolderName = Path.GetFileName(
            extractRoot.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar));

        return string.Equals(singleTopLevel, extractFolderName, StringComparison.OrdinalIgnoreCase)
            ? singleTopLevel
            : null;
    }

    private static string StripTopLevelFolder(string relativePath, string rootFolderToStrip)
    {
        string normalized = NormalizeEntryPath(relativePath);

        if (string.Equals(normalized, rootFolderToStrip, StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        string prefix = rootFolderToStrip.TrimEnd('/') + "/";

        return normalized.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? normalized[prefix.Length..]
            : normalized;
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
