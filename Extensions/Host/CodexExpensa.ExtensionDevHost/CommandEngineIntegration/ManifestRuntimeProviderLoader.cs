using Codex.CommandEngine.Core;
using System.Reflection;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ManifestRuntimeProviderLoader
{
    public RuntimeProviderDiscoveryResult LoadProviders(
        IReadOnlyList<ExtensionManifest> manifests,
        string baseFolder)
    {
        List<IRuntimeCommandRegistrationProvider> providers = [];
        List<RuntimeProviderDiscoveryIssue> issues = [];

        foreach (ExtensionManifest manifest in manifests)
        {
            if (!manifest.Enabled)
            {
                continue;
            }

            string assemblyPath =
                Path.Combine(baseFolder, manifest.AssemblyFile);

            if (!File.Exists(assemblyPath))
            {
                issues.Add(new RuntimeProviderDiscoveryIssue
                {
                    Source = manifest.ExtensionId,
                    Message = $"Assembly file not found: {assemblyPath}"
                });

                continue;
            }

            try
            {
                Assembly assembly =
                    Assembly.LoadFrom(assemblyPath);

                Type? providerType =
                    assembly.GetType(manifest.ProviderType, false);

                if (providerType is null)
                {
                    issues.Add(new RuntimeProviderDiscoveryIssue
                    {
                        Source = manifest.ExtensionId,
                        Message = $"Provider type not found: {manifest.ProviderType}"
                    });

                    continue;
                }

                if (Activator.CreateInstance(providerType) is not IRuntimeCommandRegistrationProvider provider)
                {
                    issues.Add(new RuntimeProviderDiscoveryIssue
                    {
                        Source = manifest.ExtensionId,
                        Message = $"Provider type invalid: {manifest.ProviderType}"
                    });

                    continue;
                }

                providers.Add(provider);
            }
            catch (Exception exception)
            {
                issues.Add(new RuntimeProviderDiscoveryIssue
                {
                    Source = manifest.ExtensionId,
                    Message = exception.Message,
                    Exception = exception
                });
            }
        }

        return new RuntimeProviderDiscoveryResult
        {
            Providers = providers,
            Issues = issues
        };
    }
}
