using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost.Commands.Abstractions;

public interface ICommandContext
{
    Form Owner { get; }

    object? SelectedNode { get; }

    string? ProjectName { get; }

    string? ActiveDatabasePath { get; }

    ICommandServiceProvider Services { get; }
}
