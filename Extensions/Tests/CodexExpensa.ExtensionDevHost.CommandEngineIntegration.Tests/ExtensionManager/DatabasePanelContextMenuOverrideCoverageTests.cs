using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class DatabasePanelContextMenuOverrideCoverageTests
{
    [Theory]
    [InlineData("Payees", "PayeesDatabasePanelForm.cs")]
    [InlineData("Websites", "WebsitesDatabasePanelForm.cs")]
    public void DatabasePanel_OverridesSharedContextMenuOperations(
        string featureFolder,
        string fileName)
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            featureFolder,
            fileName);

        Assert.Contains(
            "protected override void ReloadAddinDatabaseIntoMemory()",
            text);

        Assert.Contains(
            "protected override void OpenProdDatabaseFolder()",
            text);

        Assert.Contains(
            "protected override void OpenDevDatabaseFolder()",
            text);

        Assert.Contains(
            "_pathService.GetProdDatabasePath()",
            text);

        Assert.Contains(
            "_pathService.GetDevCurrentDatabasePath(AddinId)",
            text);

        Assert.Contains(
            "FolderLauncher.OpenContainingFolder(_currentDatabasePath)",
            text);
    }
}
