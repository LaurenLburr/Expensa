namespace Codex.CommandEngine.Core;

public sealed class WorkflowResumePlan
{
    public required string WorkflowExecutionId { get; init; }

    public required string WorkflowName { get; init; }

    public required string CorrelationId { get; init; }

    public int LastCompletedStepOrder { get; init; }

    public WorkflowStepExecutionRequest? NextStep { get; init; }

    public bool CanResume => NextStep is not null;

    public string Message { get; init; } = string.Empty;
}
