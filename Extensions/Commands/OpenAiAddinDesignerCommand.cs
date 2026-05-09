using CodexExpensa.ExtensionDevHost.Commands.Abstractions;
using CodexExpensa.ExtensionDevHost.UI;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class OpenAiAddinDesignerCommand : ExtMgrCommandBase
{
    public override string CommandKey => "Tools.AiAddinDesigner";
    public override string TopLevelMenu => "AI";
    protected override string GetDefaultMenuText() => "AI Add-in Designer";
    protected override int GetDefaultMenuOrder() => 400;
    protected override int GetDefaultItemOrder() => 410;

    public override void Execute(ICommandContext context)
    {
        var ui = GetUi(context);
        using AiAddinDesignerForm dialog = new();
        ui.ShowDialog(dialog);
    }
}
