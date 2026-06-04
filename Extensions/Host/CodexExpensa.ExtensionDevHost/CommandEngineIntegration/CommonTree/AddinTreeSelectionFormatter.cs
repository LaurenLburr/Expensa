namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

public static class AddinTreeSelectionFormatter
{
    public static string FormatSelectedNode(
        TreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        IAddinTreePayload? payload =
            AddinTreePayloadReader.ReadPayload(treeView.SelectedNode);

        if (payload is null)
        {
            return string.Empty;
        }

        return
            $"Add-in: {payload.AddinName}{Environment.NewLine}" +
            $"NodeType: {payload.NodeType}{Environment.NewLine}" +
            $"NodeId: {payload.NodeId}{Environment.NewLine}" +
            $"DisplayText: {payload.DisplayText}{Environment.NewLine}" +
            $"PayloadJson:{Environment.NewLine}{payload.PayloadJson}";
    }
}
