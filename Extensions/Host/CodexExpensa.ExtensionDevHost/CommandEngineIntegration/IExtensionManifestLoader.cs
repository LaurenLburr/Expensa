namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface IExtensionManifestLoader
{
    ExtensionManifestLoadResult Load(
        string manifestPath);
}
