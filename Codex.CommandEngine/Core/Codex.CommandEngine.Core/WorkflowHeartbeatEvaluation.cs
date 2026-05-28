namespace Codex.CommandEngine.Core;

public sealed class WorkflowHeartbeatEvaluation
{
    public required string WorkflowExecutionId { get; init; }

    public WorkflowHeartbeatStatus Status { get; init; }

    public DateTimeOffset? LastHeartbeatUtc { get; init; }

    public TimeSpan? Age { get; init; }

    public string Message { get; init; } = string.Empty;
}
