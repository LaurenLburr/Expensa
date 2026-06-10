using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesDatabasePanelRuntimeCopyTests
{
    [Fact]
    public void WebsitesDatabasePanel_UsesAddinRuntimeDatabaseInsteadOfProdDatabase()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "WebsitesDatabasePanelForm.cs");

        Assert.Contains("GetRuntimeDatabaseLocation(AddinId)", text);
        Assert.Contains("ReplaceRuntimeDatabaseFromProd(AddinId)", text);
        Assert.Contains("ReplaceRuntimeDatabaseFromDev(AddinId)", text);
        Assert.DoesNotContain("GetDefaultExpensaDatabasePath", text);
        Assert.DoesNotContain("LocalApplicationData", text);
        Assert.DoesNotContain("codexexpensa.db", text);
    }
}
