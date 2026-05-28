namespace Codex.CommandEngine.Data;

public sealed record ExecutionStepHistoryStart(
    string ExecutionStepId,
    string ExecutionId,
    string? WorkflowStepDefinitionId,
    string? CommandDefinitionId,
    int StepOrder,
    string Status,
    string StartedUtc,
    string InputJson,
    string OutputJson,
    string ErrorMessage,
    string MetadataJson);
