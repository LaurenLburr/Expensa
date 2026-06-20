using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsHierarchyWholeRowIndentationTests
{
    [Fact]
    public void VerificationForm_LoadsBudgetRowsIntoCustomCollapsibleGrid()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains(
            "BudgetMonthQueryResult queryResult =",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "LoadMatchingBudgetMonthRows(GetMemoryConnection(), payload)",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "SetBudgetGridData(",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "SafeDataGridViewBinding.Sanitize(",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "queryResult.Rows",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "RenderBudgetGridRows();",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void VerificationForm_ReportsGridRowCountFromQueryResult()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains(
            "Grid rows shown:",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "queryResult.Rows.Rows.Count",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "Budget rows and matching transactions are loaded into the grid",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void VerificationForm_HostsWpfGridForRowCollapse()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.DoesNotContain(
            "CreateShiftedDisplayTable(source)",
            text,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "int destinationOffset =",
            text,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "SetHierarchicalGridData(",
            text,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "HideHierarchyIdentityColumns();",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "BudgetTransactionWpfGridControl",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "ElementHost",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "_budgetRowsGrid.LoadRows(_budgetRowsTable);",
            text,
            StringComparison.Ordinal);
    }
}
