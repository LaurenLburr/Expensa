namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

public static class AddinTreePayloadReader
{
    public static IAddinTreePayload? ReadPayload(
        TreeNode? node)
    {
        return node?.Tag as IAddinTreePayload;
    }

    public static bool IsAddinNode(
        TreeNode? node,
        string addinName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(addinName);

        IAddinTreePayload? payload =
            ReadPayload(node);

        return payload is not null &&
            string.Equals(payload.AddinName, addinName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool HasNodeType(
        TreeNode? node,
        AddinTreeNodeType nodeType)
    {
        IAddinTreePayload? payload =
            ReadPayload(node);

        return payload is not null &&
            payload.NodeType == nodeType;
    }
}
