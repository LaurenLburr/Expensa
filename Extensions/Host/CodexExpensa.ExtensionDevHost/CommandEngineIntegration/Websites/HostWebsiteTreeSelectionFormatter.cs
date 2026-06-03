namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteTreeSelectionFormatter
{
    public static string FormatSelectedNode(TreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        TreeNode? selectedNode = treeView.SelectedNode;

        if (selectedNode is null)
        {
            return string.Empty;
        }

        IHostWebsiteTreeNodePayload? payload =
            HostWebsiteTreeNodeTagReader.ReadPayload(selectedNode);

        if (payload is null)
        {
            return string.Empty;
        }

        return
            $"NodeType: {payload.NodeType}{Environment.NewLine}" +
            $"NodeId: {payload.NodeId}{Environment.NewLine}" +
            $"WebsiteId: {payload.WebsiteId}{Environment.NewLine}" +
            $"DisplayText: {payload.DisplayText}{Environment.NewLine}" +
            $"TagName: {payload.TagName}{Environment.NewLine}" +
            $"Url: {payload.Url}{Environment.NewLine}" +
            $"Enabled: {payload.IsActive}{Environment.NewLine}" +
            $"Children: {selectedNode.Nodes.Count}";
    }
}
