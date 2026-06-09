using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class TreeAddinTreeNodeFactoryStructureTests
{
    [Fact]
    public void TreeNodeFactory_UsesExistingWebsiteParserAndRenderer()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinTreeNodeFactory.cs");

        Assert.Contains("WebsiteLoadResultParser.Parse", text);
        Assert.Contains("WebsiteTreeViewRenderer.Render", text);
        Assert.Contains("CloneNode", text);
        Assert.Contains("root.Text = \"Websites\"", text);
    }

    [Fact]
    public void TreeNodeFactory_ConvertsBudgetMonthsToExistingExpensaBudgetTags()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinTreeNodeFactory.cs");

        Assert.Contains("CreateBudgetsRootNode", text);
        Assert.Contains("Budget.Root", text);
        Assert.Contains("Budget.Month.", text);
        Assert.Contains("budgetYear", text);
        Assert.Contains("budgetMonth", text);
    }

    [Fact]
    public void TreeNodeFactory_CreatesFailureNodesForFailedAddins()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinTreeNodeFactory.cs");

        Assert.Contains("CreateFailureNode", text);
        Assert.Contains("failed", text);
        Assert.Contains("ToolTipText = exception.Message", text);
    }
}
