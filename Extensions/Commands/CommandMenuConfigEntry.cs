namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class CommandMenuConfigEntry
{
    public string CommandKey { get; init; } = string.Empty;

    public string MenuText { get; init; } = string.Empty;

    public int MenuOrder { get; init; }

    public int ItemOrder { get; init; }
}
