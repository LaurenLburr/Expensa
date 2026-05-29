namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public static class ExtensionManagerAddinTestTreeBuilder
{
    public const string LoadTestFormActionKey = "load-test-form";

    public static void Populate(TreeView treeView, IReadOnlyList<ExtensionManagerAddinTestNode> addins)
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

                TreeNode loadTestFormNode = new()
                {
                    Name = $"{addin.AddinId}.{LoadTestFormActionKey}",
                    Text = $"Load Test Form - {addin.TestFormName}",
                    Tag = new ExtensionManagerAddinTestAction
                    {
                        Addin = addin,
                        ActionKey = LoadTestFormActionKey
                    }
                };

                addinNode.Nodes.Add(loadTestFormNode);
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
