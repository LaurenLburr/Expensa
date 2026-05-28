namespace Codex.CommandEngine.Data;

public sealed class WorkflowStepExecutionStart
{
    public string WorkflowStepExecutionId { get; init; } = Guid.NewGuid().ToString("N");

    public required string WorkflowExecutionId { get; init; }

    public required string StepName { get; init; }

    public int StepOrder { get; init; }

    public required string CommandName { get; init; }

    public string StartedUtc { get; init; } = DateTimeOffset.UtcNow.ToString("O");
}
