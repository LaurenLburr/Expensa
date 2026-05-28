namespace Codex.CommandEngine.Core;

public interface IWorkflowResumeByDefinitionService
{
    Task<WorkflowExecutionResult> ResumeAsync(
        WorkflowResumeByDefinitionRequest request,
        CancellationToken cancellationToken = default);
}
