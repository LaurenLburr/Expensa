using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeContributionRendererTests
{
    [Fact]
    public void RenderContribution_AddsWebsitesRootWithoutClearingOtherNodes()
    {
        using TreeView treeView = new();

        treeView.Nodes.Add(new TreeNode
        {
            Name = "accounts",
            Text = "Accounts"
        });

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

        TreeNode websitesRootNode =
            HostWebsiteTreeContributionRenderer.RenderContribution(treeView, result);

        Assert.Equal(2, treeView.Nodes.Count);
        Assert.Equal("accounts", treeView.Nodes[0].Name);
        Assert.Equal("websites", websitesRootNode.Name);
        Assert.Single(websitesRootNode.Nodes);
        Assert.Equal("Banking", websitesRootNode.Nodes[0].Text);
    }

    [Fact]
    public void RenderContribution_WhenWebsitesRootExists_ReplacesOnlyWebsitesChildren()
    {
        using TreeView treeView = new();

        TreeNode existingWebsitesNode = new()
        {
            Name = "websites",
            Text = "Old Websites"
        };

        existingWebsitesNode.Nodes.Add(new TreeNode
        {
            Name = "old",
            Text = "Old"
        });

        treeView.Nodes.Add(existingWebsitesNode);

        HostWebsiteLoadResult result = new()
        {
            Nodes =
            [
                new HostWebsiteTreeNode
                {
                    NodeId = "new",
                    DisplayText = "New Website"
                }
            ]
        };

        TreeNode websitesRootNode =
            HostWebsiteTreeContributionRenderer.RenderContribution(treeView, result);

        Assert.Single(treeView.Nodes);
        Assert.Same(existingWebsitesNode, websitesRootNode);
        Assert.Equal("Websites", websitesRootNode.Text);
        Assert.Single(websitesRootNode.Nodes);
        Assert.Equal("New Website", websitesRootNode.Nodes[0].Text);
    }
}
