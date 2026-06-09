namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class TreeAddinAggregateLoadResult
{
    public IReadOnlyList<TreeAddinRuntimeResult> AddinResults { get; init; } = [];

    public int LoadedAddinCount =>
        AddinResults.Count(static result => result.Succeeded);

    public int FailedAddinCount =>
        AddinResults.Count(static result => !result.Succeeded);

    public int RootNodeCount { get; init; }

    public string Message =>
        string.Join(
            " | ",
            AddinResults.Select(static result =>
                $"{result.Definition.AddinName}: {result.Status} - {result.Message}"));
}
