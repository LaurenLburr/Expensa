namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifestValidator : IExtensionManifestValidator
{
    public ExtensionManifestValidationResult Validate(
        ExtensionManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        List<string> errors = [];

        RequireValue(manifest.ExtensionId, nameof(manifest.ExtensionId), errors);
        RequireValue(manifest.DisplayName, nameof(manifest.DisplayName), errors);
        RequireValue(manifest.Version, nameof(manifest.Version), errors);
        RequireValue(manifest.AssemblyFile, nameof(manifest.AssemblyFile), errors);
        RequireValue(manifest.ProviderType, nameof(manifest.ProviderType), errors);

        if (!string.IsNullOrWhiteSpace(manifest.Version) &&
            !Version.TryParse(manifest.Version, out _))
        {
            errors.Add("Version must be a valid version value.");
        }

        if (!string.IsNullOrWhiteSpace(manifest.MinimumHostVersion) &&
            !Version.TryParse(manifest.MinimumHostVersion, out _))
        {
            errors.Add("MinimumHostVersion must be a valid version value when provided.");
        }

        return new ExtensionManifestValidationResult
        {
            Errors = errors
        };
    }

    private static void RequireValue(
        string value,
        string propertyName,
        List<string> errors)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        errors.Add($"{propertyName} is required.");
    }
}
