namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteTreeViewNodeMapper
{
    public static TreeNode ToTreeNode(
        HostWebsiteTreeNode websiteNode)
    {
        ArgumentNullException.ThrowIfNull(websiteNode);

        TreeNode treeNode = new()
        {
            Name = websiteNode.NodeId,
            Text = websiteNode.DisplayText,
            Tag = websiteNode
        };

        foreach (HostWebsiteTreeNode child in websiteNode.Children)
        {
            treeNode.Nodes.Add(ToTreeNode(child));
        }

        return treeNode;
    }

    public static IReadOnlyList<TreeNode> ToTreeNodes(
        IReadOnlyList<HostWebsiteTreeNode> websiteNodes)
    {
        ArgumentNullException.ThrowIfNull(websiteNodes);

        return websiteNodes
            .Select(ToTreeNode)
            .ToList();
    }
}
