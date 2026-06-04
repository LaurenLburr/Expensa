using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public static class HostBudgetTreeViewRenderer
{
    public static int Render(TreeView treeView, string outputJson, bool expandAll)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        IReadOnlyList<HostBudgetTreeNode> nodes = ParseNodes(outputJson);

        treeView.BeginUpdate();

        try
        {
            treeView.Nodes.Clear();

            TreeNode rootNode = new("Budgets")
            {
                Name = "budgets",
                Tag = new HostBudgetTreeNodePayload
                {
                    NodeType = HostBudgetTreeNodeType.BudgetRoot,
                    NodeId = "budgets",
                    DisplayText = "Budgets"
                }
            };

            foreach (HostBudgetTreeNode node in nodes)
            {
                rootNode.Nodes.Add(CreateNode(node));
            }

            treeView.Nodes.Add(rootNode);

            if (expandAll) treeView.ExpandAll();
            else rootNode.Expand();

            return treeView.Nodes.Count;
        }
        finally
        {
            treeView.EndUpdate();
        }
    }

    private static IReadOnlyList<HostBudgetTreeNode> ParseNodes(string outputJson)
    {
        if (string.IsNullOrWhiteSpace(outputJson))
        {
            return [];
        }

        using JsonDocument document = JsonDocument.Parse(outputJson);

        if (!document.RootElement.TryGetProperty("nodes", out JsonElement nodesElement) ||
            nodesElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        List<HostBudgetTreeNode> nodes = [];

        foreach (JsonElement nodeElement in nodesElement.EnumerateArray())
        {
            nodes.Add(ReadNode(nodeElement));
        }

        return nodes;
    }

    private static HostBudgetTreeNode ReadNode(JsonElement element)
    {
        List<HostBudgetTreeNode> children = [];

        if (element.TryGetProperty("children", out JsonElement childrenElement) &&
            childrenElement.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement childElement in childrenElement.EnumerateArray())
            {
                children.Add(ReadNode(childElement));
            }
        }

        return new HostBudgetTreeNode
        {
            NodeId = GetString(element, "nodeId"),
            DisplayText = GetString(element, "displayText"),
            NodeType = GetString(element, "nodeType"),
            BudgetYear = GetNullableInt(element, "budgetYear"),
            BudgetMonth = GetNullableInt(element, "budgetMonth"),
            MonthKey = GetString(element, "monthKey"),
            Children = children
        };
    }

    private static TreeNode CreateNode(HostBudgetTreeNode source)
    {
        TreeNode node = new(source.DisplayText)
        {
            Name = source.NodeId,
            Tag = new HostBudgetTreeNodePayload
            {
                NodeType = ParseNodeType(source.NodeType),
                NodeId = source.NodeId,
                DisplayText = source.DisplayText,
                BudgetYear = source.BudgetYear,
                BudgetMonth = source.BudgetMonth,
                MonthKey = source.MonthKey
            }
        };

        foreach (HostBudgetTreeNode child in source.Children)
        {
            node.Nodes.Add(CreateNode(child));
        }

        return node;
    }

    private static HostBudgetTreeNodeType ParseNodeType(string nodeType)
    {
        return Enum.TryParse(nodeType, ignoreCase: true, out HostBudgetTreeNodeType parsed)
            ? parsed
            : HostBudgetTreeNodeType.Unknown;
    }

    private static string GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out JsonElement property) &&
            property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? string.Empty
                : string.Empty;
    }

    private static int? GetNullableInt(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property) ||
            property.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return null;
        }

        return property.TryGetInt32(out int value) ? value : null;
    }

    private sealed class HostBudgetTreeNode
    {
        public required string NodeId { get; init; }
        public required string DisplayText { get; init; }
        public required string NodeType { get; init; }
        public int? BudgetYear { get; init; }
        public int? BudgetMonth { get; init; }
        public string MonthKey { get; init; } = string.Empty;
        public IReadOnlyList<HostBudgetTreeNode> Children { get; init; } = [];
    }
}
