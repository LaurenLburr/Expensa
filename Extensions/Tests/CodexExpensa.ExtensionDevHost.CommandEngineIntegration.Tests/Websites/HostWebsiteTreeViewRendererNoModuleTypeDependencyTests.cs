using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeViewRendererNoModuleTypeDependencyTests
{
    [Fact]
    public void Renderer_DoesNotRequireWebsiteTreeNodeCompileTimeType()
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
        Assert.DoesNotContain("IReadOnlyList<WebsiteTreeNode>", text);
        Assert.DoesNotContain("CreateTreeNode(WebsiteTreeNode", text);
        Assert.Contains("IReadOnlyList<object> nodes", text);
        Assert.Contains("HostWebsiteSourceTreeNode.From", text);
        Assert.Contains("BindingFlags.Instance", text);
    }

    [Fact]
    public void Renderer_StillCreatesDefinitivePayloads()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "HostWebsiteTreeViewRenderer.cs");

        Assert.Contains("HostWebsiteCategoryGroupTreeNodePayload", text);
        Assert.Contains("HostWebsiteTreeNodePayload", text);
        Assert.Contains("WebsiteId = source.NodeId", text);
        Assert.Contains("TagName = source.Category", text);
    }

    private static string ReadFile(
        params string[] parts)
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string path =
            Path.Combine([repositoryRoot, .. parts]);

        Assert.True(
            File.Exists(path),
            $"File was not found: {path}");

        return File.ReadAllText(path);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory =
                directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}
