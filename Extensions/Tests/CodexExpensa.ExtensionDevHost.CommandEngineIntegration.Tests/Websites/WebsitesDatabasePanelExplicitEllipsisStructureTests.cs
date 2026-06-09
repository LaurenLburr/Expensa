using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesDatabasePanelExplicitEllipsisStructureTests
{
    [Fact]
    public void Form_StillDisplaysDatabaseNameAndPathControls()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesDatabasePanelForm.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text =
            File.ReadAllText(designerPath);

        Assert.Contains("databaseNameLinkLabel", text);
        Assert.Contains("databasePathTextBox", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
