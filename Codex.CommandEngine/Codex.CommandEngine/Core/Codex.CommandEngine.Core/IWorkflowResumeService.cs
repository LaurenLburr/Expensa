namespace Codex.CommandEngine.Core;

public interface IWorkflowResumeService
{
    Task<WorkflowExecutionResult> ResumeAsync(
        WorkflowResumeServiceRequest request,
        CancellationToken cancellationToken = default);
}
