namespace Codex.CommandEngine.Core;

public interface IWorkflowRuntimeActionViewModelFactory
{
    WorkflowRuntimeActionViewModel Create(
        string actionName,
        WorkflowRuntimeActionResult result,
        DateTimeOffset completedUtc);
}
