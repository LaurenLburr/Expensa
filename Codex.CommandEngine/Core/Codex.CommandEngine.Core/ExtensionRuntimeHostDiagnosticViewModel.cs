namespace Codex.CommandEngine.Core;

public sealed class ExtensionRuntimeHostDiagnosticViewModel
{
    public RuntimeBootstrapIssueSeverity Severity { get; init; }

    public string CommandName { get; init; } = string.Empty;

    public required string Message { get; init; }
}
