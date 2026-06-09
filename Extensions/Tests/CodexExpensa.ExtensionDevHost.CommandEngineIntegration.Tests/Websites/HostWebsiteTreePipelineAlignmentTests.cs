using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreePipelineAlignmentTests
{
    [Fact]
    public void Mapper_AndRenderer_CreateTypedPayloadsForWebsiteNodes()
    {
        HostWebsiteTreeNode websiteNode = new()
        {
            NodeId = "banking.demo",
            DisplayText = "Demo Bank",
            Url = "https://example.com/bank",
            Category = "Banking"
        };

        TreeNode treeNode =
            HostWebsiteTreeViewNodeMapper.ToTreeNode(websiteNode);

        HostWebsiteTreeNodePayload payload =
            Assert.IsType<HostWebsiteTreeNodePayload>(treeNode.Tag);

        Assert.Equal(HostWebsiteTreeNodeType.Website, payload.NodeType);
        Assert.Equal("banking.demo", payload.WebsiteId);
        Assert.Equal("https://example.com/bank", payload.Url);
    }

    [Fact]
    public void Mapper_CreatesTypedPayloadsForGroupNodes()
    {
        HostWebsiteTreeNode groupNode = new()
        {
            NodeId = "tag:Banking",
            DisplayText = "Banking",
            Children =
            [
                new HostWebsiteTreeNode
                {
                    NodeId = "banking.demo",
                    DisplayText = "Demo Bank",
                    Url = "https://example.com/bank",
                    Category = "Banking"
                }
            ]
        };

        TreeNode treeNode =
            HostWebsiteTreeViewNodeMapper.ToTreeNode(groupNode);

        HostWebsiteCategoryGroupTreeNodePayload payload =
            Assert.IsType<HostWebsiteCategoryGroupTreeNodePayload>(treeNode.Tag);

        Assert.Equal(HostWebsiteTreeNodeType.CategoryGroup, payload.NodeType);
        Assert.Single(treeNode.Nodes);
    }

    [Fact]
    public void ContributionRenderer_UsesSameMapperTypedPayloadPath()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteTreeContributionRenderer.cs");

        Assert.Contains("HostWebsiteTreeViewNodeMapper.ToTreeNodes(result.Nodes)", text);
        Assert.Contains("HostWebsiteCategoryGroupTreeNodePayload", text);
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
