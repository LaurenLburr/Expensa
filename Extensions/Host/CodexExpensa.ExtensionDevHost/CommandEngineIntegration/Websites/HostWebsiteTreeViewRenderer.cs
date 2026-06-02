using System.Reflection;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteTreeViewRenderer
{
    public static int Render(
        TreeView treeView,
        HostWebsiteLoadResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return Render(
            treeView,
            result,
            expandAll: false);
    }

    public static int Render(
        TreeView treeView,
        HostWebsiteLoadResult result,
        bool expandAll)
    {
        ArgumentNullException.ThrowIfNull(result);

        return Render(
            treeView,
            result.Nodes.Cast<object>().ToList(),
            expandAll);
    }

    public static int Render(
        TreeView treeView,
        IReadOnlyList<object> nodes)
    {
        return Render(
            treeView,
            nodes,
            expandAll: false);
    }

    public static int Render(
        TreeView treeView,
        IReadOnlyList<object> nodes,
        bool expandAll)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(nodes);

        treeView.BeginUpdate();

        try
        {
            treeView.Nodes.Clear();

            foreach (object sourceNode in nodes)
            {
                treeView.Nodes.Add(
                    CreateTreeNode(sourceNode));
            }

            if (expandAll)
            {
                treeView.ExpandAll();
            }

            return treeView.Nodes.Count;
        }
        finally
        {
            treeView.EndUpdate();
        }
    }

    public static IHostWebsiteTreeNodePayload? GetSelectedWebsiteNode(
        TreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        TreeNode? selectedNode =
            treeView.SelectedNode;

        if (selectedNode is null)
        {
            return null;
        }

        return HostWebsiteTreeNodeTagReader.ReadPayload(selectedNode);
    }

    public static string GetSelectedUrl(
        TreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        return GetSelectedWebsiteNode(treeView)?.Url ?? string.Empty;
    }

    private static TreeNode CreateTreeNode(object sourceNode)
    {
        ArgumentNullException.ThrowIfNull(sourceNode);

        HostWebsiteSourceTreeNode source =
            HostWebsiteSourceTreeNode.From(sourceNode);

        TreeNode treeNode =
            new(source.DisplayText)
            {
                Name = source.NodeId
            };

        if (source.Children.Count > 0)
        {
            treeNode.Tag =
                new HostWebsiteCategoryGroupTreeNodePayload
                {
                    NodeId = source.NodeId,
                    DisplayText = source.DisplayText
                };

            foreach (object childNode in source.Children)
            {
                treeNode.Nodes.Add(
                    CreateTreeNode(childNode));
            }

            return treeNode;
        }

        treeNode.Tag =
            new HostWebsiteTreeNodePayload
            {
                NodeId = source.NodeId,
                WebsiteId = source.NodeId,
                DisplayText = source.DisplayText,
                Url = source.Url,
                TagName = source.Category,
                IsActive = source.IsEnabled
            };

        return treeNode;
    }

    private sealed class HostWebsiteSourceTreeNode
    {
        public required string NodeId { get; init; }

        public required string DisplayText { get; init; }

        public string Url { get; init; } = string.Empty;

        public string Category { get; init; } = string.Empty;

        public bool IsEnabled { get; init; } = true;

        public IReadOnlyList<object> Children { get; init; } = [];

        public static HostWebsiteSourceTreeNode From(
            object source)
        {
            Type sourceType =
                source.GetType();

            return new HostWebsiteSourceTreeNode
            {
                NodeId = GetStringProperty(source, sourceType, "NodeId"),
                DisplayText = GetStringProperty(source, sourceType, "DisplayText"),
                Url = GetStringProperty(source, sourceType, "Url"),
                Category = GetStringProperty(source, sourceType, "Category"),
                IsEnabled = GetBooleanProperty(source, sourceType, "IsEnabled", defaultValue: true),
                Children = GetChildren(source, sourceType)
            };
        }

        private static string GetStringProperty(
            object source,
            Type sourceType,
            string propertyName)
        {
            PropertyInfo? property =
                sourceType.GetProperty(
                    propertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (property is null)
            {
                return string.Empty;
            }

            object? value =
                property.GetValue(source);

            return Convert.ToString(value) ?? string.Empty;
        }

        private static bool GetBooleanProperty(
            object source,
            Type sourceType,
            string propertyName,
            bool defaultValue)
        {
            PropertyInfo? property =
                sourceType.GetProperty(
                    propertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (property is null)
            {
                return defaultValue;
            }

            object? value =
                property.GetValue(source);

            if (value is bool boolValue)
            {
                return boolValue;
            }

            return bool.TryParse(
                Convert.ToString(value),
                out bool parsedValue)
                    ? parsedValue
                    : defaultValue;
        }

        private static IReadOnlyList<object> GetChildren(
            object source,
            Type sourceType)
        {
            PropertyInfo? property =
                sourceType.GetProperty(
                    "Children",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (property is null)
            {
                return [];
            }

            object? value =
                property.GetValue(source);

            if (value is not System.Collections.IEnumerable enumerable)
            {
                return [];
            }

            List<object> children =
                [];

            foreach (object? child in enumerable)
            {
                if (child is not null)
                {
                    children.Add(child);
                }
            }

            return children;
        }
    }
}
