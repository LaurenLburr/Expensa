using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class MainAddinProjectDatabaseNodeWiringTests
{
    [Fact]
    public void MainForm_WiresWebsitesDatabaseNodeToWebsitesDatabasePanel()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string mainFormPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "MainForm.cs");

        Assert.True(File.Exists(mainFormPath), $"File was not found: {mainFormPath}");

        string text =
            File.ReadAllText(mainFormPath);

        Assert.Contains("using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;", text);
        Assert.Contains("ShowEmbeddedForm(new WebsitesDatabasePanelForm())", text);
        Assert.Contains("IsWebsitesAddinProject", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
