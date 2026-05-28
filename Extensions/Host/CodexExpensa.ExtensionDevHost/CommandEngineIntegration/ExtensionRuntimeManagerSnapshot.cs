using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionRuntimeManagerSnapshot
{
    public ExtensionRuntimeManagerStatus Status { get; init; }

    public ExtensionRuntimeHostViewModel Host { get; init; } =
        new()
        {
            State = ExtensionRuntimeHostState.NotStarted,
            StateText = ExtensionRuntimeHostState.NotStarted.ToString(),
            Summary = "Runtime has not been started."
        };

    public string DiagnosticText { get; init; } = string.Empty;
}
