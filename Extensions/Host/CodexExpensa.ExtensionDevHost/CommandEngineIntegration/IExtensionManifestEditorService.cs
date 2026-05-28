namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface IExtensionManifestEditorService
{
    ExtensionManifestUpdateResult SetEnabled(
        string manifestPath,
        bool enabled);
}
