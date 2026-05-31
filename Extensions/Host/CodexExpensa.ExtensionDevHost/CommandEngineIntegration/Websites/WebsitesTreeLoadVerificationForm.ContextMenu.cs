namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesTreeLoadVerificationForm
{
    private ContextMenuStrip? _runtimeTreeContextMenuStrip;

    protected override void OnLoad(
        EventArgs e)
    {
        base.OnLoad(e);

        AttachRuntimeTreeContextMenu();
    }

    private void AttachRuntimeTreeContextMenu()
    {
        if (_runtimeTreeContextMenuStrip is not null)
        {
            return;
        }

        _runtimeTreeContextMenuStrip =
            new ContextMenuStrip();

        ToolStripMenuItem sortAscendingMenuItem =
            new("Sort ASC");

        sortAscendingMenuItem.Click += (_, _) =>
            SortTreeNodesFromContextMenu(ascending: true);

        ToolStripMenuItem sortDescendingMenuItem =
            new("Sort DESC");

        sortDescendingMenuItem.Click += (_, _) =>
            SortTreeNodesFromContextMenu(ascending: false);

        _runtimeTreeContextMenuStrip.Items.Add(sortAscendingMenuItem);
        _runtimeTreeContextMenuStrip.Items.Add(sortDescendingMenuItem);

        websitesTreeView.ContextMenuStrip =
            _runtimeTreeContextMenuStrip;

        websitesTreeView.MouseUp -= websitesTreeView_RuntimeMouseUp;
        websitesTreeView.MouseUp += websitesTreeView_RuntimeMouseUp;
    }

    private void websitesTreeView_RuntimeMouseUp(
        object? sender,
        MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
        {
            return;
        }

        TreeNode? clickedNode =
            websitesTreeView.GetNodeAt(e.Location);

        if (clickedNode is not null)
        {
            websitesTreeView.SelectedNode =
                clickedNode;
        }

        _runtimeTreeContextMenuStrip?.Show(
            websitesTreeView,
            e.Location);
    }

    private void SortTreeNodesFromContextMenu(
        bool ascending)
    {
        TreeNodeCollection nodes =
            websitesTreeView.SelectedNode?.Nodes ?? websitesTreeView.Nodes;

        if (nodes.Count < 2)
        {
            return;
        }

        List<TreeNode> sortedNodes =
            nodes
                .Cast<TreeNode>()
                .OrderBy(
                    static node => node.Text,
                    StringComparer.CurrentCultureIgnoreCase)
                .ToList();

        if (!ascending)
        {
            sortedNodes.Reverse();
        }

        websitesTreeView.BeginUpdate();

        try
        {
            nodes.Clear();
            nodes.AddRange(sortedNodes.ToArray());
        }
        finally
        {
            websitesTreeView.EndUpdate();
        }
    }
}
