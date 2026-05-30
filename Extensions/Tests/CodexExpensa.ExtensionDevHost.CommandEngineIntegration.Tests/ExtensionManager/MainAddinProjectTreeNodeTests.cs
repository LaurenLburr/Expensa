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
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "MainForm.cs");

        Assert.True(File.Exists(mainFormPath), $"File was not found: {mainFormPath}");

        string text = File.ReadAllText(mainFormPath);

        Assert.Contains("new TreeNode(\"Database\")", text);
        Assert.Contains("new TreeNode(\"Test\")", text);
        Assert.Contains("AddinProjectDatabaseNavigationTag", text);
        Assert.Contains("AddinProjectTestNavigationTag", text);
        Assert.Contains("ExtensionTreeLoadTestForm", text);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root containing Extensions folder.");
    }
}
