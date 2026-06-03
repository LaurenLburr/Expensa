namespace CodexExpensa.App.WinForms.UI.Websites;

public static class WebsiteTreeViewRenderer
{
    public static int Render(
        TreeView treeView,
        WebsiteLoadResult result)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(result);

        treeView.BeginUpdate();

        try
        {
            treeView.Nodes.Clear();

            foreach (WebsiteTreeNode sourceNode in result.Nodes)
            {
                treeView.Nodes.Add(CreateTreeNode(sourceNode));
            }

            return treeView.Nodes.Count;
        }
        finally
        {
            treeView.EndUpdate();
        }
    }

    private static TreeNode CreateTreeNode(
        WebsiteTreeNode sourceNode)
    {
        ArgumentNullException.ThrowIfNull(sourceNode);

        TreeNode treeNode =
            new(sourceNode.DisplayText)
            {
                Name = sourceNode.NodeId,
                Tag = CreatePayload(sourceNode)
            };

        foreach (WebsiteTreeNode childNode in sourceNode.Children)
        {
            treeNode.Nodes.Add(CreateTreeNode(childNode));
        }

        return treeNode;
    }

    private static IHostWebsiteTreeNodePayload CreatePayload(
        WebsiteTreeNode sourceNode)
    {
        if (sourceNode.Children.Count > 0)
        {
            return new HostWebsiteCategoryGroupTreeNodePayload
            {
                NodeId = sourceNode.NodeId,
                DisplayText = sourceNode.DisplayText
            };
        }

        return new HostWebsiteTreeNodePayload
        {
            NodeId = sourceNode.NodeId,
            WebsiteId = sourceNode.NodeId,
            DisplayText = sourceNode.DisplayText,
            Url = sourceNode.Url,
            TagName = sourceNode.Category,
            IsActive = sourceNode.IsEnabled
        };
    }
}
