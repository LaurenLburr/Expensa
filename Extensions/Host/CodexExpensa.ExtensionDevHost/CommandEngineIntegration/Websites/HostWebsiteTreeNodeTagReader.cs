using System.Reflection;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteTreeNodeTagReader
{
    public static IHostWebsiteTreeNodePayload? ReadPayload(TreeNode? node)
    {
        return node?.Tag as IHostWebsiteTreeNodePayload;
    }

    public static string ReadWebsiteId(TreeNode node)
    {
        return GetWebsiteId(node);
    }

    public static HostWebsiteTreeNodeType GetNodeType(TreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        IHostWebsiteTreeNodePayload? payload = ReadPayload(node);

        if (payload is not null)
        {
            return payload.NodeType;
        }

        if (node.Nodes.Count > 0)
        {
            return HostWebsiteTreeNodeType.CategoryGroup;
        }

        string websiteId = GetWebsiteId(node);

        return string.IsNullOrWhiteSpace(websiteId)
            ? HostWebsiteTreeNodeType.Unknown
            : HostWebsiteTreeNodeType.Website;
    }

    public static bool IsWebsiteNode(TreeNode node)
    {
        return GetNodeType(node) == HostWebsiteTreeNodeType.Website;
    }

    public static bool IsCategoryGroupNode(TreeNode node)
    {
        return GetNodeType(node) == HostWebsiteTreeNodeType.CategoryGroup;
    }

    public static string GetWebsiteId(TreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        IHostWebsiteTreeNodePayload? payload = ReadPayload(node);

        if (payload is not null)
        {
            return payload.WebsiteId;
        }

        if (node.Nodes.Count > 0)
        {
            return string.Empty;
        }

        string websiteId = GetWebsiteIdFromTagObject(node.Tag);

        if (!string.IsNullOrWhiteSpace(websiteId))
        {
            return websiteId;
        }

        websiteId = GetWebsiteIdFromString(node.Tag?.ToString() ?? string.Empty);

        if (!string.IsNullOrWhiteSpace(websiteId))
        {
            return websiteId;
        }

        if (node.Parent is not null &&
            !string.IsNullOrWhiteSpace(node.Name) &&
            !IsGroupNodeId(node.Name))
        {
            return node.Name;
        }

        return string.Empty;
    }

    public static HostWebsiteTreeNodePayload? GetWebsitePayload(TreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        return node.Tag as HostWebsiteTreeNodePayload;
    }

    private static string GetWebsiteIdFromTagObject(object? tag)
    {
        if (tag is null)
        {
            return string.Empty;
        }

        Type tagType = tag.GetType();

        string websiteId = GetStringPropertyValue(tag, tagType, "WebsiteId");

        if (!string.IsNullOrWhiteSpace(websiteId))
        {
            return websiteId;
        }

        string nodeId = GetStringPropertyValue(tag, tagType, "NodeId");
        string url = GetStringPropertyValue(tag, tagType, "Url");

        if (!string.IsNullOrWhiteSpace(nodeId) &&
            !IsGroupNodeId(nodeId) &&
            !string.IsNullOrWhiteSpace(url))
        {
            return nodeId;
        }

        return string.Empty;
    }

    private static string GetStringPropertyValue(object source, Type sourceType, string propertyName)
    {
        PropertyInfo? property =
            sourceType.GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        if (property is null)
        {
            return string.Empty;
        }

        object? value = property.GetValue(source);

        return Convert.ToString(value) ?? string.Empty;
    }

    private static string GetWebsiteIdFromString(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        if (TryExtractJsonStringValue(text, "WebsiteId", out string websiteId))
        {
            return websiteId;
        }

        if (TryExtractJsonStringValue(text, "websiteId", out websiteId))
        {
            return websiteId;
        }

        if (TryExtractJsonStringValue(text, "NodeId", out websiteId) &&
            !IsGroupNodeId(websiteId))
        {
            return websiteId;
        }

        if (TryExtractJsonStringValue(text, "nodeId", out websiteId) &&
            !IsGroupNodeId(websiteId))
        {
            return websiteId;
        }

        return string.Empty;
    }

    private static bool TryExtractJsonStringValue(string text, string propertyName, out string value)
    {
        value = string.Empty;

        string marker = $"\"{propertyName}\"";
        int markerIndex = text.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        if (markerIndex < 0)
        {
            return false;
        }

        int colonIndex = text.IndexOf(':', markerIndex);

        if (colonIndex < 0)
        {
            return false;
        }

        int firstQuoteIndex = text.IndexOf('"', colonIndex + 1);

        if (firstQuoteIndex < 0)
        {
            return false;
        }

        int secondQuoteIndex = text.IndexOf('"', firstQuoteIndex + 1);

        if (secondQuoteIndex < 0)
        {
            return false;
        }

        value = text.Substring(firstQuoteIndex + 1, secondQuoteIndex - firstQuoteIndex - 1);

        return !string.IsNullOrWhiteSpace(value);
    }

    private static bool IsGroupNodeId(string value)
    {
        return value.StartsWith("tag:", StringComparison.OrdinalIgnoreCase) ||
               value.StartsWith("category:", StringComparison.OrdinalIgnoreCase) ||
               value.StartsWith("group:", StringComparison.OrdinalIgnoreCase);
    }
}
