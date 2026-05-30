namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteFlatRowBuilder
{
    public static IReadOnlyList<HostWebsiteFlatRow> BuildRows(
        HostWebsiteLoadResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        List<HostWebsiteFlatRow> rows = [];

        foreach (HostWebsiteTreeNode node in result.Nodes)
        {
            AddNode(rows, node, node.DisplayText);
        }

        return rows;
    }

    private static void AddNode(
        List<HostWebsiteFlatRow> rows,
        HostWebsiteTreeNode node,
        string parentCategory)
    {
        if (!string.IsNullOrWhiteSpace(node.Url))
        {
            rows.Add(new HostWebsiteFlatRow
            {
                NodeId = node.NodeId,
                DisplayText = node.DisplayText,
                Url = node.Url,
                Category = string.IsNullOrWhiteSpace(node.Category) || node.Category == "Root"
                    ? parentCategory
                    : node.Category,
                IsEnabled = node.IsEnabled
            });
        }

        foreach (HostWebsiteTreeNode child in node.Children)
        {
            AddNode(rows, child, string.IsNullOrWhiteSpace(node.DisplayText) ? parentCategory : node.DisplayText);
        }
    }
}
