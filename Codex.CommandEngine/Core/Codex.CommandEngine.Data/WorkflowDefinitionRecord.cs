namespace Codex.CommandEngine.Data;

public sealed record WorkflowDefinitionRecord(
    string WorkflowDefinitionId,
    string WorkflowName,
    string DisplayName,
    string Description,
    int Version,
    bool IsActive)
{
    public WorkflowDefinitionRecord(
        string workflowDefinitionId,
        string workflowName,
        string displayName,
        string description,
        int version,
        bool isActive,
        string createdUtc,
        string? updatedUtc,
        string metadataJson)
        : this(
            workflowDefinitionId,
            workflowName,
            displayName,
            description,
            version,
            isActive)
    {
        CreatedUtc = createdUtc;
        UpdatedUtc = updatedUtc;
        MetadataJson = metadataJson;
    }

    public bool IsEnabled => IsActive;

    public string CreatedUtc { get; init; } = string.Empty;

    public string? UpdatedUtc { get; init; }

    public string MetadataJson { get; init; } = "{}";
}
