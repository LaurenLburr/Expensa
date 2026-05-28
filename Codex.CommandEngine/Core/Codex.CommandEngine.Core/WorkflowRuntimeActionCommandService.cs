namespace Codex.CommandEngine.Core;

public sealed class WorkflowRuntimeActionCommandService
{
    private readonly IWorkflowRuntimeActionService _actionService;
    private readonly IWorkflowRuntimeActionPresenter _presenter;

    public WorkflowRuntimeActionCommandService(
        IWorkflowRuntimeActionService actionService,
        IWorkflowRuntimeActionPresenter presenter)
    {
        ArgumentNullException.ThrowIfNull(actionService);
        ArgumentNullException.ThrowIfNull(presenter);

        _actionService = actionService;
        _presenter = presenter;
    }

    public string AbandonAndPresent(
        string workflowExecutionId,
        string message,
        DateTimeOffset completedUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        WorkflowRuntimeActionResult result =
            _actionService.Abandon(workflowExecutionId, message);

        return _presenter.Present(
            "Abandon Workflow",
            result,
            completedUtc);
    }

    public string HeartbeatAndPresent(
        string workflowExecutionId,
        DateTimeOffset heartbeatUtc,
        DateTimeOffset completedUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        WorkflowRuntimeActionResult result =
            _actionService.Heartbeat(workflowExecutionId, heartbeatUtc);

        return _presenter.Present(
            "Update Workflow Heartbeat",
            result,
            completedUtc);
    }
}
