namespace Codex.CommandEngine.Core;

public interface IWorkflowHeartbeatEvaluator
{
    WorkflowHeartbeatEvaluation Evaluate(
        WorkflowRuntimeSummary summary,
        DateTimeOffset nowUtc,
        TimeSpan staleAfter);
}
