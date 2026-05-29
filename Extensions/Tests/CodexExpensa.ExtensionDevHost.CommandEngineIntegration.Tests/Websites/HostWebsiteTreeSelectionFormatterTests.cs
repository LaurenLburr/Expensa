using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeSelectionFormatterTests
{
    [Fact]
    public void FormatSelectedNode_WhenWebsiteNodeSelected_ReturnsDetails()
    {
        using TreeView treeView = new();

        HostWebsiteTreeNode websiteNode = new()
        {
            NodeId = "banking.demo",
            DisplayText = "Demo Bank",
            Url = "https://example.com/bank",
            Category = "Banking"
        };

        TreeNode treeNode =
            HostWebsiteTreeViewNodeMapper.ToTreeNode(websiteNode);

        treeView.Nodes.Add(treeNode);
        treeView.SelectedNode = treeNode;

        string details =
            HostWebsiteTreeSelectionFormatter.FormatSelectedNode(treeView);

        Assert.Contains("banking.demo", details);
        Assert.Contains("Demo Bank", details);
        Assert.Contains("https://example.com/bank", details);
    }

    [Fact]
    public void FormatSelectedNode_WhenNoSelection_ReturnsEmpty()
    {
        using TreeView treeView = new();

        string details =
            HostWebsiteTreeSelectionFormatter.FormatSelectedNode(treeView);

        Assert.Equal(string.Empty, details);
    }
}
