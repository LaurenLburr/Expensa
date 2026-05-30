using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class MainNavigationTreePersistentExpansionStateTests
{
    [Fact]
    public void MainForm_PersistsNavigationExpandedNodeNamesInSettings()
    {
        string text = ReadMainForm();

        Assert.Contains("NavigationExpandedNodeNames", text);
        Assert.Contains("LoadSavedNavigationExpandedNodeNames", text);
        Assert.Contains("SaveNavigationExpandedNodeNames", text);
        Assert.Contains("AfterExpand += NavigationTreeView_ExpansionChanged", text);
        Assert.Contains("AfterCollapse += NavigationTreeView_ExpansionChanged", text);
    }

    [Fact]
    public void MainForm_AssignsStableTreeNodeNames()
    {
        string text = ReadMainForm();

        Assert.Contains("Name = \"project\"", text);
        Assert.Contains("Name = \"addinProjects\"", text);
        Assert.Contains("Name = $\"addin.{CreateSafeTreeNodeName(registration.ProjectName)}\"", text);
        Assert.Contains("Name = $\"addin.{CreateSafeTreeNodeName(registration.ProjectName)}.database\"", text);
        Assert.Contains("Name = $\"addin.{CreateSafeTreeNodeName(registration.ProjectName)}.test\"", text);
    }

    private static string ReadMainForm()
    {
        string repositoryRoot = FindRepositoryRoot();

        string mainFormPath = Path.Combine(
            repositoryRoot,
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "MainForm.cs");

        Assert.True(File.Exists(mainFormPath), $"File was not found: {mainFormPath}");
        return File.ReadAllText(mainFormPath);
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
