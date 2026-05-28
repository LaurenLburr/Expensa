namespace Codex.CommandEngine.Core;

public interface IWorkflowRunner
{
    Task<WorkflowExecutionResult> ExecuteAsync(
        WorkflowExecutionRequest request,
        CancellationToken cancellationToken = default);
}
