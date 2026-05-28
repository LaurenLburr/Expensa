namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifestDiscoveryService : IExtensionManifestDiscoveryService
{
    private readonly IExtensionManifestLoader _loader;

    public ExtensionManifestDiscoveryService()
        : this(new ExtensionManifestLoader())
    {
    }

    public ExtensionManifestDiscoveryService(
        IExtensionManifestLoader loader)
    {
        _loader = loader;
    }

    public ExtensionManifestDiscoveryResult DiscoverFromFolder(
        string folderPath,
        bool recursive = false)
    {
        if (!Directory.Exists(folderPath))
        {
            return new ExtensionManifestDiscoveryResult
            {
                Errors = [$"Folder was not found: {folderPath}"]
            };
        }

        SearchOption option =
            recursive
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

        List<ExtensionManifest> manifests = [];
        List<string> errors = [];

        foreach (string path in Directory.GetFiles(folderPath, "extension.json", option))
        {
            ExtensionManifestLoadResult result =
                _loader.Load(path);

            if (!result.Success || result.Manifest is null)
            {
                foreach (string error in result.Errors)
                {
                    errors.Add($"{path}: {error}");
                }

                continue;
            }

            manifests.Add(result.Manifest);
        }

        return new ExtensionManifestDiscoveryResult
        {
            Manifests = manifests,
            Errors = errors
        };
    }
}
