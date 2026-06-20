using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionManifestTests
{
    [Fact]
    public void Validate_WhenRequiredFieldsMissing_ReturnsErrors()
    {
        ExtensionManifestValidator validator = new();

        ExtensionManifestValidationResult result =
            validator.Validate(new ExtensionManifest());

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, static error => error.Contains("ExtensionId", StringComparison.Ordinal));
        Assert.Contains(result.Errors, static error => error.Contains("AssemblyFile", StringComparison.Ordinal));
        Assert.Contains(result.Errors, static error => error.Contains("ProviderType", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_WhenManifestValid_ReturnsValid()
    {
        ExtensionManifestValidator validator = new();

        ExtensionManifestValidationResult result =
            validator.Validate(ExtensionManifestTemplate.CreateForWebsitesAddin());

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Load_WhenManifestMissing_ReturnsError()
    {
        ExtensionManifestLoader loader = new();

        ExtensionManifestLoadResult result =
            loader.Load(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "extension.json"));

        Assert.False(result.Success);
        Assert.Contains("Manifest file was not found", result.Errors[0]);
    }

    [Fact]
    public void WriteThenLoad_ReturnsManifest()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                "CodexExpensa.ManifestTests",
                Guid.NewGuid().ToString("N"));

        string manifestPath =
            Path.Combine(folder, "extension.json");

        ExtensionManifest manifest =
            ExtensionManifestTemplate.CreateForWebsitesAddin();

        ExtensionManifestWriter.Write(manifestPath, manifest);

        ExtensionManifestLoadResult result =
            new ExtensionManifestLoader().Load(manifestPath);

        Assert.True(result.Success);
        Assert.NotNull(result.Manifest);
        Assert.Equal("WebsitesAddin", result.Manifest.ExtensionId);
        Assert.Equal(100, result.Manifest.DisplaySort);
        Assert.Equal("WebsitesAddin.dll", result.Manifest.AssemblyFile);
        Assert.Equal("WebsitesAddin.WebsitesAddinCommandProvider", result.Manifest.ProviderType);

        string json =
            File.ReadAllText(manifestPath);

        Assert.Contains("\"displaySort\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Validate_WhenVersionInvalid_ReturnsError()
    {
        ExtensionManifestValidator validator = new();

        ExtensionManifestValidationResult result =
            validator.Validate(new ExtensionManifest
            {
                ExtensionId = "Bad",
                DisplayName = "Bad",
                Version = "banana",
                AssemblyFile = "Bad.dll",
                ProviderType = "Bad.Provider"
            });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, static error => error.Contains("Version must be", StringComparison.Ordinal));
    }
}
