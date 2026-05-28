namespace Codex.CommandEngine.Core;

public sealed class WorkflowRuntimeActionViewModelFactory : IWorkflowRuntimeActionViewModelFactory
{
    public WorkflowRuntimeActionViewModel Create(
        string actionName,
        WorkflowRuntimeActionResult result,
        DateTimeOffset completedUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(actionName);
        ArgumentNullException.ThrowIfNull(result);
        ArgumentException.ThrowIfNullOrWhiteSpace(result.WorkflowExecutionId);

        return new WorkflowRuntimeActionViewModel
        {
            WorkflowExecutionId = result.WorkflowExecutionId,
            ActionName = actionName,
            Succeeded = result.Succeeded,
            Message = result.Message,
            CompletedUtc = completedUtc
        };
    }
}
