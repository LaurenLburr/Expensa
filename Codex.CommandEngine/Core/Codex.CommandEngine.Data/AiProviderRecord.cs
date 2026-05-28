namespace Codex.CommandEngine.Data;

public sealed record AiProviderRecord(
    string AiProviderId,
    string ProviderName,
    string DisplayName,
    string ProviderKind,
    string Description,
    string ConfigurationJson,
    string MetadataJson,
    bool IsEnabled,
    string CreatedUtc,
    string? UpdatedUtc);
