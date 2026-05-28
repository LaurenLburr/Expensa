namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface IExtensionManifestDiscoveryService
{
    ExtensionManifestDiscoveryResult DiscoverFromFolder(
        string folderPath,
        bool recursive = false);
}
