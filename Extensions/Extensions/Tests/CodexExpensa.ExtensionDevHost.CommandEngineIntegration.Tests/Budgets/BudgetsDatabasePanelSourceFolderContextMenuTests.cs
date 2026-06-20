using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsDatabasePanelSourceFolderContextMenuTests
{
    [Fact]
    public void BudgetsDatabasePanel_AddsSourceFolderContextMenusToExistingUpdateLinks()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsDatabasePanelForm.cs");

        Assert.Contains("ConfigureSourceFolderContextMenus();", text);
        Assert.Contains("linkUpdate_from_Prod.ContextMenuStrip", text);
        Assert.Contains("linkUpdate_from_Dev.ContextMenuStrip", text);
        Assert.Contains("Open Prod folder location", text);
        Assert.Contains("Open Dev folder location", text);
        Assert.Contains("ToolStripMenuItem reloadItem = new(\"Reload\")", text);
        Assert.Contains("OnUpdateFromProdClicked", text);
        Assert.Contains("OnUpdateFromDevClicked", text);
        Assert.Contains("menu.Items.Add(new ToolStripSeparator())", text);
        Assert.Contains("_pathService.GetProdDatabasePath", text);
        Assert.Contains("_pathService.GetDevCurrentDatabasePath(AddinId)", text);
        Assert.Contains("UseShellExecute = true", text);
    }

    [Fact]
    public void BudgetsDatabasePanel_DoesNotRequireTemplateModificationForContextMenus()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsDatabasePanelForm.cs");

        Assert.Contains("CreateSourceFolderContextMenu", text);
        Assert.Contains("OpenSourceDatabaseFolder", text);
    }
}
