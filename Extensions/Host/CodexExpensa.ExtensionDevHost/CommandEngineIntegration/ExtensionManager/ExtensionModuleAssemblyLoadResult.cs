using System.Reflection;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionModuleAssemblyLoadResult
{
    public required string AssemblyPath { get; init; }

    public Assembly? Assembly { get; init; }

    public bool Succeeded =>
        Assembly is not null;

    public string Message { get; init; } = string.Empty;
}
