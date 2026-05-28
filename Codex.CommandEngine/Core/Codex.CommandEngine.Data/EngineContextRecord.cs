namespace Codex.CommandEngine.Data;

public sealed record EngineContextRecord(
    string ContextId,
    string ContextName,
    string Scope,
    string Description,
    string ContextJson,
    string MetadataJson,
    bool IsEnabled,
    string CreatedUtc,
    string? UpdatedUtc);
