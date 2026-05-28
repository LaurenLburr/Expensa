namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class CommandExecutionPersistentRecord
{
    public required string ExecutionId { get; init; }
    public string SourceKind { get; init; } = string.Empty;
    public string CommandName { get; init; } = string.Empty;
    public string ParameterJson { get; init; } = "{}";
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string CorrelationId { get; init; } = string.Empty;
    public string OutputJson { get; init; } = string.Empty;
    public DateTimeOffset CreatedUtc { get; init; }
    public DateTimeOffset? StartedUtc { get; init; }
    public DateTimeOffset? CompletedUtc { get; init; }
    public int IsHistory { get; init; }
    public int IsQueueItem { get; init; }
}
