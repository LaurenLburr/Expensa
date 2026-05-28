namespace Codex.CommandEngine.App;

/*
Example wiring for MainForm AfterSelect handler:

private readonly RuntimeOperationsSelectionState _runtimeSelectionState = new();

private readonly RuntimeOperationsHostViewBuilder _runtimeViewBuilder =
    MainFormRuntimeOperations.CreateViewBuilder();

private void NavigationTree_AfterSelect(object? sender, TreeViewEventArgs e)
{
    string nodeKey =
        e.Node?.Tag?.ToString() ?? string.Empty;

    _runtimeSelectionState.SetSelectedNode(nodeKey);

    if (MainFormRuntimeNodeRouting.TryHandleRuntimeOperationsNode(
            nodeKey,
            _outputTextBox,
            _runtimeViewBuilder))
    {
        return;
    }

    // Existing routing continues here...
}

*/
