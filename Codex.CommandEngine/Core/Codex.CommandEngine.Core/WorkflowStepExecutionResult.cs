namespace Codex.CommandEngine.Core;

public sealed class WorkflowStepExecutionResult
{
    public required string StepName { get; init; }

    public required string CommandName { get; init; }

    public int StepOrder { get; init; }

    public WorkflowStepExecutionStatus Status { get; init; }

    public string Message { get; init; } = string.Empty;

    public CommandExecutionResult? CommandResult { get; init; }
}
