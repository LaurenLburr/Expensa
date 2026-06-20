using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class ExpensaMainFormAddinLoaderStructureTests
{
    [Fact]
    public void MainForm_UsesTreeAddinLoaderForRefresh()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI",
            "MainForm.cs");

        Assert.Contains(
            "TreeAddinTreeViewLoader",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "_treeAddinLoader.LoadAllIntoTreeViewAsync(",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "TreeAddinTreeNodeFactory.CreateRootNode",
            TestRepositoryPath.ReadExpensaFile(
                "UI",
                "Addins",
                "TreeAddinTreeViewLoader.cs"),
            StringComparison.Ordinal);
    }

    [Fact]
    public void MainForm_BudgetNodesOpenExtensionProvidedScreens()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI",
            "MainForm.cs");

        int branchStart =
            text.IndexOf(
                "if (e.Node.Tag is BudgetTreeNodePayload budgetPayload)",
                StringComparison.Ordinal);

        Assert.True(
            branchStart >= 0,
            "The typed Budget add-in selection branch was not found.");

        int branchEnd =
            text.IndexOf(
                "if (e.Node.Tag is PayeeTreeNodePayload payeePayload)",
                branchStart,
                StringComparison.Ordinal);

        Assert.True(
            branchEnd > branchStart,
            "The end of the typed Budget add-in selection branch was not found.");

        string budgetBranch =
            text[branchStart..branchEnd];

        Assert.Contains(
            "ShowAddinScreenAsync(",
            budgetBranch,
            StringComparison.Ordinal);

        Assert.Contains(
            "TreeAddinKind.Budgets",
            budgetBranch,
            StringComparison.Ordinal);

        Assert.Contains(
            "Year = budgetPayload.Year",
            budgetBranch,
            StringComparison.Ordinal);

        Assert.Contains(
            "Month = budgetPayload.Month",
            budgetBranch,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "ShowBudgetsLanding();",
            budgetBranch,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "ShowBudgetMonth(",
            budgetBranch,
            StringComparison.Ordinal);
    }
}
