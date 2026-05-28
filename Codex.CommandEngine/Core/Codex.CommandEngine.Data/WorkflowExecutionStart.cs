namespace Codex.CommandEngine.Data;

public sealed class WorkflowExecutionStart
{
    public string WorkflowExecutionId { get; init; } = Guid.NewGuid().ToString("N");

    public required string WorkflowName { get; init; }

    public required string CorrelationId { get; init; }

    public string StartedUtc { get; init; } = DateTimeOffset.UtcNow.ToString("O");

    public string ContextJson { get; init; } = "{}";
}
