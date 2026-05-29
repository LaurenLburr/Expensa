using System.Reflection;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionModuleAssemblyLoader
{
    public IReadOnlyList<ExtensionModuleAssemblyLoadResult> LoadAssemblies(
        string modulesFolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modulesFolder);

        if (!Directory.Exists(modulesFolder))
        {
            return [];
        }

        List<ExtensionModuleAssemblyLoadResult> results = [];

        foreach (string assemblyPath in Directory.EnumerateFiles(modulesFolder, "*.dll", SearchOption.AllDirectories))
        {
            try
            {
                Assembly assembly =
                    Assembly.LoadFrom(assemblyPath);

                results.Add(new ExtensionModuleAssemblyLoadResult
                {
                    AssemblyPath = assemblyPath,
                    Assembly = assembly,
                    Message = "Loaded."
                });
            }
            catch (Exception exception)
            {
                results.Add(new ExtensionModuleAssemblyLoadResult
                {
                    AssemblyPath = assemblyPath,
                    Message = exception.Message
                });
            }
        }

        return results;
    }
}
