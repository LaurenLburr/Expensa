namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteDatabaseCopyService
{
    private readonly HostWebsiteDatabasePathService _pathService;

    public HostWebsiteDatabaseCopyService()
        : this(new HostWebsiteDatabasePathService())
    {
    }

    public HostWebsiteDatabaseCopyService(
        HostWebsiteDatabasePathService pathService)
    {
        ArgumentNullException.ThrowIfNull(pathService);

        _pathService = pathService;
    }

    public string CopyNewFromDevTemplate()
    {
        string sourcePath =
            _pathService.GetDevTemplateDatabasePath();

        return CopyFromSource(
            sourcePath,
            "dev");
    }

    public string CopyNewFromSandbox()
    {
        string sourcePath =
            _pathService.GetSandboxDatabasePath();

        return CopyFromSource(
            sourcePath,
            "sandbox");
    }

    private string CopyFromSource(
        string sourcePath,
        string sourceLabel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceLabel);

        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException(
                $"Source database was not found: {sourcePath}",
                sourcePath);
        }

        HostWebsiteDatabaseLocation runtimeLocation =
            _pathService.GetRuntimeDatabaseLocation();

        Directory.CreateDirectory(runtimeLocation.DatabaseFolder);

        string targetFileName =
            _pathService.GenerateTimestampedDatabaseFileName(sourceLabel);

        string targetPath =
            Path.Combine(
                runtimeLocation.DatabaseFolder,
                targetFileName);

        File.Copy(sourcePath, targetPath, overwrite: false);

        return targetPath;
    }
}
