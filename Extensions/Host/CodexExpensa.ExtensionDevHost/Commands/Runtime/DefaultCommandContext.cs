using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.Commands.Abstractions;

namespace CodexExpensa.ExtensionDevHost.Commands.Runtime;

public sealed class DefaultCommandContext : ICommandContext
{
    public required Form Owner { get; init; }

    public object? SelectedNode { get; init; }

    public string? ProjectName { get; init; }

    public string? ActiveDatabasePath { get; init; }

    public required ICommandServiceProvider Services { get; init; }
}
