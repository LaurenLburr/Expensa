namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class CommandExecutionRetentionOptions
{
    public DateTimeOffset? CreatedBeforeUtc { get; init; }

    public string Status { get; init; } = string.Empty;

    public CommandExecutionPersistentRecordSourceFilter SourceFilter { get; init; } =
        CommandExecutionPersistentRecordSourceFilter.All;
}
