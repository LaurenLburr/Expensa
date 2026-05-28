namespace Codex.CommandEngine.Core;

public sealed class RuntimeBootstrapIssue
{
    public RuntimeBootstrapIssueSeverity Severity { get; init; } =
        RuntimeBootstrapIssueSeverity.Information;

    public required string Message { get; init; }

    public string CommandName { get; init; } = string.Empty;

    public Exception? Exception { get; init; }
}
