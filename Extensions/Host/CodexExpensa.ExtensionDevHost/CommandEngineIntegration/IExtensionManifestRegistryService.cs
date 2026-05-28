namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface IExtensionManifestRegistryService
{
    ExtensionManifestRegistrySnapshot Discover(string folderPath, bool recursive = false);
}
