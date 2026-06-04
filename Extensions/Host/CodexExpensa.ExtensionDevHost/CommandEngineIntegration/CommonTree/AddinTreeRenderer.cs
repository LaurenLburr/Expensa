namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

public static class AddinTreeRenderer
{
    public static int Render<TPayload>(
        TreeView treeView,
        IReadOnlyList<AddinTreeNode<TPayload>> nodes,
        bool expandAll)
        where TPayload : IAddinTreePayload
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(nodes);

        treeView.BeginUpdate();

        try
        {
            treeView.Nodes.Clear();

            foreach (AddinTreeNode<TPayload> node in nodes)
            {
                treeView.Nodes.Add(
                    CreateTreeNode(node));
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

    public static TreeNode CreateTreeNode<TPayload>(
        AddinTreeNode<TPayload> sourceNode)
        where TPayload : IAddinTreePayload
    {
        ArgumentNullException.ThrowIfNull(sourceNode);

        TreeNode treeNode =
            new(sourceNode.Payload.DisplayText)
            {
                Name = sourceNode.Payload.NodeId,
                Tag = sourceNode.Payload
            };

        foreach (AddinTreeNode<TPayload> child in sourceNode.Children)
        {
            treeNode.Nodes.Add(
                CreateTreeNode(child));
        }

        return treeNode;
    }
}
