namespace Codex.CommandEngine.Data;

public sealed class ExecutionHistoryStart
{
    public string ExecutionId { get; init; } = Guid.NewGuid().ToString("N");

    public required string CorrelationId { get; init; }

    public required string CommandName { get; init; }

    public string RequestJson { get; init; } = "{}";

    public string StartedUtc { get; init; } = DateTimeOffset.UtcNow.ToString("O");
}
