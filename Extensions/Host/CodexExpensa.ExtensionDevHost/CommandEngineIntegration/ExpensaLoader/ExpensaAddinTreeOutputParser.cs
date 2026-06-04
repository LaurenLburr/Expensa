using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;
using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;

public static class ExpensaAddinTreeOutputParser
{
    public static AddinTreeNode<ExpensaAddinTreePayload> ParseResultAsRoot(
        ExpensaAddinLoaderKind kind,
        ExpensaAddinLoaderResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        ExpensaAddinRuntimeDescriptor descriptor =
            ExpensaAddinRuntimeDescriptorFactory.Create(kind);

        IReadOnlyList<AddinTreeNode<ExpensaAddinTreePayload>> children =
            ParseChildren(kind, descriptor.AddinName, result.OutputJson);

        return new AddinTreeNode<ExpensaAddinTreePayload>
        {
            Payload = new ExpensaAddinTreePayload(
                descriptor.AddinName,
                AddinTreeNodeType.Root,
                descriptor.AddinName,
                GetRootDisplayText(kind))
            {
                LoaderKind = kind
            },
            Children = children
        };
    }

    private static IReadOnlyList<AddinTreeNode<ExpensaAddinTreePayload>> ParseChildren(
        ExpensaAddinLoaderKind kind,
        string addinName,
        string outputJson)
    {
        if (string.IsNullOrWhiteSpace(outputJson))
        {
            return [];
        }

        using JsonDocument document =
            JsonDocument.Parse(outputJson);

        if (!document.RootElement.TryGetProperty("nodes", out JsonElement nodesElement) ||
            nodesElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        List<AddinTreeNode<ExpensaAddinTreePayload>> nodes = [];

        foreach (JsonElement nodeElement in nodesElement.EnumerateArray())
        {
            nodes.Add(ReadNode(kind, addinName, nodeElement));
        }

        return nodes;
    }

    private static AddinTreeNode<ExpensaAddinTreePayload> ReadNode(
        ExpensaAddinLoaderKind kind,
        string addinName,
        JsonElement element)
    {
        List<AddinTreeNode<ExpensaAddinTreePayload>> children = [];

        if (element.TryGetProperty("children", out JsonElement childrenElement) &&
            childrenElement.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement childElement in childrenElement.EnumerateArray())
            {
                children.Add(ReadNode(kind, addinName, childElement));
            }
        }

        AddinTreeNodeType nodeType =
            ResolveNodeType(kind, element, children.Count > 0);

        string nodeId = GetString(element, "nodeId");
        string displayText = GetString(element, "displayText");

        ExpensaAddinTreePayload payload =
            new(addinName, nodeType, nodeId, displayText)
            {
                LoaderKind = kind,
                WebsiteId = kind == ExpensaAddinLoaderKind.Websites && nodeType == AddinTreeNodeType.Website ? nodeId : string.Empty,
                Url = GetString(element, "url"),
                TagName = GetString(element, "category"),
                IsActive = GetBoolean(element, "isEnabled", defaultValue: true),
                BudgetYear = GetNullableInt(element, "budgetYear"),
                BudgetMonth = GetNullableInt(element, "budgetMonth"),
                MonthKey = GetString(element, "monthKey")
            };

        return new AddinTreeNode<ExpensaAddinTreePayload>
        {
            Payload = payload,
            Children = children
        };
    }

    private static AddinTreeNodeType ResolveNodeType(
        ExpensaAddinLoaderKind kind,
        JsonElement element,
        bool hasChildren)
    {
        if (kind == ExpensaAddinLoaderKind.Websites)
        {
            return hasChildren ? AddinTreeNodeType.Group : AddinTreeNodeType.Website;
        }

        string nodeType = GetString(element, "nodeType");

        return nodeType switch
        {
            "BudgetYear" => AddinTreeNodeType.BudgetYear,
            "BudgetMonth" => AddinTreeNodeType.BudgetMonth,
            "BudgetRoot" => AddinTreeNodeType.Root,
            _ => hasChildren ? AddinTreeNodeType.Group : AddinTreeNodeType.Item
        };
    }

    private static string GetRootDisplayText(ExpensaAddinLoaderKind kind)
    {
        return kind switch
        {
            ExpensaAddinLoaderKind.Websites => "Websites",
            ExpensaAddinLoaderKind.Budgets => "Budgets",
            _ => kind.ToString()
        };
    }

    private static string GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out JsonElement property) &&
            property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? string.Empty
                : string.Empty;
    }

    private static bool GetBoolean(JsonElement element, string propertyName, bool defaultValue)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property))
        {
            return defaultValue;
        }

        return property.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => defaultValue
        };
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
}
