using System.Text.Json;
using CodexExpensa.App.WinForms.UI.Websites;

namespace CodexExpensa.App.WinForms.UI.Addins;

public static class TreeAddinTreeNodeFactory
{
    public static TreeNode CreateRootNode(TreeAddinRuntimeResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result.Definition.Kind switch
        {
            TreeAddinKind.Websites => CreateWebsitesRootNode(result),
            TreeAddinKind.Budgets => CreateBudgetsRootNode(result),
            _ => new TreeNode(result.Definition.AddinName)
        };
    }

    public static TreeNode CreateFailureNode(TreeAddinDefinition definition, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(exception);

        return new TreeNode($"{GetDisplayName(definition)} failed")
        {
            Tag = null,
            ToolTipText = exception.Message
        };
    }

    private static TreeNode CreateWebsitesRootNode(TreeAddinRuntimeResult result)
    {
        WebsiteLoadResult websiteResult =
            WebsiteLoadResultParser.Parse(result.OutputJson);

        using TreeView scratchTree =
            new();

        WebsiteTreeViewRenderer.Render(
            scratchTree,
            websiteResult);

        if (scratchTree.Nodes.Count == 0)
        {
            return new TreeNode("Websites")
            {
                Name = "addin.websites"
            };
        }

        TreeNode renderedRoot =
            scratchTree.Nodes[0];

        TreeNode root =
            CloneNode(renderedRoot);

        root.Text = "Websites";
        root.Name = "addin.websites";

        return root;
    }

    private static TreeNode CreateBudgetsRootNode(TreeAddinRuntimeResult result)
    {
        TreeNode root =
            new("Budgets")
            {
                Name = "addin.budgets",
                Tag = "Budget.Root"
            };

        if (string.IsNullOrWhiteSpace(result.OutputJson))
        {
            return root;
        }

        using JsonDocument document =
            JsonDocument.Parse(result.OutputJson);

        if (!document.RootElement.TryGetProperty("nodes", out JsonElement nodesElement) ||
            nodesElement.ValueKind != JsonValueKind.Array)
        {
            return root;
        }

        foreach (JsonElement yearNodeElement in nodesElement.EnumerateArray())
        {
            TreeNode yearNode =
                new(GetString(yearNodeElement, "displayText"))
                {
                    Name = GetString(yearNodeElement, "nodeId"),
                    Tag = GetString(yearNodeElement, "nodeId")
                };

            if (yearNodeElement.TryGetProperty("children", out JsonElement childrenElement) &&
                childrenElement.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement monthNodeElement in childrenElement.EnumerateArray())
                {
                    int? year =
                        GetNullableInt(monthNodeElement, "budgetYear");

                    int? month =
                        GetNullableInt(monthNodeElement, "budgetMonth");

                    if (year is null || month is null)
                    {
                        continue;
                    }

                    string tag =
                        $"Budget.Month.{year.Value:D4}.{month.Value:D2}";

                    yearNode.Nodes.Add(
                        new TreeNode(GetString(monthNodeElement, "displayText"))
                        {
                            Name = GetString(monthNodeElement, "nodeId"),
                            Tag = tag
                        });
                }
            }

            root.Nodes.Add(yearNode);
        }

        return root;
    }

    private static TreeNode CloneNode(TreeNode source)
    {
        TreeNode clone =
            new(source.Text)
            {
                Name = source.Name,
                Tag = source.Tag,
                ToolTipText = source.ToolTipText,
                ImageKey = source.ImageKey,
                SelectedImageKey = source.SelectedImageKey,
                ForeColor = source.ForeColor,
                BackColor = source.BackColor,
                NodeFont = source.NodeFont
            };

        foreach (TreeNode child in source.Nodes)
        {
            clone.Nodes.Add(
                CloneNode(child));
        }

        return clone;
    }

    private static string GetDisplayName(TreeAddinDefinition definition)
    {
        return definition.Kind switch
        {
            TreeAddinKind.Websites => "Websites",
            TreeAddinKind.Budgets => "Budgets",
            _ => definition.AddinName
        };
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
