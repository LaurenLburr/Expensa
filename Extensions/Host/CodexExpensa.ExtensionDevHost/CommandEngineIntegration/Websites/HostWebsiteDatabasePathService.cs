namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteDatabasePathService
{
    private const string AddinId = "WebsitesAddin";
    private const string DefaultDatabaseFileName = "websites.current.db";
    private const string SandboxDatabaseFileName = "websitesaddin.sandbox.db";
    private const string DevDatabaseFileName = "websites.dev.db";

    public HostWebsiteDatabaseLocation GetRuntimeDatabaseLocation()
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "Runtime",
            AddinId);

        return new HostWebsiteDatabaseLocation
        {
            DatabaseName = DefaultDatabaseFileName,
            DatabasePath = Path.Combine(folder, DefaultDatabaseFileName)
        };
    }

    public string GetDevTemplateDatabasePath()
    {
        return Path.Combine(
            FindSolutionRoot(),
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

    public string GenerateTimestampedDatabaseFileName(
        string sourceLabel)
    {
        string timestamp =
            DateTime.Now.ToString("yyyyMMdd_HHmmss");

        string safeSourceLabel =
            string.IsNullOrWhiteSpace(sourceLabel)
                ? "copy"
                : new string(
                    sourceLabel
                        .Trim()
                        .Select(static character =>
                            char.IsLetterOrDigit(character)
                                ? character
                                : '_')
                        .ToArray());

        return $"websitesaddin.{safeSourceLabel}.{timestamp}.db";
    }

    private static string FindSolutionRoot()
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
