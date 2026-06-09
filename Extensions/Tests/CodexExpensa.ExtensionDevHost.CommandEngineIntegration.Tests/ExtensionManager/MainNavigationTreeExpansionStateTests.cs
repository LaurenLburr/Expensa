using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class MainNavigationTreeExpansionStateTests
{
    [Fact]
    public void MainForm_CapturesAndRestoresNavigationExpansionState()
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

        Assert.Contains("CaptureExpandedNavigationNodeNames", text);
        Assert.Contains("RestoreExpandedNavigationNodeNames", text);
        Assert.Contains("NavigationTreeView_ExpansionChanged", text);
        Assert.Contains("AfterExpand += NavigationTreeView_ExpansionChanged", text);
        Assert.Contains("AfterCollapse += NavigationTreeView_ExpansionChanged", text);
        Assert.Contains("CreateSafeTreeNodeName", text);
    }

    [Fact]
    public void MainForm_AssignsStableNamesToMainNavigationNodes()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string mainFormPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "MainForm.cs");

        string text =
            File.ReadAllText(mainFormPath);

        Assert.Contains("Name = \"project\"", text);
        Assert.Contains("Name = \"addinProjects\"", text);
        Assert.Contains("Name = \"templates\"", text);
        Assert.Contains("Name = \"tools\"", text);
        Assert.Contains("Name = \"docs\"", text);
        Assert.Contains("Name = $\"addin.{CreateSafeTreeNodeName(registration.ProjectName)}\"", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
