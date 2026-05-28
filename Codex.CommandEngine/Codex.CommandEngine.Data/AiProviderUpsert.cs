namespace Codex.CommandEngine.Data;

public sealed class AiProviderUpsert
{
    public string AiProviderId { get; init; } = Guid.NewGuid().ToString("N");

    public required string ProviderName { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public string ProviderKind { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string ConfigurationJson { get; init; } = "{}";

    public string MetadataJson { get; init; } = "{}";

    public bool IsEnabled { get; init; } = true;
}
