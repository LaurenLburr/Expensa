namespace Codex.CommandEngine.Data;

public sealed record CommandDefinitionRecord(
    string CommandDefinitionId,
    string CommandName,
    string DisplayName,
    string Description,
    string Category,
    int Version,
    bool IsEnabled,
    string HandlerType,
    string CreatedUtc,
    string? UpdatedUtc);
