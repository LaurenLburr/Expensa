namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class ExtensionManifestTemplate
{
    public static ExtensionManifest CreateForWebsitesAddin()
    {
        return new ExtensionManifest
        {
            ExtensionId = "WebsitesAddin",
            DisplayName = "Websites Add-in",
            DisplaySort = 100,
            Version = "1.0.0",
            AssemblyFile = "WebsitesAddin.dll",
            ProviderType = "WebsitesAddin.WebsitesAddinCommandProvider",
            MinimumHostVersion = "1.0.0",
            Enabled = true,
            Description = "CommandEngine-enabled Websites add-in."
        };
    }
}
