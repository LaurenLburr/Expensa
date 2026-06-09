using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeViewRendererNamespaceTests
{
    [Fact]
    public void Renderer_DoesNotImportWebsitesAddin()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteTreeViewRenderer.cs");

        Assert.DoesNotContain("using WebsitesAddin;", text);
        Assert.Contains("HostWebsiteTreeNode", text);
        Assert.Contains("HostWebsiteTreeViewNodeMapper.ToTreeNodes", text);
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
        return TestPathHelper.ExtensionsRoot;
    }
}
