namespace Codex.CommandEngine.App;

// This file intentionally does NOT declare MainForm as partial.
// The existing MainForm class is not partial, so runtime-operation helpers live here
// as a standalone static helper instead of a partial class extension.
public static class MainFormRuntimeOperations
{
    public static RuntimeOperationsHostViewBuilder CreateViewBuilder()
    {
        return new RuntimeOperationsHostViewBuilder(
            () => RuntimeOperationsViewServiceFactory.CreateEmpty());
    }

    public static string BuildRuntimeOperationsText(string nodeKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nodeKey);

        RuntimeOperationsHostViewBuilder builder =
            CreateViewBuilder();

        return builder.BuildTextForNode(nodeKey);
    }

    public static bool IsRuntimeOperationsNode(string nodeKey)
    {
        return nodeKey is HostNodeKeys.RuntimeOperations
            or HostNodeKeys.RuntimeResumableWorkflows
            or HostNodeKeys.RuntimeStaleWorkflows
            or HostNodeKeys.RuntimeIncompleteWorkflows;
    }
}
