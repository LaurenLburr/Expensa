namespace Codex.CommandEngine.Data;

public sealed record WorkflowStepExecutionRecord(
    string WorkflowStepExecutionId,
    string WorkflowExecutionId,
    string StepName,
    int StepOrder,
    string CommandName,
    string Status,
    string StartedUtc,
    string? CompletedUtc,
    string Message,
    string? CommandExecutionId);
