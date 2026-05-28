namespace Codex.CommandEngine.Data;

public sealed record ExecutionStepHistoryRecord(
    string ExecutionStepId,
    string ExecutionId,
    string? WorkflowStepDefinitionId,
    string? CommandDefinitionId,
    int StepOrder,
    string Status,
    string StartedUtc,
    string? CompletedUtc,
    string InputJson,
    string OutputJson,
    string ErrorMessage,
    string MetadataJson);
