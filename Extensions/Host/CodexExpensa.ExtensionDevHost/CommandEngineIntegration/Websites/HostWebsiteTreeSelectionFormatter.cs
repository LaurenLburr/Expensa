namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteTreeSelectionFormatter
{
    public static string FormatSelectedNode(
        TreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        IHostWebsiteTreeNodePayload? node =
            HostWebsiteTreeViewRenderer.GetSelectedWebsiteNode(treeView);

        if (node is null)
        {
            return string.Empty;
        }

        return
            $"NodeId: {node.NodeId}{Environment.NewLine}" +
            $"WebsiteId: {node.WebsiteId}{Environment.NewLine}" +
            $"DisplayText: {node.DisplayText}{Environment.NewLine}" +
            $"Category: {node.TagName}{Environment.NewLine}" +
            $"NodeType: {node.NodeType}{Environment.NewLine}" +
            $"Url: {node.Url}{Environment.NewLine}" +
            $"Enabled: {node.IsActive}";
    }
}
