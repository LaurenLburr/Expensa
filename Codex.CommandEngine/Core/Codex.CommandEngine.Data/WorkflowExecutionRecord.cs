namespace Codex.CommandEngine.Data;

public sealed record WorkflowExecutionRecord(
    string WorkflowExecutionId,
    string WorkflowName,
    string CorrelationId,
    string Status,
    string StartedUtc,
    string? CompletedUtc,
    int CurrentStepOrder,
    string ContextJson,
    string Message);
