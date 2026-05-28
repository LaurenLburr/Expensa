namespace Codex.CommandEngine.Data;

public sealed class ExecutionHistoryCompletion
{
    public required string ExecutionId { get; init; }

    public required string Status { get; init; }

    public string CompletedUtc { get; init; } = DateTimeOffset.UtcNow.ToString("O");

    public long? DurationMilliseconds { get; init; }

    public string OutputJson { get; init; } = "{}";

    public string Message { get; init; } = string.Empty;

    public string? ExceptionText { get; init; }
}
