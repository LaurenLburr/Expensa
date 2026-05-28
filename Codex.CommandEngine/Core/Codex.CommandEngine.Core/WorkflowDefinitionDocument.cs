namespace Codex.CommandEngine.Core;

public sealed class WorkflowDefinitionDocument
{
    public required string WorkflowDefinitionId { get; init; }

    public required string WorkflowName { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int Version { get; init; } = 1;

    public bool IsActive { get; init; } = true;

    public IReadOnlyList<WorkflowDefinitionStep> Steps { get; init; } =
        [];
}
