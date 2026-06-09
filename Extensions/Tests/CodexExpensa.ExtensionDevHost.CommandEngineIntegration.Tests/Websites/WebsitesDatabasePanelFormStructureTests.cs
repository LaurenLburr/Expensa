using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesDatabasePanelFormStructureTests
{
    [Fact]
    public void Designer_IncludesExpectedDatabasePanelControls()
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

        Assert.Contains("websitesListView", text);
        Assert.Contains("searchTextBox", text);
        Assert.Contains("includeDisabledCheckBox", text);
        Assert.Contains("refreshButton", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
