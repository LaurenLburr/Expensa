using System.Text.Json.Serialization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifest
{
    public string ExtensionId { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    [JsonPropertyName("displaySort")]
    public int DisplaySort { get; init; }

    public string Version { get; init; } = string.Empty;

    public string AssemblyFile { get; init; } = string.Empty;

    public string ProviderType { get; init; } = string.Empty;

    public string MinimumHostVersion { get; init; } = string.Empty;

    public bool Enabled { get; init; } = true;

    public string Description { get; init; } = string.Empty;
}
