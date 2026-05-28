using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifestLoader : IExtensionManifestLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private readonly IExtensionManifestValidator _validator;

    public ExtensionManifestLoader()
        : this(new ExtensionManifestValidator())
    {
    }

    public ExtensionManifestLoader(
        IExtensionManifestValidator validator)
    {
        ArgumentNullException.ThrowIfNull(validator);

        _validator = validator;
    }

    public ExtensionManifestLoadResult Load(
        string manifestPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestPath);

        if (!File.Exists(manifestPath))
        {
            return new ExtensionManifestLoadResult
            {
                Errors = [$"Manifest file was not found: {manifestPath}"]
            };
        }

        try
        {
            string json =
                File.ReadAllText(manifestPath);

            ExtensionManifest? manifest =
                JsonSerializer.Deserialize<ExtensionManifest>(json, JsonOptions);

            if (manifest is null)
            {
                return new ExtensionManifestLoadResult
                {
                    Errors = ["Manifest file could not be deserialized."]
                };
            }

            ExtensionManifestValidationResult validationResult =
                _validator.Validate(manifest);

            return new ExtensionManifestLoadResult
            {
                Manifest = validationResult.IsValid ? manifest : null,
                Errors = validationResult.Errors
            };
        }
        catch (Exception exception)
        {
            return new ExtensionManifestLoadResult
            {
                Errors = [exception.Message]
            };
        }
    }
}
