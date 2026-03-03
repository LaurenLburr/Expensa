using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.Navigation;

internal sealed class NavigationController
{
    private readonly TreeView _tree;
    private readonly ScreenHost _screenHost;

    private readonly Dictionary<string, NavigationNode> _nodeById = new(StringComparer.OrdinalIgnoreCase);

    public NavigationController(TreeView tree, ScreenHost screenHost)
    {
        _tree = tree ?? throw new ArgumentNullException(nameof(tree));
        _screenHost = screenHost ?? throw new ArgumentNullException(nameof(screenHost));
    }

    public void BuildTree(IReadOnlyList<NavigationNode> roots)
    {
        if (roots is null) throw new ArgumentNullException(nameof(roots));

        _tree.BeginUpdate();
        try
        {
            _tree.Nodes.Clear();
            _nodeById.Clear();

            foreach (var root in roots)
            {
                _tree.Nodes.Add(CreateTreeNode(root));
            }

            _tree.ExpandAll();
        }
        finally
        {
            _tree.EndUpdate();
        }

        _tree.AfterSelect -= TreeAfterSelect;
        _tree.AfterSelect += TreeAfterSelect;
    }

    private TreeNode CreateTreeNode(NavigationNode nav)
    {
        if (nav is null) throw new ArgumentNullException(nameof(nav));

        _nodeById[nav.Id] = nav;

        var node = new TreeNode(nav.Text)
        {
            Name = nav.Id,
            Tag = nav.Id
        };

        foreach (var child in nav.Children)
        {
            node.Nodes.Add(CreateTreeNode(child));
        }

        return node;
    }

    private void TreeAfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is not string id) return;
        if (!_nodeById.TryGetValue(id, out var nav)) return;

        // Header/category nodes do nothing
        if (nav.CreateScreen is null) return;

        _screenHost.Show(nav.Id, nav.CreateScreen, nav.SingleInstance);
    }
}