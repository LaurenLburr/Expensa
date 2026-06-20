using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class AddinRuntimeDatabasePathServiceTests
{
    [Theory]
    [InlineData("WebsitesAddin", "websites.current.db")]
    [InlineData("BudgetsAddin", "budgets.current.db")]
    [InlineData("PayeesAddin", "payees.current.db")]
    public void GetCurrentDatabaseFileName_UsesAddinCurrentDatabaseNaming(string addinId, string expectedFileName)
    {
        Assert.Equal(expectedFileName, AddinRuntimeDatabasePathService.GetCurrentDatabaseFileName(addinId));
    }

    [Fact]
    public void GetRuntimeDatabaseLocation_UsesRuntimeFolderNotProdFolder()
    {
        AddinRuntimeDatabasePathService service = new();

        AddinRuntimeDatabaseLocation location =
            service.GetRuntimeDatabaseLocation("PayeesAddin");

        Assert.EndsWith(
            Path.Combine("Expensa", "Extensions", "Runtime", "PayeesAddin", "payees.current.db"),
            location.DatabasePath,
            StringComparison.OrdinalIgnoreCase);

        Assert.DoesNotContain(
            Path.Combine("CodexExpensa", "db", "codexexpensa.db"),
            location.DatabasePath,
            StringComparison.OrdinalIgnoreCase);
    }
    [Fact]
    public void GetDevCurrentDatabasePath_UsesLiteralCurrentDatabaseFileName()
    {
        AddinRuntimeDatabasePathService service = new();

        string path =
            service.GetDevCurrentDatabasePath("PayeesAddin");

        Assert.EndsWith(
            Path.Combine(
                "Extensions",
                "Modules",
                "PayeesAddin",
                "DevDatabase",
                "current.db"),
            path,
            StringComparison.OrdinalIgnoreCase);

        Assert.False(
            path.EndsWith(
                Path.Combine("DevDatabase", "payees.current.db"),
                StringComparison.OrdinalIgnoreCase));
    }

}
