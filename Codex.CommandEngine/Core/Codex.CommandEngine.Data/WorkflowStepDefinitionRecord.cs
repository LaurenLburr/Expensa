namespace Codex.CommandEngine.Data;

public sealed record WorkflowStepDefinitionRecord(
    string WorkflowDefinitionStepId,
    string WorkflowDefinitionId,
    string StepName,
    int StepOrder,
    string CommandName,
    string ParametersJson)
{
    public bool IsEnabled { get; init; } = true;

    public string CommandDefinitionId { get; init; } = CommandName;

    public string InputMapJson { get; init; } = ParametersJson;
}
