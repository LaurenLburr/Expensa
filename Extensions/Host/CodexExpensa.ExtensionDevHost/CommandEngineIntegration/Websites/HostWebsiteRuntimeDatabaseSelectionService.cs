namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteRuntimeDatabaseSelectionService
{
    private readonly HostWebsiteDatabasePathService _pathService;

    public HostWebsiteRuntimeDatabaseSelectionService()
        : this(new HostWebsiteDatabasePathService())
    {
    }

    public HostWebsiteRuntimeDatabaseSelectionService(
        HostWebsiteDatabasePathService pathService)
    {
        ArgumentNullException.ThrowIfNull(pathService);

        _pathService = pathService;
    }

    public HostWebsiteDatabaseLocation GetActiveRuntimeDatabaseLocation()
    {
        string settingsPath =
            GetSettingsPath();

        if (!File.Exists(settingsPath))
        {
            return _pathService.GetRuntimeDatabaseLocation();
        }

        string runtimeDatabasePath =
            File.ReadAllText(settingsPath).Trim();

        if (string.IsNullOrWhiteSpace(runtimeDatabasePath) ||
            Directory.Exists(runtimeDatabasePath) ||
            !File.Exists(runtimeDatabasePath))
        {
            return _pathService.GetRuntimeDatabaseLocation();
        }

        return new HostWebsiteDatabaseLocation
        {
            DatabaseName = Path.GetFileName(runtimeDatabasePath),
            DatabasePath = runtimeDatabasePath
        };
    }

    public HostWebsiteDatabaseLocation SetActiveRuntimeDatabase(
        string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        if (!File.Exists(databasePath))
        {
            throw new FileNotFoundException(
                $"Selected database was not found: {databasePath}",
                databasePath);
        }

        HostWebsiteDatabaseLocation runtimeLocation =
            _pathService.GetRuntimeDatabaseLocation();

        Directory.CreateDirectory(runtimeLocation.DatabaseFolder);

        File.Copy(
            databasePath,
            runtimeLocation.DatabasePath,
            overwrite: true);

        string settingsPath =
            GetSettingsPath();

        Directory.CreateDirectory(
            Path.GetDirectoryName(settingsPath)!);

        File.WriteAllText(
            settingsPath,
            runtimeLocation.DatabasePath);

        return runtimeLocation;
    }

    private static string GetSettingsPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "Runtime",
            "WebsitesAddin",
            "active-runtime-db.txt");
    }
}
