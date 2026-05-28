using System.Globalization;

namespace Codex.CommandEngine.Core;

public sealed class WorkflowHeartbeatEvaluator : IWorkflowHeartbeatEvaluator
{
    public WorkflowHeartbeatEvaluation Evaluate(
        WorkflowRuntimeSummary summary,
        DateTimeOffset nowUtc,
        TimeSpan staleAfter)
    {
        ArgumentNullException.ThrowIfNull(summary);

        if (staleAfter <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(staleAfter), staleAfter, "Stale threshold must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(summary.LastHeartbeatUtc))
        {
            return new WorkflowHeartbeatEvaluation
            {
                WorkflowExecutionId = summary.WorkflowExecutionId,
                Status = WorkflowHeartbeatStatus.Missing,
                LastHeartbeatUtc = null,
                Age = null,
                Message = "Workflow has no heartbeat."
            };
        }

        if (!DateTimeOffset.TryParse(
                summary.LastHeartbeatUtc,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out DateTimeOffset lastHeartbeatUtc))
        {
            return new WorkflowHeartbeatEvaluation
            {
                WorkflowExecutionId = summary.WorkflowExecutionId,
                Status = WorkflowHeartbeatStatus.Unknown,
                LastHeartbeatUtc = null,
                Age = null,
                Message = "Workflow heartbeat could not be parsed."
            };
        }

        TimeSpan age = nowUtc - lastHeartbeatUtc;

        if (age < TimeSpan.Zero)
        {
            age = TimeSpan.Zero;
        }

        if (age > staleAfter)
        {
            return new WorkflowHeartbeatEvaluation
            {
                WorkflowExecutionId = summary.WorkflowExecutionId,
                Status = WorkflowHeartbeatStatus.Stale,
                LastHeartbeatUtc = lastHeartbeatUtc,
                Age = age,
                Message = "Workflow heartbeat is stale."
            };
        }

        return new WorkflowHeartbeatEvaluation
        {
            WorkflowExecutionId = summary.WorkflowExecutionId,
            Status = WorkflowHeartbeatStatus.Healthy,
            LastHeartbeatUtc = lastHeartbeatUtc,
            Age = age,
            Message = "Workflow heartbeat is healthy."
        };
    }
}
