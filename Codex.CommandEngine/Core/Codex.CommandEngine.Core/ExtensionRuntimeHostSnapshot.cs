namespace Codex.CommandEngine.Core;

public sealed class ExtensionRuntimeHostSnapshot
{
    public ExtensionRuntimeHostState State { get; init; }

    public int RegisteredCommandCount { get; init; }

    public RuntimeBootstrapDiagnostics Diagnostics { get; init; } =
        RuntimeBootstrapDiagnostics.Empty();

    public IReadOnlyList<RuntimeCommandDescriptor> Commands { get; init; } =
        [];
}
