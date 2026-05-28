namespace Codex.CommandEngine.Abstractions;

public sealed class CommandRequest
{
    public required string CommandName { get; init; }

    public Dictionary<string, string> Parameters { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}
