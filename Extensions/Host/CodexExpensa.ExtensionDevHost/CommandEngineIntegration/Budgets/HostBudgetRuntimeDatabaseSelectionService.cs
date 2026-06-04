namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetRuntimeDatabaseSelectionService
{
    private readonly HostBudgetDatabasePathService pathService;

    public HostBudgetRuntimeDatabaseSelectionService()
        : this(new HostBudgetDatabasePathService())
    {
    }

    public HostBudgetRuntimeDatabaseSelectionService(HostBudgetDatabasePathService pathService)
    {
        ArgumentNullException.ThrowIfNull(pathService);

        this.pathService = pathService;
    }

    public HostBudgetDatabaseLocation GetActiveRuntimeDatabaseLocation()
    {
        string settingsPath = GetSettingsPath();

        if (!File.Exists(settingsPath))
        {
            return pathService.GetRuntimeDatabaseLocation();
        }

        string runtimeDatabasePath = File.ReadAllText(settingsPath).Trim();

        if (string.IsNullOrWhiteSpace(runtimeDatabasePath))
        {
            return pathService.GetRuntimeDatabaseLocation();
        }

        return new HostBudgetDatabaseLocation
        {
            DatabaseName = Path.GetFileName(runtimeDatabasePath),
            DatabasePath = runtimeDatabasePath
        };
    }

    public HostBudgetDatabaseLocation SetActiveRuntimeDatabase(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        if (!File.Exists(databasePath))
        {
            throw new FileNotFoundException(
                $"Selected database was not found: {databasePath}",
                databasePath);
        }

        HostBudgetDatabaseLocation runtimeLocation =
            pathService.GetRuntimeDatabaseLocation();

        Directory.CreateDirectory(runtimeLocation.DatabaseFolder);

        File.Copy(
            databasePath,
            runtimeLocation.DatabasePath,
            overwrite: true);

        string settingsPath = GetSettingsPath();

        Directory.CreateDirectory(Path.GetDirectoryName(settingsPath)!);

        File.WriteAllText(settingsPath, runtimeLocation.DatabasePath);

        return runtimeLocation;
    }

    private static string GetSettingsPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "Runtime",
            "BudgetsAddin",
            "active-runtime-db.txt");
    }
}
