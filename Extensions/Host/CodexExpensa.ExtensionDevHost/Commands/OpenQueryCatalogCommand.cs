using System;
using System.IO;
using CodexExpensa.ExtensionDevHost.Commands.Abstractions;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class OpenQueryCatalogCommand : ExtMgrCommandBase
{
    public override string CommandKey => "Tools.QueryCatalog";

    public override string TopLevelMenu => "Tools";

    protected override string GetDefaultMenuText() => "Query Catalog";

    protected override int GetDefaultMenuOrder() => 300;

    protected override int GetDefaultItemOrder() => 200;

    public override void Execute(ICommandContext context)
    {
        var owner = GetOwner(context);

        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "ExtensionMgr.db");

        using var dialog = CreateFormIfAvailable(
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
