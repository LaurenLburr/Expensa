namespace Codex.CommandEngine.Data;

public sealed class WorkflowExecutionCompletion
{
    public required string WorkflowExecutionId { get; init; }

    public required string Status { get; init; }

    public string CompletedUtc { get; init; } = DateTimeOffset.UtcNow.ToString("O");

    public string Message { get; init; } = string.Empty;
}
