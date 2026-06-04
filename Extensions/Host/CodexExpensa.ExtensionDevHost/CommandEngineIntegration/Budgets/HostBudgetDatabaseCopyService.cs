namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetDatabaseCopyService
{
    private readonly HostBudgetDatabasePathService pathService;

    public HostBudgetDatabaseCopyService()
        : this(new HostBudgetDatabasePathService())
    {
    }

    public HostBudgetDatabaseCopyService(HostBudgetDatabasePathService pathService)
    {
        ArgumentNullException.ThrowIfNull(pathService);

        this.pathService = pathService;
    }

    public string CopyNewFromDevTemplate()
    {
        return CopyFromSource(
            pathService.GetDevTemplateDatabasePath(),
            "dev");
    }

    public string CopyNewFromSandbox()
    {
        return CopyFromSource(
            pathService.GetSandboxDatabasePath(),
            "sandbox");
    }

    private string CopyFromSource(string sourcePath, string sourceLabel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceLabel);

        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException(
                $"Source database was not found: {sourcePath}",
                sourcePath);
        }

        HostBudgetDatabaseLocation runtimeLocation =
            pathService.GetRuntimeDatabaseLocation();

        Directory.CreateDirectory(runtimeLocation.DatabaseFolder);

        string targetPath =
            Path.Combine(
                runtimeLocation.DatabaseFolder,
                pathService.GenerateTimestampedDatabaseFileName(sourceLabel));

        File.Copy(sourcePath, targetPath, overwrite: false);

        return targetPath;
    }
}
