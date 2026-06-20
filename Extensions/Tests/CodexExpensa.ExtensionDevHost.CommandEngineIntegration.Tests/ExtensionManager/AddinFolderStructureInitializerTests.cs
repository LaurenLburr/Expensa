using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class AddinFolderStructureInitializerTests
{
    [Theory]
    [InlineData("payees", "PayeesAddin")]
    [InlineData("websites", "WebsitesAddin")]
    [InlineData("budgets", "BudgetsAddin")]
    [InlineData("PayeesAddin", "PayeesAddin")]
    public void NormalizeModuleAddinId_ReturnsModuleFolderName(
        string registeredAddinId,
        string expected)
    {
        string actual =
            AddinFolderStructureInitializer.NormalizeModuleAddinId(
                registeredAddinId);

        Assert.Equal(expected, actual);
    }
}
