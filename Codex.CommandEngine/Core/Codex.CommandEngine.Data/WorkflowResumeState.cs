namespace Codex.CommandEngine.Data;

public sealed record WorkflowResumeState(
    string WorkflowExecutionId,
    string WorkflowName,
    string CorrelationId,
    string Status,
    int CurrentStepOrder,
    int LastCompletedStepOrder,
    bool IsResumable,
    string ResumeToken,
    string RuntimeStateJson,
    string? LastHeartbeatUtc);
