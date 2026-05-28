namespace Codex.CommandEngine.Core;

public sealed class RuntimeBootstrapDiagnosticLine
{
    public RuntimeBootstrapIssueSeverity Severity { get; init; }

    public required string Message { get; init; }

    public string CommandName { get; init; } = string.Empty;
}
