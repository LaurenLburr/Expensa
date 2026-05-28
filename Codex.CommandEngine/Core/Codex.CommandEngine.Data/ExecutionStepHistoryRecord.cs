namespace Codex.CommandEngine.Data;

public sealed record ExecutionStepHistoryRecord(
    string ExecutionStepHistoryId,
    string ExecutionId,
    int StepOrder,
    string Status,
    string StartedUtc,
    string? CompletedUtc,
    string? CommandDefinitionId,
    string? WorkflowStepDefinitionId,
    string ErrorMessage);
