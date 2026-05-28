namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class CommandExecutionQueueItem
{
    public Guid QueueItemId { get; init; } = Guid.NewGuid();

    public DateTimeOffset CreatedUtc { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? StartedUtc { get; set; }

    public DateTimeOffset? CompletedUtc { get; set; }

    public required string CommandName { get; init; }

    public string ParameterJson { get; init; } = "{}";

    public CommandExecutionQueueStatus Status { get; set; } = CommandExecutionQueueStatus.Pending;

    public string Message { get; set; } = string.Empty;

    public string CorrelationId { get; set; } = string.Empty;

    public string OutputJson { get; set; } = string.Empty;

    public TimeSpan? Duration =>
        StartedUtc is null || CompletedUtc is null
            ? null
            : CompletedUtc.Value - StartedUtc.Value;
}
