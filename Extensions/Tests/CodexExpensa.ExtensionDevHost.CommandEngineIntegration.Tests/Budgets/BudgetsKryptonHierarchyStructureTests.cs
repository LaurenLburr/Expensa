using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsKryptonHierarchyStructureTests
{
    [Fact]
    public void VerificationForm_BindsBudgetRowsToVisibleGrid()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains(
            "SafeDataGridViewBinding.Sanitize(",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "queryResult.Rows",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "SetBudgetGridData(",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "private readonly BudgetTransactionWpfGridControl _budgetRowsGrid = new();",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "private readonly ElementHost _budgetRowsHost = new();",
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
    }

    [Fact]
    public void VerificationForm_LoadsBudgetRowsAndTransactionsFromHierarchyQuery()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains(
            "LoadMatchingBudgetMonthRows(GetMemoryConnection(), payload)",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "BudgetMonthHierarchyQueryName",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "Budgets.SelectBudgetMonthHierarchy",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void VerificationForm_ReportsRowsShownInNotes()
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
    }

    [Fact]
    public void VerificationForm_DoesNotUseShiftedIndentationForBudgetRows()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.DoesNotContain(
            "CreateIndentedHierarchyTable(queryResult.Rows)",
            text,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "int destinationOffset =",
            text,
            StringComparison.Ordinal);
    }
}
