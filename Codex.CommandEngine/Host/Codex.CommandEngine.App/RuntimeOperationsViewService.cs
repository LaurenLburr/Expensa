using Codex.CommandEngine.Core;

namespace Codex.CommandEngine.App;

public sealed class RuntimeOperationsViewService
{
    private readonly IWorkflowRuntimeOperations _operations;
    private readonly IWorkflowHeartbeatEvaluator _heartbeatEvaluator;
    private readonly TimeSpan _staleAfter;

    public RuntimeOperationsViewService(
        IWorkflowRuntimeOperations operations,
        IWorkflowHeartbeatEvaluator heartbeatEvaluator,
        TimeSpan staleAfter)
    {
        ArgumentNullException.ThrowIfNull(operations);
        ArgumentNullException.ThrowIfNull(heartbeatEvaluator);

        if (staleAfter <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(staleAfter), staleAfter, "Stale threshold must be greater than zero.");
        }

        _operations = operations;
        _heartbeatEvaluator = heartbeatEvaluator;
        _staleAfter = staleAfter;
    }

    public string BuildResumableWorkflowsText()
    {
        return RuntimeOperationsTextFormatter.FormatWorkflowSummaries(
            "Resumable Workflows",
            _operations.ListResumable());
    }

    public string BuildIncompleteWorkflowsText()
    {
        return RuntimeOperationsTextFormatter.FormatWorkflowSummaries(
            "Incomplete Workflows",
            _operations.ListIncomplete());
    }

    public string BuildStaleWorkflowsText(DateTimeOffset nowUtc)
    {
        return RuntimeOperationsTextFormatter.FormatWorkflowSummaries(
            "Stale Workflows",
            _operations.ListStale(_heartbeatEvaluator, nowUtc, _staleAfter));
    }

    public string BuildWorkflowDetailText(
        string workflowExecutionId,
        DateTimeOffset nowUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        WorkflowRuntimeDetail detail =
            _operations.GetDetail(workflowExecutionId);

        return RuntimeOperationsTextFormatter.FormatWorkflowDetail(
            detail,
            _heartbeatEvaluator,
            nowUtc,
            _staleAfter);
    }
}
