namespace Codex.CommandEngine.Core;

public sealed class RuntimeBootstrapDiagnostics
{
    public IReadOnlyList<RuntimeBootstrapIssue> Issues { get; init; } =
        [];

    public bool HasErrors =>
        Issues.Any(static issue => issue.Severity == RuntimeBootstrapIssueSeverity.Error);

    public bool HasWarnings =>
        Issues.Any(static issue => issue.Severity == RuntimeBootstrapIssueSeverity.Warning);

    public static RuntimeBootstrapDiagnostics Empty()
    {
        return new RuntimeBootstrapDiagnostics
        {
            Issues = []
        };
    }
}
