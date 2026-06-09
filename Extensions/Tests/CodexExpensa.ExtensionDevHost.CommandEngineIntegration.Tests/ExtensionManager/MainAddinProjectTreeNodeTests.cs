using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class MainAddinProjectTreeNodeTests
{
    [Fact]
    public void MainForm_AddsDatabaseAndTestNodesToRegisteredProjects()
    {
        string repositoryRoot = FindRepositoryRoot();

        string mainFormPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "MainForm.cs");

        Assert.True(File.Exists(mainFormPath), $"File was not found: {mainFormPath}");

        string text = File.ReadAllText(mainFormPath);

        Assert.Contains("new TreeNode(\"Database\")", text);
        Assert.Contains("new TreeNode(\"Test\")", text);
        Assert.Contains("AddinProjectDatabaseNavigationTag", text);
        Assert.Contains("AddinProjectTestNavigationTag", text);
        Assert.Contains("WebsitesTreeLoadVerificationForm", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
