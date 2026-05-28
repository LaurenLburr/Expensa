namespace Codex.CommandEngine.Core;

public sealed class WorkflowDefinitionValidationIssue
{
    public required string Code { get; init; }

    public required string Message { get; init; }

    public string? StepName { get; init; }

    public int? StepOrder { get; init; }
}
