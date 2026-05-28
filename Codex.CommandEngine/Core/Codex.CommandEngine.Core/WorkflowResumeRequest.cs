namespace Codex.CommandEngine.Core;

public sealed class WorkflowResumeRequest
{
    public required string WorkflowExecutionId { get; init; }

    public required string WorkflowName { get; init; }

    public required string CorrelationId { get; init; }

    public int LastCompletedStepOrder { get; init; }

    public IReadOnlyList<WorkflowStepExecutionRequest> Steps { get; init; } =
        [];
}
