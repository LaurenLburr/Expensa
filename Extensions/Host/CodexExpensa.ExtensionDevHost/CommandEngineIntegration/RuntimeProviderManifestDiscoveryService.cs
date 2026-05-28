using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class RuntimeProviderManifestDiscoveryService
{
    private readonly IExtensionManifestLoader _manifestLoader;
    private readonly ManifestRuntimeProviderLoader _providerLoader;

    public RuntimeProviderManifestDiscoveryService()
        : this(
            new ExtensionManifestLoader(),
            new ManifestRuntimeProviderLoader())
    {
    }

    public RuntimeProviderManifestDiscoveryService(
        IExtensionManifestLoader manifestLoader,
        ManifestRuntimeProviderLoader providerLoader)
    {
        ArgumentNullException.ThrowIfNull(manifestLoader);
        ArgumentNullException.ThrowIfNull(providerLoader);

        _manifestLoader = manifestLoader;
        _providerLoader = providerLoader;
    }

    public RuntimeProviderDiscoveryResult DiscoverProvidersFromFolder(
        string folderPath,
        bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        if (!Directory.Exists(folderPath))
        {
            return new RuntimeProviderDiscoveryResult
            {
                Issues =
                [
                    new RuntimeProviderDiscoveryIssue
                    {
                        Source = folderPath,
                        Message = "Folder was not found."
                    }
                ]
            };
        }

        SearchOption option =
            recursive
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

        List<IRuntimeCommandRegistrationProvider> providers = [];
        List<RuntimeProviderDiscoveryIssue> issues = [];

        foreach (string manifestPath in Directory.GetFiles(folderPath, "extension.json", option).OrderBy(static path => path))
        {
            ExtensionManifestLoadResult manifestResult =
                _manifestLoader.Load(manifestPath);

            if (!manifestResult.Success || manifestResult.Manifest is null)
            {
                foreach (string error in manifestResult.Errors)
                {
                    issues.Add(new RuntimeProviderDiscoveryIssue
                    {
                        Source = manifestPath,
                        Message = error
                    });
                }

                continue;
            }

            string manifestFolder =
                Path.GetDirectoryName(manifestPath) ?? folderPath;

            RuntimeProviderDiscoveryResult providerResult =
                _providerLoader.LoadProviders(
                    [manifestResult.Manifest],
                    manifestFolder);

            providers.AddRange(providerResult.Providers);

            foreach (RuntimeProviderDiscoveryIssue issue in providerResult.Issues)
            {
                issues.Add(new RuntimeProviderDiscoveryIssue
                {
                    Source = $"{manifestResult.Manifest.ExtensionId} ({manifestPath})",
                    Message = issue.Message,
                    Exception = issue.Exception
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
