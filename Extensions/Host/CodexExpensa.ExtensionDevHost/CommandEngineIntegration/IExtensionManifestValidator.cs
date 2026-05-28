namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface IExtensionManifestValidator
{
    ExtensionManifestValidationResult Validate(
        ExtensionManifest manifest);
}
