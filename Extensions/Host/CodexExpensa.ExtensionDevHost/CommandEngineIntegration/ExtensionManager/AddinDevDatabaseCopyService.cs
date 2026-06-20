namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class AddinDevDatabaseCopyService
{
    private readonly AddinRuntimeDatabasePathService _pathService;

    public AddinDevDatabaseCopyService()
        : this(new AddinRuntimeDatabasePathService())
    {
    }

    public AddinDevDatabaseCopyService(AddinRuntimeDatabasePathService pathService)
    {
        ArgumentNullException.ThrowIfNull(pathService);
        _pathService = pathService;
    }

    public AddinDevDatabaseCopyResult CopyRuntimeDatabaseToDev(string addinId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(addinId);

        string runtimePath =
            _pathService.GetRuntimeDatabaseLocation(addinId).DatabasePath;

        if (!File.Exists(runtimePath))
        {
            throw new FileNotFoundException(
                $"The {addinId} add-in runtime database does not exist.{Environment.NewLine}{Environment.NewLine}" +
                $"Expected file:{Environment.NewLine}{runtimePath}{Environment.NewLine}{Environment.NewLine}" +
                "Update the add-in database from Prod first, then create the Dev database.",
                runtimePath);
        }

        string devPath = _pathService.GetDevCurrentDatabasePath(addinId);
        string devFolder = Path.GetDirectoryName(devPath)!;
        Directory.CreateDirectory(devFolder);

        string incomingPath = Path.Combine(
            devFolder,
            $"current.incoming.{DateTime.Now:yyyyMMdd_HHmmss_ffff}.db");

        File.Copy(runtimePath, incomingPath, overwrite: false);

        string? archivePath = null;

        try
        {
            if (File.Exists(devPath))
            {
                archivePath = Path.Combine(
                    devFolder,
                    $"current.before-runtime-copy.{DateTime.Now:yyyyMMdd_HHmmss}.db");

                File.Move(devPath, archivePath);
            }

            File.Move(incomingPath, devPath);
        }
        catch
        {
            if (File.Exists(incomingPath))
            {
                File.Delete(incomingPath);
            }

            throw;
        }

        return new AddinDevDatabaseCopyResult
        {
            AddinId = addinId,
            RuntimeDatabasePath = runtimePath,
            DevDatabasePath = devPath,
            ReplacedDevDatabaseArchivePath = archivePath
        };
    }
}
