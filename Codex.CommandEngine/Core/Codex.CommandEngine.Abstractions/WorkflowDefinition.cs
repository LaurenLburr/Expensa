namespace Codex.CommandEngine.Abstractions;

public sealed record WorkflowDefinition(
    string WorkflowId,
    string WorkflowName,
    string DisplayName,
    string Description,
    int Version,
    bool IsEnabled,
    string MetadataJson);
