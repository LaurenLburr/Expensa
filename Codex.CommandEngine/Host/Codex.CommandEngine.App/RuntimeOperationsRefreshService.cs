namespace Codex.CommandEngine.App;

public sealed class RuntimeOperationsRefreshService
{
    private readonly RuntimeOperationsHostViewBuilder _viewBuilder;
    private readonly RuntimeOperationsSelectionState _selectionState;

    public RuntimeOperationsRefreshService(
        RuntimeOperationsHostViewBuilder viewBuilder,
        RuntimeOperationsSelectionState selectionState)
    {
        ArgumentNullException.ThrowIfNull(viewBuilder);
        ArgumentNullException.ThrowIfNull(selectionState);

        _viewBuilder = viewBuilder;
        _selectionState = selectionState;
    }

    public string Refresh()
    {
        return _viewBuilder.BuildTextForNode(
            _selectionState.SelectedNodeKey);
    }
}
