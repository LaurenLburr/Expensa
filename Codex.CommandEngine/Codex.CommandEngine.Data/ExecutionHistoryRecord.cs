namespace Codex.CommandEngine.Data;

public sealed record ExecutionHistoryRecord(
    string ExecutionId,
    string ExecutionKind,
    string TargetId,
    string TargetName,
    string Status,
    string StartedUtc,
    string? CompletedUtc,
    string CorrelationId,
    string InputJson,
    string OutputJson,
    string ErrorMessage,
    string MetadataJson);
