namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class CommandExecutionPersistentRecordQuerySql
{
    public required string SqlText { get; init; }

    public IReadOnlyDictionary<string, object> Parameters { get; init; } =
        new Dictionary<string, object>();
}
