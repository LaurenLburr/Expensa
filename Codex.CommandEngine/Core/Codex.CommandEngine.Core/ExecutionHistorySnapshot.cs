namespace Codex.CommandEngine.Core;

public sealed class ExecutionHistorySnapshot
{
    public required string ExecutionId { get; init; }

    public required string CorrelationId { get; init; }

    public required string CommandName { get; init; }

    public ExecutionHistoryStatus Status { get; init; }

    public DateTimeOffset StartedUtc { get; init; }

    public DateTimeOffset? CompletedUtc { get; init; }

    public long? DurationMilliseconds { get; init; }

    public string RequestJson { get; init; } = "{}";

    public string OutputJson { get; init; } = "{}";

    public string Message { get; init; } = string.Empty;

    public string? ExceptionText { get; init; }
}
