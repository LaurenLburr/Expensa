namespace Codex.CommandEngine.Abstractions;

public sealed class AiProviderRequest
{
    public required string Prompt { get; init; }

    public string ContextJson { get; init; } = "{}";

    public string MetadataJson { get; init; } = "{}";
}
