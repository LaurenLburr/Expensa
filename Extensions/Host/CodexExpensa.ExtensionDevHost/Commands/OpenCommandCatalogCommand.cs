using CodexExpensa.ExtensionDevHost.Commands.Abstractions;
using CodexExpensa.ExtensionDevHost.Commands.Services;
using CodexExpensa.ExtensionDevHost.UI;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class OpenCommandCatalogCommand : ExtMgrCommandBase
{
    public override string CommandKey => "Tools.CommandCatalog";
    public override string TopLevelMenu => "Tools";
    protected override string GetDefaultMenuText() => "Command Catalog";

    public override void Execute(ICommandContext context)
    {
        var ui = GetUi(context);
        var logger = context.Services.GetRequiredService<ICommandLogger>();
        logger.Log("OpenCommandCatalogCommand executed");

        if (ui.Owner is not MainForm mainForm)
        {
            ShowNotAvailable(ui.Owner, "Command Catalog");
            return;
        }

        using CommandCatalogForm dialog = new(
            mainForm.CommandRegistry,
            mainForm.CommandConfigPath,
            mainForm.RebuildMenu
        );

        ui.ShowDialog(dialog);
    }
}
