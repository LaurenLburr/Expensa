namespace Codex.CommandEngine.Data;

public sealed record ExecutionHistoryRecord(
    string ExecutionId,
    string CorrelationId,
    string CommandName,
    string Status,
    string StartedUtc,
    string? CompletedUtc,
    long? DurationMilliseconds,
    string RequestJson,
    string OutputJson,
    string Message,
    string? ExceptionText,
    string ExecutionKind = "Command",
    string TargetId = "",
    string TargetName = "",
    string ErrorMessage = "");
