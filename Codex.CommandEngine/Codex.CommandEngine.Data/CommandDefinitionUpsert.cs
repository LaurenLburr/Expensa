namespace Codex.CommandEngine.Data;

public sealed class CommandDefinitionUpsert
{
    public string CommandDefinitionId { get; init; } = Guid.NewGuid().ToString("N");

    public required string CommandName { get; init; }

    public required string DisplayName { get; init; }

    public string Description { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public int Version { get; init; } = 1;

    public bool IsEnabled { get; init; } = true;

    public string HandlerType { get; init; } = string.Empty;
}
