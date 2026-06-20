using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsHierarchyGridIndentationTests
{
    [Fact]
    public void VerificationForm_BindsQueryRowsToVisibleGridInSqlOrder()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains(
            "SetBudgetGridData(",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "private readonly BudgetTransactionWpfGridControl _budgetRowsGrid = new();",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "_budgetRowsGrid.TransactionRowDoubleClicked",
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

        Assert.Contains(
            "_budgetRowsGrid.LoadRows(_budgetRowsTable);",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "ElementHost _budgetRowsHost",
            text,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "SetHierarchicalGridData(",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void VerificationForm_DoesNotChangeHierarchySql()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains(
            "Budgets.SelectBudgetMonthHierarchy",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "catalog.GetRequiredSqlText(BudgetMonthHierarchyQueryName)",
            text,
            StringComparison.Ordinal);
    }
}
