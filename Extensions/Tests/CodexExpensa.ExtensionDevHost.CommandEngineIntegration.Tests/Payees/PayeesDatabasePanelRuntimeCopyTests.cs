using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Payees;

public sealed class PayeesDatabasePanelRuntimeCopyTests
{
    [Fact]
    public void PayeesDatabasePanel_UsesAddinRuntimeDatabaseInsteadOfProdDatabase()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesDatabasePanelForm.cs");

        Assert.Contains("GetRuntimeDatabaseLocation(AddinId)", text);
        Assert.Contains("ReplaceRuntimeDatabaseFromProd(AddinId)", text);
        Assert.Contains("ReplaceRuntimeDatabaseFromDev(AddinId)", text);
        Assert.DoesNotContain("GetDefaultExpensaDatabasePath", text);
        Assert.DoesNotContain("LocalApplicationData", text);
        Assert.DoesNotContain("codexexpensa.db", text);
    }
}
