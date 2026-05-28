namespace Codex.CommandEngine.Data;

public sealed record WorkflowDefinitionRecord(
    string WorkflowDefinitionId,
    string WorkflowName,
    string DisplayName,
    string Description,
    int Version,
    bool IsEnabled,
    string MetadataJson,
    string CreatedUtc,
    string? UpdatedUtc);
