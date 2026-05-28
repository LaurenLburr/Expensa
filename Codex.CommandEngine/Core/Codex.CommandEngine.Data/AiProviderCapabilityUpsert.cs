namespace Codex.CommandEngine.Data;

public sealed class AiProviderCapabilityUpsert
{
    public AiProviderCapabilityUpsert()
    {
    }

    public AiProviderCapabilityUpsert(
        string aiProviderCapabilityId,
        string aiProviderId,
        string capabilityName,
        string capabilityKind,
        string description,
        string metadataJson,
        bool isEnabled)
    {
        AiProviderCapabilityId = aiProviderCapabilityId;
        AiProviderId = aiProviderId;
        CapabilityName = capabilityName;
        CapabilityKind = capabilityKind;
        Description = description;
        MetadataJson = metadataJson;
        IsEnabled = isEnabled;
    }

    public string AiProviderCapabilityId { get; init; } = Guid.NewGuid().ToString("N");

    public required string AiProviderId { get; init; }

    public required string CapabilityName { get; init; }

    public string CapabilityKind { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string MetadataJson { get; init; } = "{}";

    public bool IsEnabled { get; init; } = true;
}
