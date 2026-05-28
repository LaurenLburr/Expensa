namespace Codex.CommandEngine.Data;

public sealed record WorkflowDefinitionUpsert(
    string WorkflowDefinitionId,
    string WorkflowName,
    string DisplayName,
    string Description,
    int Version,
    bool IsEnabled,
    string MetadataJson);
