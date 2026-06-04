namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;

public sealed class ExpensaAddinRuntimeDescriptor
{
    public required ExpensaAddinLoaderKind Kind { get; init; }

    public required string AddinName { get; init; }

    public required string AssemblyFileName { get; init; }

    public required string SmokeRunnerTypeName { get; init; }

    public required string RequestTypeName { get; init; }

    public required string DefaultCommandName { get; init; }
}
