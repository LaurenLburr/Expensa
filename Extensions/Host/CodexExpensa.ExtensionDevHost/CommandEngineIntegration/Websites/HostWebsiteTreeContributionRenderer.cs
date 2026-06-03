namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteTreeContributionRenderer
{
    public const string WebsitesRootNodeName = "websites";

    public static TreeNode RenderContribution(TreeView treeView, HostWebsiteLoadResult result)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(result);

        TreeNode websitesRootNode =
            FindOrCreateWebsitesRootNode(treeView);

        websitesRootNode.Nodes.Clear();

        foreach (TreeNode childNode in HostWebsiteTreeViewNodeMapper.ToTreeNodes(result.Nodes))
        {
            websitesRootNode.Nodes.Add(childNode);
        }

        return websitesRootNode;
    }

    private static TreeNode FindOrCreateWebsitesRootNode(TreeView treeView)
    {
        TreeNode[] existingNodes =
            treeView.Nodes.Find(WebsitesRootNodeName, searchAllChildren: false);

        if (existingNodes.Length > 0)
        {
            existingNodes[0].Text = "Websites";
            return existingNodes[0];
        }

        TreeNode websitesRootNode = new()
        {
            Name = WebsitesRootNodeName,
            Text = "Websites",
            Tag = new HostWebsiteCategoryGroupTreeNodePayload
            {
                NodeId = WebsitesRootNodeName,
                DisplayText = "Websites"
            }
        };

        treeView.Nodes.Add(websitesRootNode);

        return websitesRootNode;
    }
}
