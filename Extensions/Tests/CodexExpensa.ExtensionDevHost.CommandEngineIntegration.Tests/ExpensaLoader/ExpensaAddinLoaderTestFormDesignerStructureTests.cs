using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExpensaLoader;

public sealed class ExpensaAddinLoaderTestFormDesignerStructureTests
{
    [Fact]
    public void Form_UsesDesignerInitializeComponentPattern()
    {
        string codeBehind =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "ExpensaLoader",
                "ExpensaAddinLoaderTestForm.cs");

        string designer =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "ExpensaLoader",
                "ExpensaAddinLoaderTestForm.Designer.cs");

        Assert.Contains("public sealed partial class ExpensaAddinLoaderTestForm", codeBehind);
        Assert.Contains("InitializeComponent();", codeBehind);
        Assert.DoesNotContain("BuildLayout", codeBehind);
        Assert.DoesNotContain("new TreeView()", codeBehind);
        Assert.Contains("partial class ExpensaAddinLoaderTestForm", designer);
        Assert.Contains("private void InitializeComponent()", designer);
        Assert.Contains("treeView.AfterSelect += treeView_AfterSelect;", designer);
        Assert.Contains("loadAllButton.Click += loadAllButton_Click;", designer);
    }

    private static string ReadFile(params string[] parts)
    {
        string repositoryRoot = FindRepositoryRoot();
        string path = Path.Combine([repositoryRoot, .. parts]);

        Assert.True(File.Exists(path), $"File was not found: {path}");

        return File.ReadAllText(path);
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

        throw new DirectoryNotFoundException();
    }
}
