using CodexExpensa.ExtensionDevHost.Commands.Abstractions;
using CodexExpensa.ExtensionDevHost.UI;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class OpenManageExtensionsCommand : ExtMgrCommandBase
{
    public override string CommandKey => "Tools.ManageExtensions";

    public override string TopLevelMenu => "Tools";

    protected override string GetDefaultMenuText() => "Manage Extensions";

    protected override int GetDefaultMenuOrder() => 300;

    protected override int GetDefaultItemOrder() => 320;

    public override void Execute(ICommandContext context)
    {
        var ui = GetUi(context);

        using ManageExtensionsForm dialog = new();
        ui.ShowDialog(dialog);
    }
}
