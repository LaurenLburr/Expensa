namespace Codex.CommandEngine.Data;

public sealed record ExecutionHistoryCompletion(
    string ExecutionId,
    string Status,
    string CompletedUtc,
    string OutputJson,
    string ErrorMessage,
    string MetadataJson);
