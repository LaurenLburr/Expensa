using System.Collections.Generic;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class CommandMenuConfigDocument
{
    public List<CommandMenuConfigEntry> Commands { get; init; } = new();
}
