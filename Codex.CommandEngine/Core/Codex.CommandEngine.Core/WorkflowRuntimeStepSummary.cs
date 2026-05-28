namespace Codex.CommandEngine.Core;

public sealed class WorkflowRuntimeStepSummary
{
    public required string WorkflowStepExecutionId { get; init; }

    public required string StepName { get; init; }

    public int StepOrder { get; init; }

    public required string CommandName { get; init; }

    public required string Status { get; init; }

    public string? CompletedUtc { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? CommandExecutionId { get; init; }
}
