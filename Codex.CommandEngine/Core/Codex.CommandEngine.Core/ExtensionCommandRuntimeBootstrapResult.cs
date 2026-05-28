namespace Codex.CommandEngine.Core;

public sealed class ExtensionCommandRuntimeBootstrapResult
{
    public required IExtensionCommandRuntime Runtime { get; init; }

    public IReadOnlyList<RuntimeCommandDescriptor> RegisteredCommands { get; init; } =
        [];

    public RuntimeBootstrapDiagnostics Diagnostics { get; init; } =
        RuntimeBootstrapDiagnostics.Empty();

    public bool HasErrors => Diagnostics.HasErrors;

    public bool HasWarnings => Diagnostics.HasWarnings;
}
