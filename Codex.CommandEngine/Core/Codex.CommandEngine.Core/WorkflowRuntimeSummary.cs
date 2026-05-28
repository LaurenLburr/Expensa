namespace Codex.CommandEngine.Core;

public sealed class WorkflowRuntimeSummary
{
    public required string WorkflowExecutionId { get; init; }

    public required string WorkflowName { get; init; }

    public required string CorrelationId { get; init; }

    public required string Status { get; init; }

    public WorkflowRuntimeOperationStatus OperationStatus { get; init; }

    public int CurrentStepOrder { get; init; }

    public int LastCompletedStepOrder { get; init; }

    public bool IsResumable { get; init; }

    public string? LastHeartbeatUtc { get; init; }

    public string RuntimeStateJson { get; init; } = "{}";
}
