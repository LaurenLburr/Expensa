namespace Codex.CommandEngine.App;

public sealed class RuntimeOperationsSelectionState
{
    public string SelectedNodeKey { get; private set; } =
        HostNodeKeys.RuntimeOperations;

    public DateTimeOffset LastRefreshUtc { get; private set; } =
        DateTimeOffset.MinValue;

    public void SetSelectedNode(string nodeKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nodeKey);

        SelectedNodeKey = nodeKey;
        LastRefreshUtc = DateTimeOffset.UtcNow;
    }
}
