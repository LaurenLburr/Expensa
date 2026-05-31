namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteDatabaseLocation
{
    public required string DatabaseName { get; init; }

    public required string DatabasePath { get; init; }

    public string DatabaseFolder =>
        Path.GetDirectoryName(DatabasePath) ?? string.Empty;
}
