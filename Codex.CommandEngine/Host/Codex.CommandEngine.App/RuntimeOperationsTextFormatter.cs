using System.Text;
using Codex.CommandEngine.Core;

namespace Codex.CommandEngine.App;

public static class RuntimeOperationsTextFormatter
{
    public static string FormatWorkflowSummaries(
        string title,
        IReadOnlyList<WorkflowRuntimeSummary> summaries)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(summaries);

        StringBuilder builder = new();

        builder.AppendLine(title);
        builder.AppendLine(new string('=', title.Length));
        builder.AppendLine();
        builder.AppendLine($"Count: {summaries.Count}");
        builder.AppendLine();

        if (summaries.Count == 0)
        {
            builder.AppendLine("No workflows found.");
            return builder.ToString();
        }

        foreach (WorkflowRuntimeSummary summary in summaries)
        {
            builder.AppendLine(summary.WorkflowName);
            builder.AppendLine(new string('-', summary.WorkflowName.Length));
            builder.AppendLine($"Execution Id: {summary.WorkflowExecutionId}");
            builder.AppendLine($"Correlation Id: {summary.CorrelationId}");
            builder.AppendLine($"Status: {summary.Status}");
            builder.AppendLine($"Operation Status: {summary.OperationStatus}");
            builder.AppendLine($"Current Step Order: {summary.CurrentStepOrder}");
            builder.AppendLine($"Last Completed Step Order: {summary.LastCompletedStepOrder}");
            builder.AppendLine($"Is Resumable: {summary.IsResumable}");
            builder.AppendLine($"Last Heartbeat UTC: {summary.LastHeartbeatUtc ?? "(none)"}");
            builder.AppendLine();
        }

        return builder.ToString();
    }

    public static string FormatWorkflowDetail(
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

        builder.AppendLine("Workflow Detail");
        builder.AppendLine("===============");
        builder.AppendLine();
        builder.AppendLine($"Workflow: {detail.Summary.WorkflowName}");
        builder.AppendLine($"Execution Id: {detail.Summary.WorkflowExecutionId}");
        builder.AppendLine($"Correlation Id: {detail.Summary.CorrelationId}");
        builder.AppendLine($"Status: {detail.Summary.Status}");
        builder.AppendLine($"Operation Status: {detail.Summary.OperationStatus}");
        builder.AppendLine($"Heartbeat Status: {heartbeat.Status}");
        builder.AppendLine($"Heartbeat Age: {heartbeat.Age?.ToString() ?? "(unknown)"}");
        builder.AppendLine($"Heartbeat Message: {heartbeat.Message}");
        builder.AppendLine($"Current Step Order: {detail.Summary.CurrentStepOrder}");
        builder.AppendLine($"Last Completed Step Order: {detail.Summary.LastCompletedStepOrder}");
        builder.AppendLine($"Is Resumable: {detail.Summary.IsResumable}");
        builder.AppendLine();

        builder.AppendLine("Runtime State JSON");
        builder.AppendLine("------------------");
        builder.AppendLine(string.IsNullOrWhiteSpace(detail.Summary.RuntimeStateJson)
            ? "{}"
            : detail.Summary.RuntimeStateJson);
        builder.AppendLine();

        builder.AppendLine("Steps");
        builder.AppendLine("-----");
        builder.AppendLine();

        if (detail.Steps.Count == 0)
        {
            builder.AppendLine("No steps have been recorded.");
            return builder.ToString();
        }

        foreach (WorkflowRuntimeStepSummary step in detail.Steps.OrderBy(static item => item.StepOrder))
        {
            builder.AppendLine($"[{step.StepOrder}] {step.StepName}");
            builder.AppendLine($"Command: {step.CommandName}");
            builder.AppendLine($"Status: {step.Status}");
            builder.AppendLine($"Completed UTC: {step.CompletedUtc ?? "(not completed)"}");
            builder.AppendLine($"Command Execution Id: {step.CommandExecutionId ?? "(none)"}");
            builder.AppendLine($"Message: {step.Message}");
            builder.AppendLine();
        }

        return builder.ToString();
    }
}
