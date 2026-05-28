namespace Codex.CommandEngine.Data;

public sealed class WorkflowDefinitionStep
{
    public required string StepName { get; init; }

    public int StepOrder { get; init; }

    public required string CommandName { get; init; }

    public IReadOnlyDictionary<string, object?> Parameters { get; init; } =
        new Dictionary<string, object?>();
}
