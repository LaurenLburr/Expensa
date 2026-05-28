namespace Codex.CommandEngine.Core;

public sealed class RuntimeCommandDescriptor
{
    public required string CommandName { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public int Version { get; init; } = 1;

    public bool IsEnabled { get; init; } = true;
}
