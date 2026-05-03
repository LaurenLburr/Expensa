using CodexExpensa.ExtensionDevHost.Commands.Abstractions;
using CodexExpensa.ExtensionDevHost.UI;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class NewAddinProjectCommand : ExtMgrCommandBase
{
    public override string CommandKey => "Project.NewAddinProject";
    public override string TopLevelMenu => "Project";
    protected override string GetDefaultMenuText() => "New Add-in Project";
    protected override int GetDefaultMenuOrder() => 100;
    protected override int GetDefaultItemOrder() => 10;

    public override void Execute(ICommandContext context)
    {
        var ui = GetUi(context);

        using NewAddinProjectForm dialog = new();
        ui.ShowDialog(dialog);
    }
}
