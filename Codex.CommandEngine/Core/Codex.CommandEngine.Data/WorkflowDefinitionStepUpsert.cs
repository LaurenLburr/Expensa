namespace Codex.CommandEngine.Data;

public sealed class WorkflowDefinitionStepUpsert
{
    public required string WorkflowDefinitionStepId { get; init; }

    public required string WorkflowDefinitionId { get; init; }

    public required string StepName { get; init; }

    public int StepOrder { get; init; }

    public required string CommandName { get; init; }

    public string ParametersJson { get; init; } = "{}";
}
