namespace Codex.CommandEngine.Data;

public sealed record ExecutionHistoryStart(
    string ExecutionId,
    string ExecutionKind,
    string TargetId,
    string TargetName,
    string Status,
    string StartedUtc,
    string CorrelationId,
    string InputJson,
    string OutputJson,
    string ErrorMessage,
    string MetadataJson);
