namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteExpensaProdDatabaseCopyResult
{
    public required string ProdDatabasePath { get; init; }

    public required string StagingDatabasePath { get; init; }

    public required string DevDatabasePath { get; init; }

    public required string RuntimeDatabasePath { get; init; }

    public required string ActiveRuntimePathFile { get; init; }

    public List<string> Warnings { get; init; } = [];
}
