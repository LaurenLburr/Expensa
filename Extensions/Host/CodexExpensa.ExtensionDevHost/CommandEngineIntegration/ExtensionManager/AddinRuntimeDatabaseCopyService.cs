namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class AddinRuntimeDatabaseCopyService
{
    private readonly AddinRuntimeDatabasePathService _pathService;

    public AddinRuntimeDatabaseCopyService()
        : this(new AddinRuntimeDatabasePathService())
    {
    }

    public AddinRuntimeDatabaseCopyService(
        AddinRuntimeDatabasePathService pathService)
    {
        ArgumentNullException.ThrowIfNull(pathService);

        _pathService = pathService;
    }

    public AddinRuntimeDatabaseCopyResult ReplaceRuntimeDatabaseFromProd(
        string addinId)
    {
        return ReplaceRuntimeDatabase(
            addinId,
            "Prod",
            _pathService.GetProdDatabasePath());
    }

    public AddinRuntimeDatabaseCopyResult ReplaceRuntimeDatabaseFromDev(
        string addinId)
    {
        return ReplaceRuntimeDatabase(
            addinId,
            "Dev",
            _pathService.GetDevCurrentDatabasePath(addinId));
    }

    public AddinRuntimeDatabaseCopyResult ReplaceRuntimeDatabase(
        string addinId,
        string sourceLabel,
        string sourceDatabasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(addinId);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceLabel);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDatabasePath);

        if (!File.Exists(sourceDatabasePath))
        {
            throw new FileNotFoundException(
                $"{sourceLabel} database was not found: {sourceDatabasePath}",
                sourceDatabasePath);
        }

        AddinRuntimeDatabaseLocation runtimeLocation =
            _pathService.GetRuntimeDatabaseLocation(addinId);

        string fullSourcePath =
            Path.GetFullPath(sourceDatabasePath);

        string fullRuntimePath =
            Path.GetFullPath(runtimeLocation.DatabasePath);

        if (string.Equals(fullSourcePath, fullRuntimePath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The source database and add-in runtime database are the same file. Refusing to replace a database with itself.");
        }

        Directory.CreateDirectory(runtimeLocation.DatabaseFolder);

        string tempCopyPath =
            Path.Combine(
                runtimeLocation.DatabaseFolder,
                $"{Path.GetFileNameWithoutExtension(runtimeLocation.DatabasePath)}.incoming.{DateTime.Now:yyyyMMdd_HHmmss_ffff}.db");

        File.Copy(
            sourceDatabasePath,
            tempCopyPath,
            overwrite: false);

        string? archivePath = null;

        try
        {
            if (File.Exists(runtimeLocation.DatabasePath))
            {
                archivePath =
                    CreateArchivePath(runtimeLocation.DatabasePath, sourceLabel);

                File.Move(
                    runtimeLocation.DatabasePath,
                    archivePath);
            }

            File.Move(
                tempCopyPath,
                runtimeLocation.DatabasePath);
        }
        catch
        {
            if (File.Exists(tempCopyPath))
            {
                File.Delete(tempCopyPath);
            }

            throw;
        }

        string activeRuntimePathFile =
            _pathService.GetActiveRuntimePathFile(addinId);

        Directory.CreateDirectory(
            Path.GetDirectoryName(activeRuntimePathFile)!);

        File.WriteAllText(
            activeRuntimePathFile,
            runtimeLocation.DatabasePath);

        return new AddinRuntimeDatabaseCopyResult
        {
            AddinId = addinId,
            SourceLabel = sourceLabel,
            SourceDatabasePath = sourceDatabasePath,
            RuntimeDatabasePath = runtimeLocation.DatabasePath,
            ActiveRuntimePathFile = activeRuntimePathFile,
            ReplacedDatabaseArchivePath = archivePath
        };
    }

    private static string CreateArchivePath(
        string databasePath,
        string sourceLabel)
    {
        string folder =
            Path.GetDirectoryName(databasePath)!;

        string fileNameWithoutExtension =
            Path.GetFileNameWithoutExtension(databasePath);

        string extension =
            Path.GetExtension(databasePath);

        string safeSourceLabel =
            new string(
                sourceLabel
                    .Where(static character => char.IsLetterOrDigit(character))
                    .Select(static character => char.ToLowerInvariant(character))
                    .ToArray());

        if (string.IsNullOrWhiteSpace(safeSourceLabel))
        {
            safeSourceLabel = "copy";
        }

        return Path.Combine(
            folder,
            $"{fileNameWithoutExtension}.before-{safeSourceLabel}-copy.{DateTime.Now:yyyyMMdd_HHmmss}{extension}");
    }
}
