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
            Tag = CreatePayload(websiteNode)
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

    private static IHostWebsiteTreeNodePayload CreatePayload(
        HostWebsiteTreeNode websiteNode)
    {
        if (websiteNode.Children.Count > 0)
        {
            return new HostWebsiteCategoryGroupTreeNodePayload
            {
                NodeId = websiteNode.NodeId,
                DisplayText = websiteNode.DisplayText
            };
        }

        return new HostWebsiteTreeNodePayload
        {
            NodeId = websiteNode.NodeId,
            WebsiteId = websiteNode.NodeId,
            DisplayText = websiteNode.DisplayText,
            Url = websiteNode.Url,
            TagName = websiteNode.Category,
            IsActive = websiteNode.IsEnabled
        };
    }
}
