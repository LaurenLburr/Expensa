namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public static class TreeViewExpansionStateService
{
    public static IReadOnlySet<string> CaptureExpandedNodeNames(TreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        HashSet<string> expandedNodeNames = new(StringComparer.OrdinalIgnoreCase);

        foreach (TreeNode node in treeView.Nodes)
        {
            CaptureNode(node, expandedNodeNames);
        }

        return expandedNodeNames;
    }

    public static void RestoreExpandedNodeNames(TreeView treeView, IReadOnlySet<string> expandedNodeNames)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(expandedNodeNames);

        foreach (TreeNode node in treeView.Nodes)
        {
            RestoreNode(node, expandedNodeNames);
        }
    }

    private static void CaptureNode(TreeNode node, HashSet<string> expandedNodeNames)
    {
        if (node.IsExpanded && !string.IsNullOrWhiteSpace(node.Name))
        {
            expandedNodeNames.Add(node.Name);
        }

        foreach (TreeNode child in node.Nodes)
        {
            CaptureNode(child, expandedNodeNames);
        }
    }

    private static void RestoreNode(TreeNode node, IReadOnlySet<string> expandedNodeNames)
    {
        if (!string.IsNullOrWhiteSpace(node.Name) && expandedNodeNames.Contains(node.Name))
        {
            node.Expand();
        }
        else
        {
            node.Collapse();
        }

        foreach (TreeNode child in node.Nodes)
        {
            RestoreNode(child, expandedNodeNames);
        }
    }
}
