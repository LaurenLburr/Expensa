using System.Text;

namespace Codex.CommandEngine.Core;

public static class WorkflowRuntimeDetailTextFormatter
{
    public static string Format(
        WorkflowRuntimeDetail detail,
        IWorkflowHeartbeatEvaluator heartbeatEvaluator,
        DateTimeOffset nowUtc,
        TimeSpan staleAfter)
    {
        ArgumentNullException.ThrowIfNull(detail);
        ArgumentNullException.ThrowIfNull(detail.Summary);
        ArgumentNullException.ThrowIfNull(heartbeatEvaluator);

        WorkflowHeartbeatEvaluation heartbeat =
            heartbeatEvaluator.Evaluate(detail.Summary, nowUtc, staleAfter);

        StringBuilder builder = new();

        builder.AppendLine("Workflow Runtime Detail");
        builder.AppendLine("=======================");
        builder.AppendLine();
        builder.AppendLine($"Workflow: {detail.Summary.WorkflowName}");
        builder.AppendLine($"Execution Id: {detail.Summary.WorkflowExecutionId}");
        builder.AppendLine($"Correlation Id: {detail.Summary.CorrelationId}");
        builder.AppendLine($"Status: {detail.Summary.Status}");
        builder.AppendLine($"Operation Status: {detail.Summary.OperationStatus}");
        builder.AppendLine($"Heartbeat Status: {heartbeat.Status}");
        builder.AppendLine($"Last Completed Step Order: {detail.Summary.LastCompletedStepOrder}");
        builder.AppendLine();

        builder.AppendLine("Steps");
        builder.AppendLine("-----");

        if (detail.Steps.Count == 0)
        {
            builder.AppendLine("No workflow steps have been recorded.");
            return builder.ToString();
        }

        foreach (WorkflowRuntimeStepSummary step in detail.Steps.OrderBy(static item => item.StepOrder))
        {
            builder.AppendLine();
            builder.AppendLine($"[{step.StepOrder}] {step.StepName}");
            builder.AppendLine($"Command: {step.CommandName}");
            builder.AppendLine($"Status: {step.Status}");
            builder.AppendLine($"Message: {step.Message}");
        }

        return builder.ToString();
    }
}
