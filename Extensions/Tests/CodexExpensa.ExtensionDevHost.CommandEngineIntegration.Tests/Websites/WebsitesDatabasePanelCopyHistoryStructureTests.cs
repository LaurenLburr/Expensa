using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesDatabasePanelCopyHistoryStructureTests
{
    [Fact]
    public void Designer_IncludesCopiedDatabaseHistoryControls()
    {
        string repositoryRoot = FindRepositoryRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesDatabasePanelForm.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text = File.ReadAllText(designerPath);

        Assert.Contains("copiedDatabasesListView", text);
        Assert.Contains("copiedDatabasePathTextBox", text);
        Assert.Contains("openCopiedDatabaseFolderLinkLabel", text);
        Assert.Contains("Open copied DB folder", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
