namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteTreeViewRenderer
{
    public static void Render(
        TreeView treeView,
        HostWebsiteLoadResult result)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(result);

        treeView.BeginUpdate();

        try
        {
            treeView.Nodes.Clear();

            foreach (TreeNode node in HostWebsiteTreeViewNodeMapper.ToTreeNodes(result.Nodes))
            {
                treeView.Nodes.Add(node);
            }
        }
        finally
        {
            treeView.EndUpdate();
        }
    }

    public static HostWebsiteTreeNode? GetSelectedWebsiteNode(
        TreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        return treeView.SelectedNode?.Tag as HostWebsiteTreeNode;
    }

    public static string GetSelectedUrl(
        TreeView treeView)
    {
        return GetSelectedWebsiteNode(treeView)?.Url ?? string.Empty;
    }
}
