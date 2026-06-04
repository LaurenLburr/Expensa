namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetDatabasePathService
{
    private const string AddinId = "BudgetsAddin";
    private const string RuntimeDatabaseFileName = "budgets.current.db";
    private const string SandboxDatabaseFileName = "budgets.sandbox.db";
    private const string DevDatabaseFileName = "budgets.dev.db";

    public HostBudgetDatabaseLocation GetRuntimeDatabaseLocation()
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "Runtime",
            AddinId);

        return new HostBudgetDatabaseLocation
        {
            DatabaseName = RuntimeDatabaseFileName,
            DatabasePath = Path.Combine(folder, RuntimeDatabaseFileName)
        };
    }

    public string GetDevTemplateDatabasePath()
    {
        return Path.Combine(
            FindExtensionsRoot(),
            "Modules",
            AddinId,
            "DevDatabase",
            DevDatabaseFileName);
    }

    public string GetSandboxDatabasePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "Sandbox",
            AddinId,
            SandboxDatabaseFileName);
    }

    public string GenerateTimestampedDatabaseFileName(string sourceLabel)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        string safeSourceLabel =
            string.IsNullOrWhiteSpace(sourceLabel)
                ? "copy"
                : new string(
                    sourceLabel
                        .Trim()
                        .Select(static character => char.IsLetterOrDigit(character) ? character : '_')
                        .ToArray());

        return $"budgets.{safeSourceLabel}.{timestamp}.db";
    }

    private static string FindExtensionsRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (string.Equals(directory.Name, "Extensions", StringComparison.OrdinalIgnoreCase))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Extensions");
    }
}
