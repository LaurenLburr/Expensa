using System.ComponentModel;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

public static class AddinTreeRenderer
{
    public static int Render<TPayload>(
        TreeView treeView,
        IReadOnlyList<AddinTreeNode<TPayload>> nodes,
        bool expandAll)
        where TPayload : IAddinTreePayload
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(nodes);

        IReadOnlySet<string> expandedNodeNames =
            CaptureExpandedNodeNames(treeView);

        EnsureNodeContextMenu(treeView);

        treeView.BeginUpdate();

        try
        {
            treeView.Nodes.Clear();

            foreach (AddinTreeNode<TPayload> node in nodes)
            {
                treeView.Nodes.Add(
                    CreateTreeNode(node));
            }

            if (expandAll)
            {
                treeView.ExpandAll();
            }
            else
            {
                RestoreExpandedNodeNames(treeView, expandedNodeNames);
            }

            return treeView.Nodes.Count;
        }
        finally
        {
            treeView.EndUpdate();
        }
    }

    public static TreeNode CreateTreeNode<TPayload>(
        AddinTreeNode<TPayload> sourceNode)
        where TPayload : IAddinTreePayload
    {
        ArgumentNullException.ThrowIfNull(sourceNode);

        TreeNode treeNode =
            new(sourceNode.Payload.DisplayText)
            {
                Name = sourceNode.Payload.NodeId,
                Tag = sourceNode.Payload
            };

        foreach (AddinTreeNode<TPayload> child in sourceNode.Children)
        {
            treeNode.Nodes.Add(
                CreateTreeNode(child));
        }

        return treeNode;
    }

    private static IReadOnlySet<string> CaptureExpandedNodeNames(TreeView treeView)
    {
        HashSet<string> expandedNodeNames =
            new(StringComparer.OrdinalIgnoreCase);

        foreach (TreeNode node in treeView.Nodes)
        {
            CaptureExpandedNodeNames(node, expandedNodeNames);
        }

        return expandedNodeNames;
    }

    private static void CaptureExpandedNodeNames(
        TreeNode node,
        HashSet<string> expandedNodeNames)
    {
        if (node.IsExpanded &&
            !string.IsNullOrWhiteSpace(node.Name))
        {
            expandedNodeNames.Add(node.Name);
        }

        foreach (TreeNode child in node.Nodes)
        {
            CaptureExpandedNodeNames(child, expandedNodeNames);
        }
    }

    private static void RestoreExpandedNodeNames(
        TreeView treeView,
        IReadOnlySet<string> expandedNodeNames)
    {
        foreach (TreeNode node in treeView.Nodes)
        {
            RestoreExpandedNodeNames(node, expandedNodeNames);
        }
    }

    private static void RestoreExpandedNodeNames(
        TreeNode node,
        IReadOnlySet<string> expandedNodeNames)
    {
        if (!string.IsNullOrWhiteSpace(node.Name) &&
            expandedNodeNames.Contains(node.Name))
        {
            node.Expand();
        }

        foreach (TreeNode child in node.Nodes)
        {
            RestoreExpandedNodeNames(child, expandedNodeNames);
        }
    }

    private static void EnsureNodeContextMenu(TreeView treeView)
    {
        if (treeView.ContextMenuStrip?.Tag is AddinTreeRendererContextMenuTag)
        {
            return;
        }

        ContextMenuStrip menu =
            new()
            {
                Tag = new AddinTreeRendererContextMenuTag()
            };

        ToolStripMenuItem expandAllMenuItem =
            new("Expand All");

        ToolStripMenuItem collapseAllMenuItem =
            new("Collapse All");

        expandAllMenuItem.Click += (_, _) =>
        {
            if (treeView.SelectedNode is not null)
            {
                treeView.SelectedNode.ExpandAll();
            }
        };

        collapseAllMenuItem.Click += (_, _) =>
        {
            if (treeView.SelectedNode is not null)
            {
                CollapseAllChildren(treeView.SelectedNode);
            }
        };

        menu.Items.Add(expandAllMenuItem);
        menu.Items.Add(collapseAllMenuItem);

        menu.Opening += (_, e) =>
        {
            TreeNode? node =
                treeView.GetNodeAt(treeView.PointToClient(Cursor.Position));

            if (node is null ||
                node.Nodes.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            treeView.SelectedNode =
                node;

            expandAllMenuItem.Enabled =
                node.Nodes.Count > 0;

            collapseAllMenuItem.Enabled =
                node.Nodes.Count > 0;
        };

        treeView.ContextMenuStrip =
            menu;
    }

    private static void CollapseAllChildren(TreeNode node)
    {
        foreach (TreeNode child in node.Nodes)
        {
            CollapseAllChildren(child);
        }

        node.Collapse();
    }

    private sealed class AddinTreeRendererContextMenuTag
    {
    }
}
