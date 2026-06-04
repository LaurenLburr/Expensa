namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;

public sealed class ExpensaAddinLoaderResult
{
    public required string AddinName { get; init; }

    public required string AssemblyPath { get; init; }

    public required string DatabasePath { get; init; }

    public required string CommandName { get; init; }

    public required string CorrelationId { get; init; }

    public required string Status { get; init; }

    public required string Message { get; init; }

    public required string OutputJson { get; init; }
}
