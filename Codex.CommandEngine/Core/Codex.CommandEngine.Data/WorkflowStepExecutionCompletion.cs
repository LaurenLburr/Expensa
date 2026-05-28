namespace Codex.CommandEngine.Data;

public sealed class WorkflowStepExecutionCompletion
{
    public required string WorkflowStepExecutionId { get; init; }

    public required string Status { get; init; }

    public string CompletedUtc { get; init; } = DateTimeOffset.UtcNow.ToString("O");

    public string Message { get; init; } = string.Empty;

    public string? CommandExecutionId { get; init; }
}
