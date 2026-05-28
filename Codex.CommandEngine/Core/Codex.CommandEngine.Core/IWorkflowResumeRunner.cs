namespace Codex.CommandEngine.Core;

public interface IWorkflowResumeRunner
{
    Task<WorkflowExecutionResult> ResumeAsync(
        WorkflowResumeExecutionRequest request,
        CancellationToken cancellationToken = default);
}
