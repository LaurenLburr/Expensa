using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class TreeAddinTreeNodeFactoryStructureTests
{
    [Fact]
    public void TreeNodeFactory_UsesExistingWebsiteParserAndRenderer()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI",
            "Addins",
            "TreeAddinTreeNodeFactory.cs");

        Assert.Contains(
            "WebsiteLoadResultParser.Parse",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "WebsiteTreeViewRenderer.Render",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "CloneNode",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "root.Text = \"Websites\"",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void TreeNodeFactory_ConvertsBudgetNodesToTypedPayloads()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI",
            "Addins",
            "TreeAddinTreeNodeFactory.cs");

        Assert.Contains(
            "CreateBudgetsRootNode",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "new BudgetTreeNodePayload(",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "GetString(element, \"nodeId\")",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "GetString(element, \"nodeType\")",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "budgetYear",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "budgetMonth",
            text,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "Budget.Month.",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void TreeNodeFactory_CreatesDetailedFailureNodesForFailedAddins()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI",
            "Addins",
            "TreeAddinTreeNodeFactory.cs");

        Assert.Contains(
            "CreateFailureNode",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "failed",
            text,
            StringComparison.OrdinalIgnoreCase);

        Assert.Contains(
            "ToolTipText = exception.ToString()",
            text,
            StringComparison.Ordinal);
    }
}
