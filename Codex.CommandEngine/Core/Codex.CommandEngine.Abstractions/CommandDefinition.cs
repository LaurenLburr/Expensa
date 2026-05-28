namespace Codex.CommandEngine.Abstractions;

public sealed class CommandDefinition
{
    public required string CommandName { get; init; }

    public required string DisplayName { get; init; }

    public string Description { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public int Version { get; init; } = 1;

    public bool IsEnabled { get; init; } = true;

    public string HandlerType { get; init; } = string.Empty;

    public IReadOnlyList<CommandParameterDefinition> Parameters { get; init; } = [];
}
