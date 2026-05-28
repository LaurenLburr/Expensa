namespace Codex.CommandEngine.Core;

public sealed class WorkflowExecutionResult
{
    public required string WorkflowName { get; init; }

    public required string CorrelationId { get; init; }

    public WorkflowExecutionStatus Status { get; init; }

    public string Message { get; init; } = string.Empty;

    public IReadOnlyList<WorkflowStepExecutionResult> StepResults { get; init; } =
        [];
}
