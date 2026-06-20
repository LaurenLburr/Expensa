using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetScreenProviderStructureTests
{
    [Fact]
    public void BudgetScreenProvider_LoadsMonthRowsAndTransactions()
    {
        string text = TestPathHelper.ReadExtensionsFile(
            "Modules",
            "BudgetsAddin",
            "BudgetScreenRuntimeRunner.cs");

        Assert.Contains("BudgetMonthRow", text);
        Assert.Contains("Txn", text);
        Assert.Contains("BudgetMonthId", text);
        Assert.Contains("HierarchyLevel", text);
        Assert.Contains("'budget:' || bmr.[BudgetMonthRowId] AS [NodeId]", text);
        Assert.Contains("'transaction:' || t.[TransactionId] AS [NodeId]", text);
        Assert.Contains("ParentNodeId", text);
        Assert.Contains("IsHierarchical = true", text);
        Assert.Contains("Rows = ToRows(source)", text);
        Assert.DoesNotContain("ShiftChildRows", text);
    }
}
