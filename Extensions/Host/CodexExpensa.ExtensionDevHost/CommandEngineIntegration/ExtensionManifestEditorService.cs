namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifestEditorService : IExtensionManifestEditorService
{
    private readonly IExtensionManifestLoader _loader;

    public ExtensionManifestEditorService()
        : this(new ExtensionManifestLoader())
    {
    }

    public ExtensionManifestEditorService(
        IExtensionManifestLoader loader)
    {
        ArgumentNullException.ThrowIfNull(loader);

        _loader = loader;
    }

    public ExtensionManifestUpdateResult SetEnabled(
        string manifestPath,
        bool enabled)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestPath);

        ExtensionManifestLoadResult loadResult =
            _loader.Load(manifestPath);

        if (!loadResult.Success || loadResult.Manifest is null)
        {
            return new ExtensionManifestUpdateResult
            {
                Success = false,
                Message = string.Join(Environment.NewLine, loadResult.Errors)
            };
        }

        ExtensionManifest updatedManifest =
            new()
            {
                ExtensionId = loadResult.Manifest.ExtensionId,
                DisplayName = loadResult.Manifest.DisplayName,
                DisplaySort = loadResult.Manifest.DisplaySort,
                Version = loadResult.Manifest.Version,
                AssemblyFile = loadResult.Manifest.AssemblyFile,
                ProviderType = loadResult.Manifest.ProviderType,
                MinimumHostVersion = loadResult.Manifest.MinimumHostVersion,
                Enabled = enabled,
                Description = loadResult.Manifest.Description
            };

        ExtensionManifestWriter.Write(
            manifestPath,
            updatedManifest);

        return new ExtensionManifestUpdateResult
        {
            Success = true,
            Message = enabled
                ? "Manifest enabled."
                : "Manifest disabled.",
            Manifest = updatedManifest
        };
    }

    public ExtensionManifestUpdateResult SetDisplaySort(
        string manifestPath,
        int displaySort)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestPath);

        ExtensionManifestLoadResult loadResult =
            _loader.Load(manifestPath);

        if (!loadResult.Success || loadResult.Manifest is null)
        {
            return new ExtensionManifestUpdateResult
            {
                Success = false,
                Message = string.Join(Environment.NewLine, loadResult.Errors)
            };
        }

        ExtensionManifest updatedManifest =
            new()
            {
                ExtensionId = loadResult.Manifest.ExtensionId,
                DisplayName = loadResult.Manifest.DisplayName,
                DisplaySort = displaySort,
                Version = loadResult.Manifest.Version,
                AssemblyFile = loadResult.Manifest.AssemblyFile,
                ProviderType = loadResult.Manifest.ProviderType,
                MinimumHostVersion = loadResult.Manifest.MinimumHostVersion,
                Enabled = loadResult.Manifest.Enabled,
                Description = loadResult.Manifest.Description
            };

        ExtensionManifestWriter.Write(
            manifestPath,
            updatedManifest);

        return new ExtensionManifestUpdateResult
        {
            Success = true,
            Message = "Manifest display sort updated.",
            Manifest = updatedManifest
        };
    }
}
