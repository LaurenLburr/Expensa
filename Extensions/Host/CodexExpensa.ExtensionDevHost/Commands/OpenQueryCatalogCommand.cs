using System;
using System.IO;
using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class OpenQueryCatalogCommand : ExtMgrCommandBase
{
    public override string CommandKey => "Tools.QueryCatalog";

    public override string TopLevelMenu => "Tools";

    protected override string GetDefaultMenuText() => "Query Catalog";

    protected override int GetDefaultMenuOrder() => 300;

    protected override int GetDefaultItemOrder() => 200;

    public override void Execute(Form owner)
    {
        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "ExtensionMgr.db");

        using Form? dialog = CreateFormIfAvailable(
            owner,
            "CodexExpensa.ExtensionDevHost.UI.QueryCatalogForm",
            dbPath);

        if (dialog is null)
        {
            ShowNotAvailable(owner, "Query Catalog");
            return;
        }

        dialog.ShowDialog(owner);
    }
}
