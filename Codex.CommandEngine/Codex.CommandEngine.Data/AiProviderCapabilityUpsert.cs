namespace Codex.CommandEngine.Data;

public sealed class AiProviderCapabilityUpsert
{
    public string AiProviderCapabilityId { get; init; } = Guid.NewGuid().ToString("N");

    public required string AiProviderId { get; init; }

    public required string CapabilityName { get; init; }

    public string Description { get; init; } = string.Empty;

    public string MetadataJson { get; init; } = "{}";

    public bool IsEnabled { get; init; } = true;
}
