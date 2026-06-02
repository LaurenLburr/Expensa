namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteTreeNodeTagReader
{
    public static IHostWebsiteTreeNodePayload? ReadPayload(
        TreeNode treeNode)
    {
        ArgumentNullException.ThrowIfNull(treeNode);

        return ReadPayload(treeNode.Tag);
    }

    public static IHostWebsiteTreeNodePayload? ReadPayload(
        object? tag)
    {
        if (tag is IHostWebsiteTreeNodePayload payload)
        {
            return payload;
        }

        return null;
    }

    public static bool TryReadPayload(
        TreeNode treeNode,
        out IHostWebsiteTreeNodePayload? payload)
    {
        ArgumentNullException.ThrowIfNull(treeNode);

        payload =
            ReadPayload(treeNode);

        return payload is not null;
    }

    public static string ReadWebsiteId(
        TreeNode treeNode)
    {
        ArgumentNullException.ThrowIfNull(treeNode);

        return ReadWebsiteId(treeNode.Tag);
    }

    public static string ReadWebsiteId(
        object? tag)
    {
        return ReadPayload(tag)?.WebsiteId ?? string.Empty;
    }

    public static string GetWebsiteId(
        TreeNode treeNode)
    {
        return ReadWebsiteId(treeNode);
    }

    public static string GetWebsiteId(
        object? tag)
    {
        return ReadWebsiteId(tag);
    }

    public static string ReadUrl(
        TreeNode treeNode)
    {
        ArgumentNullException.ThrowIfNull(treeNode);

        return ReadUrl(treeNode.Tag);
    }

    public static string ReadUrl(
        object? tag)
    {
        return ReadPayload(tag)?.Url ?? string.Empty;
    }

    public static string GetUrl(
        TreeNode treeNode)
    {
        return ReadUrl(treeNode);
    }

    public static string GetUrl(
        object? tag)
    {
        return ReadUrl(tag);
    }

    public static HostWebsiteTreeNodeType GetNodeType(
        TreeNode treeNode)
    {
        ArgumentNullException.ThrowIfNull(treeNode);

        return GetNodeType(treeNode.Tag);
    }

    public static HostWebsiteTreeNodeType GetNodeType(
        object? tag)
    {
        return ReadPayload(tag)?.NodeType ?? HostWebsiteTreeNodeType.Unknown;
    }

    public static bool IsWebsiteNode(
        TreeNode treeNode)
    {
        ArgumentNullException.ThrowIfNull(treeNode);

        return GetNodeType(treeNode) == HostWebsiteTreeNodeType.Website;
    }

    public static bool IsWebsiteNode(
        object? tag)
    {
        return GetNodeType(tag) == HostWebsiteTreeNodeType.Website;
    }
}
