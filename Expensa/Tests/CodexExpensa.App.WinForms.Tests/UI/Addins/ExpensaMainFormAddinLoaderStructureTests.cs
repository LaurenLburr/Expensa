using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class ExpensaMainFormAddinLoaderStructureTests
{
    [Fact]
    public void MainForm_UsesAggregateTreeAddinLoaderInsteadOfWebsiteOnlyLoader()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "MainForm.cs");

        Assert.Contains("TreeAddinTreeViewLoader", text);
        Assert.Contains("_treeAddinLoader", text);
        Assert.Contains("LoadTreeAddinsAsync", text);
        Assert.Contains("LoadAllIntoTreeViewAsync", text);

        Assert.DoesNotContain("private readonly WebsiteAddinTreeLoader _websiteTreeLoader", text);
        Assert.DoesNotContain("LoadWebsiteAddinTreeAsync", text);
    }

    [Fact]
    public void MainForm_LoadAndRefreshUseTreeAddins()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "MainForm.cs");

        Assert.Contains("await LoadTreeAddinsAsync();", text);
        Assert.Contains("SetStatus(\"Tree add-ins refreshed.\")", text);
        Assert.Contains("SetStatus(\"Loading tree add-ins...\")", text);
        Assert.Contains("Tree add-ins loaded:", text);
    }

    [Fact]
    public void MainForm_BudgetRootTagOpensBudgetsLanding()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "MainForm.cs");

        Assert.Contains("Budget.Root", text);
        Assert.Contains("ShowBudgetsLanding();", text);
        Assert.Contains("BudgetNav.TryParseMonth", text);
    }
}
