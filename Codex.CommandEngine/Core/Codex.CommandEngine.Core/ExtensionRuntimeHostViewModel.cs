namespace Codex.CommandEngine.Core;

public sealed class ExtensionRuntimeHostViewModel
{
    public ExtensionRuntimeHostState State { get; init; }

    public string StateText { get; init; } = string.Empty;

    public int RegisteredCommandCount { get; init; }

    public int DiagnosticCount { get; init; }

    public bool HasErrors { get; init; }

    public bool HasWarnings { get; init; }

    public string Summary { get; init; } = string.Empty;

    public IReadOnlyList<ExtensionRuntimeHostCommandViewModel> Commands { get; init; } =
        [];

    public IReadOnlyList<ExtensionRuntimeHostDiagnosticViewModel> Diagnostics { get; init; } =
        [];
}
