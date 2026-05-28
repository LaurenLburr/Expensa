namespace Codex.CommandEngine.Core;

public static class WorkflowRuntimeOperationsExtensions
{
    public static IReadOnlyList<WorkflowRuntimeSummary> ListStale(
        this IWorkflowRuntimeOperations operations,
        IWorkflowHeartbeatEvaluator heartbeatEvaluator,
        DateTimeOffset nowUtc,
        TimeSpan staleAfter)
    {
        ArgumentNullException.ThrowIfNull(operations);
        ArgumentNullException.ThrowIfNull(heartbeatEvaluator);

        return operations
            .ListResumable()
            .Where(summary =>
                heartbeatEvaluator.Evaluate(summary, nowUtc, staleAfter).Status
                is WorkflowHeartbeatStatus.Stale or WorkflowHeartbeatStatus.Missing or WorkflowHeartbeatStatus.Unknown)
            .ToList();
    }
}
