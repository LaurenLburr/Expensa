using System.Text.Json;
using CodexExpensa.App.WinForms.UI.Websites;

namespace CodexExpensa.App.WinForms.UI.Addins;

public static class TreeAddinTreeNodeFactory
{
    public static TreeNode CreateRootNode(TreeAddinRuntimeResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        TreeNode root =
            result.Definition.Kind switch
            {
                TreeAddinKind.Websites => CreateWebsitesRootNode(result),
                TreeAddinKind.Budgets => CreateBudgetsRootNode(result),
                TreeAddinKind.Payees => CreatePayeesRootNode(result),
                _ => new TreeNode(result.Definition.AddinName)
            };

        root.ToolTipText =
            CreateAssemblyToolTip(result);

        return root;
    }

    public static TreeNode CreateFailureNode(
        TreeAddinDefinition definition,
        Exception exception)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(exception);

        return new TreeNode($"{GetDisplayName(definition)} failed")
        {
            Tag = null,
            ToolTipText = exception.ToString()
        };
    }

    private static TreeNode CreateWebsitesRootNode(
        TreeAddinRuntimeResult result)
    {
        WebsiteLoadResult websiteResult =
            WebsiteLoadResultParser.Parse(result.OutputJson);

        using TreeView scratchTree = new();

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

    private static TreeNode CreateBudgetsRootNode(
        TreeAddinRuntimeResult result)
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

        if (!document.RootElement.TryGetProperty(
                "nodes",
                out JsonElement nodesElement)
            ||
            nodesElement.ValueKind != JsonValueKind.Array)
        {
            return root;
        }

        foreach (JsonElement nodeElement in
                 nodesElement.EnumerateArray())
        {
            root.Nodes.Add(
                CreateBudgetNode(nodeElement));
        }

        return root;
    }

    private static TreeNode CreateBudgetNode(
        JsonElement element)
    {
        string nodeType =
            GetString(element, "nodeType");

        int? year =
            GetNullableInt(element, "budgetYear");

        int? month =
            GetNullableInt(element, "budgetMonth");

        TreeNode node =
            new(GetString(element, "displayText"))
            {
                Name = GetString(element, "nodeId"),
                Tag = new BudgetTreeNodePayload(
                    GetString(element, "nodeId"),
                    nodeType,
                    GetString(element, "displayText"),
                    year,
                    month)
            };

        if (element.TryGetProperty(
                "children",
                out JsonElement childrenElement)
            &&
            childrenElement.ValueKind ==
            JsonValueKind.Array)
        {
            foreach (JsonElement childElement in
                     childrenElement.EnumerateArray())
            {
                node.Nodes.Add(
                    CreateBudgetNode(childElement));
            }
        }

        return node;
    }

    private static TreeNode CreatePayeesRootNode(
        TreeAddinRuntimeResult result)
    {
        TreeNode fallback =
            new("Payees")
            {
                Name = "addin.payees",
                Tag = new PayeeTreeNodePayload(
                    "PayeeRoot",
                    string.Empty,
                    "Payees")
            };

        if (string.IsNullOrWhiteSpace(result.OutputJson))
        {
            return fallback;
        }

        using JsonDocument document =
            JsonDocument.Parse(result.OutputJson);

        if (!document.RootElement.TryGetProperty(
                "nodes",
                out JsonElement nodesElement)
            ||
            nodesElement.ValueKind != JsonValueKind.Array)
        {
            return fallback;
        }

        JsonElement.ArrayEnumerator enumerator =
            nodesElement.EnumerateArray();

        if (!enumerator.MoveNext())
        {
            return fallback;
        }

        TreeNode root =
            CreatePayeeNode(enumerator.Current);

        root.Text = "Payees";
        root.Name = "addin.payees";

        return root;
    }

    private static TreeNode CreatePayeeNode(
        JsonElement element)
    {
        string nodeType =
            GetString(element, "nodeType");

        string displayText =
            GetString(element, "displayText");

        string payeeId =
            GetString(element, "payeeId");

        TreeNode node =
            new(displayText)
            {
                Name = GetString(element, "nodeId"),
                Tag = new PayeeTreeNodePayload(
                    nodeType,
                    payeeId,
                    displayText)
            };

        if (element.TryGetProperty(
                "children",
                out JsonElement childrenElement)
            &&
            childrenElement.ValueKind ==
            JsonValueKind.Array)
        {
            foreach (JsonElement childElement in
                     childrenElement.EnumerateArray())
            {
                node.Nodes.Add(
                    CreatePayeeNode(childElement));
            }
        }

        return node;
    }

    private static string CreateAssemblyToolTip(
        TreeAddinRuntimeResult result)
    {
        if (string.IsNullOrWhiteSpace(
                result.AssemblyPath))
        {
            return result.Message;
        }

        string timestamp =
            result.AssemblyLastWriteTimeUtc is null
                ? "(unknown)"
                : result.AssemblyLastWriteTimeUtc.Value
                    .ToLocalTime()
                    .ToString("yyyy-MM-dd HH:mm:ss");

        return
            $"{result.Definition.AddinName}"
            + Environment.NewLine
            + $"Loaded from: {result.AssemblyPath}"
            + Environment.NewLine
            + $"DLL modified: {timestamp}"
            + Environment.NewLine
            + result.Message;
    }

    private static TreeNode CloneNode(
        TreeNode source)
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

    private static string GetDisplayName(
        TreeAddinDefinition definition)
    {
        return definition.Kind switch
        {
            TreeAddinKind.Websites => "Websites",
            TreeAddinKind.Budgets => "Budgets",
            TreeAddinKind.Payees => "Payees",
            _ => definition.AddinName
        };
    }

    private static string GetString(
        JsonElement element,
        string propertyName)
    {
        return element.TryGetProperty(
                   propertyName,
                   out JsonElement property)
               &&
               property.ValueKind ==
               JsonValueKind.String
            ? property.GetString()
              ?? string.Empty
            : string.Empty;
    }

    private static int? GetNullableInt(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(
                propertyName,
                out JsonElement property)
            ||
            property.ValueKind is
                JsonValueKind.Null
                or JsonValueKind.Undefined)
        {
            return null;
        }

        return property.TryGetInt32(
            out int value)
            ? value
            : null;
    }
}
