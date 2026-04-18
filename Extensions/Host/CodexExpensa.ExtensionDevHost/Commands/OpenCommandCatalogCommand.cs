using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.UI;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class OpenCommandCatalogCommand : ExtMgrCommandBase
{
    public override string CommandKey => "Tools.CommandCatalog";
    public override string TopLevelMenu => "Tools";
    protected override string GetDefaultMenuText() => "Command Catalog";

    public override void Execute(Form owner)
    {
        if (owner is not MainForm mainForm)
        {
            ShowNotAvailable(owner, "Command Catalog");
            return;
        }

        using CommandCatalogForm dialog = new(
            mainForm.CommandRegistry,
            mainForm.CommandConfigPath,
            mainForm.RebuildMenu
        );

        dialog.ShowDialog(owner);
    }
}
