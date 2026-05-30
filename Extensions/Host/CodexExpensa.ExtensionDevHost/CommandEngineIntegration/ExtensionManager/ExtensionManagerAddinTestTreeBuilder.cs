namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public static class ExtensionManagerAddinTestTreeBuilder
{
    public static void Populate(
        TreeView treeView,
        IReadOnlyList<ExtensionManagerAddinTestNode> addins)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(addins);

        treeView.BeginUpdate();

        try
        {
            treeView.Nodes.Clear();

            TreeNode rootNode = new()
            {
                Name = "addins",
                Text = "Add-ins"
            };

            foreach (ExtensionManagerAddinTestNode addin in addins.OrderBy(static item => item.SortOrder))
            {
                TreeNode addinNode = new()
                {
                    Name = addin.AddinId,
                    Text = $"{addin.DisplayName} ({addin.SortOrder})",
                    Tag = addin
                };

                TreeNode databaseNode = new()
                {
                    Name = $"{addin.AddinId}.database",
                    Text = addin.DatabaseDisplayName,
                    Tag = new ExtensionManagerAddinTestAction
                    {
                        Addin = addin,
                        ActionKind = ExtensionManagerAddinTestActionKind.Database
                    }
                };

                TreeNode testNode = new()
                {
                    Name = $"{addin.AddinId}.test",
                    Text = $"Test - {addin.TestFormName}",
                    Tag = new ExtensionManagerAddinTestAction
                    {
                        Addin = addin,
                        ActionKind = ExtensionManagerAddinTestActionKind.Test
                    }
                };

                addinNode.Nodes.Add(databaseNode);
                addinNode.Nodes.Add(testNode);
                rootNode.Nodes.Add(addinNode);
            }

            treeView.Nodes.Add(rootNode);
            rootNode.ExpandAll();
        }
        finally
        {
            treeView.EndUpdate();
        }
    }
}
