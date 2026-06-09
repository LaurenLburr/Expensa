using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeViewRendererCompatibilityTests
{
    [Fact]
    public void Renderer_KeepsLoadResultOverloadWithoutExpandAllArgument()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteTreeViewRenderer.cs");

        Assert.Contains("Render", text);
        Assert.Contains("TreeView treeView", text);
        Assert.Contains("HostWebsiteLoadResult result", text);
        Assert.Contains("expandAll: false", text);
    }

    [Fact]
    public void Renderer_KeepsSelectionHelpersUsedByExistingForms()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteTreeViewRenderer.cs");

        Assert.Contains("GetSelectedWebsiteNode", text);
        Assert.Contains("GetSelectedUrl", text);
        Assert.Contains("HostWebsiteTreeNodeTagReader.ReadPayload", text);
    }

    [Fact]
    public void Renderer_UsesHostWebsiteTreeNodeBoundary()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteTreeViewRenderer.cs");

        Assert.DoesNotContain("using WebsitesAddin;", text);
        Assert.Contains("HostWebsiteTreeNode", text);
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
