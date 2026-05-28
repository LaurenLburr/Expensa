using System.Windows.Forms;

namespace Codex.CommandEngine.App;

public static class NavigationTreeBuilder
{
    public static void Build(TreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        treeView.BeginUpdate();

        try
        {
            treeView.Nodes.Clear();

            TreeNode databaseNode = new("Database");
            databaseNode.Nodes.Add(CreateNode("DB Schema", HostNodeKeys.DatabaseSchema));
            databaseNode.Nodes.Add(CreateNode("Full DB Schema", HostNodeKeys.DatabaseFullSchema));
            databaseNode.Nodes.Add(CreateNode("SQL Catalog", HostNodeKeys.SqlCatalog));

            TreeNode definitionsNode = new("Definitions");
            definitionsNode.Nodes.Add(CreateNode("Commands", HostNodeKeys.CommandDefinitions));
            definitionsNode.Nodes.Add(CreateNode("Workflows", HostNodeKeys.WorkflowDefinitions));
            definitionsNode.Nodes.Add(CreateNode("Contexts", HostNodeKeys.ContextDefinitions));
            definitionsNode.Nodes.Add(CreateNode("AI Providers", HostNodeKeys.AiProviderDefinitions));

            TreeNode executionNode = new("Execution");
            executionNode.Nodes.Add(CreateNode("Execution History", HostNodeKeys.ExecutionHistory));

            TreeNode runtimeNode = new("Runtime Operations");
            runtimeNode.Nodes.Add(CreateNode("Resumable Workflows", HostNodeKeys.RuntimeResumableWorkflows));
            runtimeNode.Nodes.Add(CreateNode("Stale Workflows", HostNodeKeys.RuntimeStaleWorkflows));
            runtimeNode.Nodes.Add(CreateNode("Incomplete Workflows", HostNodeKeys.RuntimeIncompleteWorkflows));

            treeView.Nodes.Add(databaseNode);
            treeView.Nodes.Add(definitionsNode);
            treeView.Nodes.Add(executionNode);
            treeView.Nodes.Add(runtimeNode);

            treeView.ExpandAll();
        }
        finally
        {
            treeView.EndUpdate();
        }
    }

    private static TreeNode CreateNode(string text, string key)
    {
        return new TreeNode(text)
        {
            Name = key,
            Tag = key
        };
    }
}
