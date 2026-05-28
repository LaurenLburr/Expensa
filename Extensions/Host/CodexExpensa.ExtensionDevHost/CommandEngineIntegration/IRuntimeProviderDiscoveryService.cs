using Codex.CommandEngine.Core;
using System.Reflection;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface IRuntimeProviderDiscoveryService
{
    RuntimeProviderDiscoveryResult DiscoverFromLoadedAssemblies();

    RuntimeProviderDiscoveryResult DiscoverFromAssembly(
        Assembly assembly);

    RuntimeProviderDiscoveryResult DiscoverFromAssemblyFile(
        string assemblyPath);

    RuntimeProviderDiscoveryResult DiscoverFromFolder(
        string folderPath,
        bool recursive = false);
}
