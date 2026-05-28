namespace Codex.CommandEngine.Core;

public sealed class WorkflowResumeServiceRequest
{
    public required string WorkflowExecutionId { get; init; }

    public IReadOnlyList<WorkflowStepExecutionRequest> WorkflowDefinitionSteps { get; init; } =
        [];
}
