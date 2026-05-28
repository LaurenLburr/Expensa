namespace Codex.CommandEngine.Data;

public sealed class AiProviderUpsert
{
    public AiProviderUpsert()
    {
    }

    public AiProviderUpsert(
        string aiProviderId,
        string providerName,
        string displayName,
        string providerKind,
        string description,
        string configurationJson,
        string metadataJson,
        bool isEnabled)
    {
        AiProviderId = aiProviderId;
        ProviderName = providerName;
        DisplayName = displayName;
        ProviderKind = providerKind;
        Description = description;
        ConfigurationJson = configurationJson;
        MetadataJson = metadataJson;
        IsEnabled = isEnabled;
    }

    public string AiProviderId { get; init; } = Guid.NewGuid().ToString("N");

    public required string ProviderName { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public string ProviderKind { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string ConfigurationJson { get; init; } = "{}";

    public string MetadataJson { get; init; } = "{}";

    public bool IsEnabled { get; init; } = true;
}
