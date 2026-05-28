namespace Codex.CommandEngine.Data;

public sealed class EngineContextUpsert
{
    public string ContextId { get; init; } = Guid.NewGuid().ToString("N");

    public required string ContextName { get; init; }

    public string Scope { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string ContextJson { get; init; } = "{}";

    public string MetadataJson { get; init; } = "{}";

    public bool IsEnabled { get; init; } = true;
}
