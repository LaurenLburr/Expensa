using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsSqlCatalogUsageTests
{
    [Fact]
    public void BudgetHierarchy_LoadsNamedQueryFromSqlCatalog()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains("Budgets.SelectBudgetMonthHierarchy", text, StringComparison.Ordinal);
        Assert.Contains("GetRequiredSqlText(BudgetMonthHierarchyQueryName)", text, StringComparison.Ordinal);
        Assert.DoesNotContain("WITH [SelectedBudgetRows]", text, StringComparison.Ordinal);
        Assert.DoesNotContain("UNION ALL", text, StringComparison.Ordinal);
    }

    [Fact]
    public void BudgetHierarchy_BindsCatalogQueryParameters()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains("@BudgetMonthId", text, StringComparison.Ordinal);
        Assert.Contains("@MaximumRows", text, StringComparison.Ordinal);
    }

    [Fact]
    public void BudgetHierarchy_CatalogPatchReturnsPayeeColumnsForGrid()
    {
        string text = TestPathHelper.ReadModuleFile(
            "BudgetsAddin",
            "DevDatabase",
            "004_Update_BudgetMonthHierarchy_Payee.sql");

        Assert.Contains("'Budgets.SelectBudgetMonthHierarchy'", text, StringComparison.Ordinal);
        Assert.Contains("[SelectedBudgetRows].[Payee],", text, StringComparison.Ordinal);
        Assert.Contains("[SelectedBudgetRows].[PayeeId],", text, StringComparison.Ordinal);
        string normalized =
            text.Replace("\r\n", "\n", StringComparison.Ordinal);

        Assert.Contains("[Item],\n    [Payee],\n    [Status]", normalized, StringComparison.Ordinal);
        Assert.Contains("[BudgetMonthRowId],\n    [PayeeId],\n    [TransactionId]", normalized, StringComparison.Ordinal);
        Assert.Contains("[SelectedBudgetRows].[BudgetMonthRowId]\n            AS [BudgetRowSortKey]", normalized, StringComparison.Ordinal);
        Assert.Contains("0 AS [RowSortIndex]", normalized, StringComparison.Ordinal);
        Assert.Contains("1 AS [RowSortIndex]", normalized, StringComparison.Ordinal);
        Assert.Contains("[BudgetRowSortKey],\n    [RowSortIndex],\n    [ChildSortIndex]", normalized, StringComparison.Ordinal);
    }

    [Fact]
    public void BudgetTestData_ReplacesStaleMonthRowsAndMapsRowsToPayees()
    {
        string text = TestPathHelper.ReadModuleFile(
            "BudgetsAddin",
            "DevDatabase",
            "BudgetsTestData.sql");

        Assert.Contains("BudgetMonthRowId LIKE 'test-month-row-%'", text, StringComparison.Ordinal);
        Assert.Contains("INSERT INTO BudgetMonthPayee", text, StringComparison.Ordinal);
        Assert.Contains("'BudgetMonthRows missing Payee' AS Verification", text, StringComparison.Ordinal);
        Assert.Contains("ON p.PayeeName = bmr.Name", text, StringComparison.Ordinal);
        Assert.Contains("p.PayeeId IS NULL", text, StringComparison.Ordinal);
    }
}
