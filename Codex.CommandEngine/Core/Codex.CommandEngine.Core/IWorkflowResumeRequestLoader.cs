namespace Codex.CommandEngine.Core;

public interface IWorkflowResumeRequestLoader
{
    WorkflowResumeExecutionRequest Load(
        string workflowExecutionId,
        IReadOnlyList<WorkflowStepExecutionRequest> workflowDefinitionSteps);
}
