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

        if (string.IsNullOrWhiteSpace(runtimeDatabasePath))
        {
            return _pathService.GetRuntimeDatabaseLocation();
        }

        return new HostWebsiteDatabaseLocation
        {
            DatabaseName = Path.GetFileName(runtimeDatabasePath),
            DatabasePath = runtimeDatabasePath
        };
    }

    public void SetActiveRuntimeDatabase(
        string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        string settingsPath =
            GetSettingsPath();

        Directory.CreateDirectory(
            Path.GetDirectoryName(settingsPath)!);

        File.WriteAllText(
            settingsPath,
            databasePath);
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
