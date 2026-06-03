namespace CodexExpensa.App.WinForms.UI.Websites;

public static class WebsiteTreeNodeTagReader
{
    public static bool TryReadPayload(
        TreeNode treeNode,
        out IHostWebsiteTreeNodePayload? payload)
    {
        ArgumentNullException.ThrowIfNull(treeNode);

        payload = treeNode.Tag as IHostWebsiteTreeNodePayload;

        return payload is not null;
    }

    public static bool IsWebsiteNode(
        TreeNode treeNode)
    {
        ArgumentNullException.ThrowIfNull(treeNode);

        return treeNode.Tag is IHostWebsiteTreeNodePayload payload &&
            payload.NodeType == HostWebsiteTreeNodeType.Website;
    }

    public static string GetSelectedUrl(
        TreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        return treeView.SelectedNode?.Tag is IHostWebsiteTreeNodePayload payload
            ? payload.Url
            : string.Empty;
    }
}
