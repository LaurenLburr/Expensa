namespace Codex.CommandEngine.Core;

public sealed class RuntimeBootstrapDiagnosticViewModel
{
    public int RegisteredCommandCount { get; init; }

    public int IssueCount { get; init; }

    public bool HasErrors { get; init; }

    public bool HasWarnings { get; init; }

    public IReadOnlyList<RuntimeBootstrapDiagnosticLine> Lines { get; init; } =
        [];
}
