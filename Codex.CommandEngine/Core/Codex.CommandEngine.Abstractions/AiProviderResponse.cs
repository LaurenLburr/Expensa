namespace Codex.CommandEngine.Abstractions;

public sealed class AiProviderResponse
{
    public required string ResponseText { get; init; }

    public string RawResponseJson { get; init; } = "{}";

    public string MetadataJson { get; init; } = "{}";
}
