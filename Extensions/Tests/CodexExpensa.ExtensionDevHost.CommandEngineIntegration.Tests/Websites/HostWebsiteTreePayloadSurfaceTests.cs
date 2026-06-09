using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreePayloadSurfaceTests
{
    [Fact]
    public void PayloadInterface_ExposesWebsiteDetailSurfaceExpectedByExistingCode()
    {
        Assert.NotNull(typeof(IHostWebsiteTreeNodePayload).GetProperty("WebsiteId"));
        Assert.NotNull(typeof(IHostWebsiteTreeNodePayload).GetProperty("TagName"));
        Assert.NotNull(typeof(IHostWebsiteTreeNodePayload).GetProperty("Url"));
        Assert.NotNull(typeof(IHostWebsiteTreeNodePayload).GetProperty("IsActive"));
    }

    [Fact]
    public void Reader_ProvidesReadPayloadCompatibilityMethod()
    {
        TreeNode node = new("Website")
        {
            Tag = new HostWebsiteTreeNodePayload
            {
                NodeId = "website-1",
                WebsiteId = "website-1",
                DisplayText = "Website",
                Url = "https://example.test",
                TagName = "Example",
                IsActive = true
            }
        };

        IHostWebsiteTreeNodePayload? payload =
            HostWebsiteTreeNodeTagReader.ReadPayload(node);

        Assert.NotNull(payload);
        Assert.Equal(HostWebsiteTreeNodeType.Website, payload.NodeType);
        Assert.Equal("website-1", payload.WebsiteId);
        Assert.Equal("Example", payload.TagName);
    }

    [Fact]
    public void Renderer_ProvidesExistingCompatibilityMethods()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteTreeViewRenderer.cs");

        Assert.Contains("Render(TreeView treeView, HostWebsiteLoadResult result)", text);
        Assert.Contains("GetSelectedWebsiteNode", text);
        Assert.Contains("GetSelectedUrl", text);
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
