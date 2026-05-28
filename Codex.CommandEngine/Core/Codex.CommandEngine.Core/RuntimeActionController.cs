namespace Codex.CommandEngine.Core;

public sealed class RuntimeActionController : IRuntimeActionController
{
    private readonly IWorkflowRuntimeActionService _actionService;
    private readonly IWorkflowRuntimeActionPresenter _presenter;
    private readonly IWorkflowRuntimeOperations _runtimeOperations;
    private readonly IWorkflowHeartbeatEvaluator _heartbeatEvaluator;
    private readonly TimeSpan _staleAfter;

    public RuntimeActionController(
        IWorkflowRuntimeActionService actionService,
        IWorkflowRuntimeActionPresenter presenter,
        IWorkflowRuntimeOperations runtimeOperations,
        IWorkflowHeartbeatEvaluator heartbeatEvaluator,
        TimeSpan staleAfter)
    {
        ArgumentNullException.ThrowIfNull(actionService);
        ArgumentNullException.ThrowIfNull(presenter);
        ArgumentNullException.ThrowIfNull(runtimeOperations);
        ArgumentNullException.ThrowIfNull(heartbeatEvaluator);

        if (staleAfter <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(staleAfter), staleAfter, "Stale threshold must be greater than zero.");
        }

        _actionService = actionService;
        _presenter = presenter;
        _runtimeOperations = runtimeOperations;
        _heartbeatEvaluator = heartbeatEvaluator;
        _staleAfter = staleAfter;
    }

    public RuntimeActionControllerResult AbandonWorkflow(
        string workflowExecutionId,
        string message,
        DateTimeOffset completedUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        WorkflowRuntimeActionResult actionResult =
            _actionService.Abandon(workflowExecutionId, message);

        string displayText =
            _presenter.Present(
                "Abandon Workflow",
                actionResult,
                completedUtc);

        string refreshText =
            BuildRefreshText(workflowExecutionId, completedUtc);

        return new RuntimeActionControllerResult
        {
            WorkflowExecutionId = workflowExecutionId,
            ActionName = "Abandon Workflow",
            Succeeded = actionResult.Succeeded,
            DisplayText = displayText,
            RefreshText = refreshText,
            Message = actionResult.Message
        };
    }

    public RuntimeActionControllerResult UpdateHeartbeat(
        string workflowExecutionId,
        DateTimeOffset heartbeatUtc,
        DateTimeOffset completedUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        WorkflowRuntimeActionResult actionResult =
            _actionService.Heartbeat(workflowExecutionId, heartbeatUtc);

        string displayText =
            _presenter.Present(
                "Update Workflow Heartbeat",
                actionResult,
                completedUtc);

        string refreshText =
            BuildRefreshText(workflowExecutionId, completedUtc);

        return new RuntimeActionControllerResult
        {
            WorkflowExecutionId = workflowExecutionId,
            ActionName = "Update Workflow Heartbeat",
            Succeeded = actionResult.Succeeded,
            DisplayText = displayText,
            RefreshText = refreshText,
            Message = actionResult.Message
        };
    }

    private string BuildRefreshText(
        string workflowExecutionId,
        DateTimeOffset nowUtc)
    {
        try
        {
            WorkflowRuntimeDetail detail =
                _runtimeOperations.GetDetail(workflowExecutionId);

            return WorkflowRuntimeDetailTextFormatter.Format(
                detail,
                _heartbeatEvaluator,
                nowUtc,
                _staleAfter);
        }
        catch (Exception exception)
        {
            return
                $"""
                Runtime Refresh Failed
                ======================

                Workflow Execution Id:
                {workflowExecutionId}

                Error:
                {exception.Message}

                Details:
                {exception}
                """;
        }
    }
}
