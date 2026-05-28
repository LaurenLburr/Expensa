namespace Codex.CommandEngine.Core;

public sealed class WorkflowStepExecutionRequest
{
    public required string StepName { get; init; }

    public required string CommandName { get; init; }

    public int StepOrder { get; init; }

    public IReadOnlyDictionary<string, object?> Parameters { get; init; } =
        new Dictionary<string, object?>();
}
