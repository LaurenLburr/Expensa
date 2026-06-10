using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsDatabasePanelRuntimeCopyTests
{
    [Fact]
    public void BudgetsDatabasePanel_UsesAddinRuntimeDatabaseInsteadOfProdDatabase()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsDatabasePanelForm.cs");

        Assert.Contains("GetRuntimeDatabaseLocation(AddinId)", text);
        Assert.Contains("ReplaceRuntimeDatabaseFromProd(AddinId)", text);
        Assert.Contains("ReplaceRuntimeDatabaseFromDev(AddinId)", text);
        Assert.DoesNotContain("GetDefaultExpensaDatabasePath", text);
        Assert.DoesNotContain("LocalApplicationData", text);
        Assert.DoesNotContain("codexexpensa.db", text);
    }
}
