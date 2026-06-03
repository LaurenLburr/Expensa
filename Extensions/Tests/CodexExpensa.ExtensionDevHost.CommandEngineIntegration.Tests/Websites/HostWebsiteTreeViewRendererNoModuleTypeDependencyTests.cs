using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeViewRendererNoModuleTypeDependencyTests
{
    [Fact]
    public void Renderer_DoesNotRequireWebsiteAddinModuleTypes()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "HostWebsiteTreeViewRenderer.cs");

        Assert.DoesNotContain("using WebsitesAddin;", text);
        Assert.Contains("IReadOnlyList<HostWebsiteTreeNode> nodes", text);
        Assert.Contains("HostWebsiteTreeViewNodeMapper.ToTreeNodes(nodes)", text);
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
