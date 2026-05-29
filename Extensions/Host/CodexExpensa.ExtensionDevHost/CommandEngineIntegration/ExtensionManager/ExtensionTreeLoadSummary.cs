namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionTreeLoadSummary
{
    public IReadOnlyList<ExtensionTreeLoadResult> Results { get; init; } = [];

    public int SucceededCount =>
        Results.Count(static result => result.Succeeded);

    public int FailedCount =>
        Results.Count(static result => !result.Succeeded);

    public string ToDisplayText()
    {
        return $"Tree load: {SucceededCount} succeeded, {FailedCount} failed.";
    }
}
