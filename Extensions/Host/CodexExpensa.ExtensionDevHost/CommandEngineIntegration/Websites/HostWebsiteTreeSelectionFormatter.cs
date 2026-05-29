namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteTreeSelectionFormatter
{
    public static string FormatSelectedNode(
        TreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        HostWebsiteTreeNode? node =
            HostWebsiteTreeViewRenderer.GetSelectedWebsiteNode(treeView);

        if (node is null)
        {
            return string.Empty;
        }

        return
            $"NodeId: {node.NodeId}{Environment.NewLine}" +
            $"DisplayText: {node.DisplayText}{Environment.NewLine}" +
            $"Category: {node.Category}{Environment.NewLine}" +
            $"Url: {node.Url}{Environment.NewLine}" +
            $"Enabled: {node.IsEnabled}{Environment.NewLine}" +
            $"Children: {node.Children.Count}";
    }
}
