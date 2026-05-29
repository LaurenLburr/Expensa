using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeViewRendererTests
{
    [Fact]
    public void Render_PopulatesTreeView()
    {
        using TreeView treeView = new();

        HostWebsiteLoadResult result = new()
        {
            Nodes =
            [
                new HostWebsiteTreeNode
                {
                    NodeId = "banking",
                    DisplayText = "Banking",
                    Category = "Root",
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
                }
            ]
        };

        HostWebsiteTreeViewRenderer.Render(treeView, result);

        Assert.Single(treeView.Nodes);
        Assert.Equal("Banking", treeView.Nodes[0].Text);
        Assert.Single(treeView.Nodes[0].Nodes);
    }

    [Fact]
    public void GetSelectedUrl_WhenSelectedNodeHasWebsiteTag_ReturnsUrl()
    {
        using TreeView treeView = new();

        HostWebsiteTreeNode websiteNode = new()
        {
            NodeId = "banking.demo",
            DisplayText = "Demo Bank",
            Url = "https://example.com/bank"
        };

        TreeNode treeNode =
            HostWebsiteTreeViewNodeMapper.ToTreeNode(websiteNode);

        treeView.Nodes.Add(treeNode);
        treeView.SelectedNode = treeNode;

        Assert.Equal(
            "https://example.com/bank",
            HostWebsiteTreeViewRenderer.GetSelectedUrl(treeView));
    }
}
