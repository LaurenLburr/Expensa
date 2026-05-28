namespace Codex.CommandEngine.Core;

public interface IWorkflowRuntimeActionPresenter
{
    string Present(
        string actionName,
        WorkflowRuntimeActionResult result,
        DateTimeOffset completedUtc);
}
