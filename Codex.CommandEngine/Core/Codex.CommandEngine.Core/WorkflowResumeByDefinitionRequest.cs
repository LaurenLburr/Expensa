namespace Codex.CommandEngine.Core;

public sealed class WorkflowResumeByDefinitionRequest
{
    public required string WorkflowExecutionId { get; init; }
}
