using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public static class BudgetFlatRowBuilder
{
    public static IReadOnlyList<BudgetFlatRow> BuildRows(string outputJson)
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

        List<BudgetFlatRow> rows = [];

        foreach (JsonElement nodeElement in nodesElement.EnumerateArray())
        {
            AddRows(nodeElement, rows);
        }

        return rows;
    }

    private static void AddRows(JsonElement element, List<BudgetFlatRow> rows)
    {
        string nodeType = GetString(element, "nodeType");

        if (string.Equals(nodeType, "BudgetMonth", StringComparison.OrdinalIgnoreCase))
        {
            rows.Add(
                new BudgetFlatRow
                {
                    NodeId = GetString(element, "nodeId"),
                    DisplayText = GetString(element, "displayText"),
                    BudgetYear = GetNullableInt(element, "budgetYear"),
                    BudgetMonth = GetNullableInt(element, "budgetMonth"),
                    MonthKey = GetString(element, "monthKey")
                });
        }

        if (element.TryGetProperty("children", out JsonElement childrenElement) &&
            childrenElement.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement childElement in childrenElement.EnumerateArray())
            {
                AddRows(childElement, rows);
            }
        }
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

        return property.TryGetInt32(out int value)
            ? value
            : null;
    }
}
