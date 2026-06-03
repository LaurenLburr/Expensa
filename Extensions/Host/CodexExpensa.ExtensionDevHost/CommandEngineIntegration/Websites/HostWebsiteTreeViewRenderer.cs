namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteTreeViewRenderer
{
    public static int Render(TreeView treeView, HostWebsiteLoadResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return Render(treeView, result, expandAll: false);
    }

    public static int Render(TreeView treeView, HostWebsiteLoadResult result, bool expandAll)
    {
        ArgumentNullException.ThrowIfNull(result);

        return Render(treeView, result.Nodes, expandAll);
    }

    public static int Render(TreeView treeView, IReadOnlyList<HostWebsiteTreeNode> nodes)
    {
        return Render(treeView, nodes, expandAll: false);
    }

    public static int Render(TreeView treeView, IReadOnlyList<HostWebsiteTreeNode> nodes, bool expandAll)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(nodes);

        treeView.BeginUpdate();

        try
        {
            treeView.Nodes.Clear();

            foreach (TreeNode node in HostWebsiteTreeViewNodeMapper.ToTreeNodes(nodes))
            {
                treeView.Nodes.Add(node);
            }

            if (expandAll)
            {
                treeView.ExpandAll();
            }

            return treeView.Nodes.Count;
        }
        finally
        {
            treeView.EndUpdate();
        }
    }

    public static HostWebsiteTreeNode? GetSelectedWebsiteNode(TreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        IHostWebsiteTreeNodePayload? payload =
            HostWebsiteTreeNodeTagReader.ReadPayload(treeView.SelectedNode);

        if (payload is null ||
            payload.NodeType != HostWebsiteTreeNodeType.Website)
        {
            return null;
        }

        return new HostWebsiteTreeNode
        {
            NodeId = string.IsNullOrWhiteSpace(payload.WebsiteId)
                ? payload.NodeId
                : payload.WebsiteId,
            DisplayText = payload.DisplayText,
            Url = payload.Url,
            Category = payload.TagName,
            IsEnabled = payload.IsActive,
            Children = []
        };
    }

    public static string GetSelectedUrl(TreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        IHostWebsiteTreeNodePayload? payload =
            HostWebsiteTreeNodeTagReader.ReadPayload(treeView.SelectedNode);

        return payload?.Url ?? string.Empty;
    }
}
