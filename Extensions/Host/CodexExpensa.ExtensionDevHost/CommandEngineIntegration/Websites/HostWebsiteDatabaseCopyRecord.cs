namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteDatabaseCopyRecord
{
    public required string SourceLabel { get; init; }

    public required string DatabasePath { get; init; }

    public string DatabaseName =>
        Path.GetFileName(DatabasePath);

    public DateTime CreatedLocal { get; init; } = DateTime.Now;
}
