namespace Codex.CommandEngine.Data;

public sealed record AiProviderCapabilityRecord(
    string AiProviderCapabilityId,
    string AiProviderId,
    string CapabilityName,
    string Description,
    string MetadataJson,
    bool IsEnabled,
    string CreatedUtc,
    string? UpdatedUtc);
