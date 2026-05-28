namespace Codex.CommandEngine.Data;

public sealed record ExecutionStepHistoryCompletion(
    string ExecutionStepId,
    string Status,
    string CompletedUtc,
    string OutputJson,
    string ErrorMessage,
    string MetadataJson);
