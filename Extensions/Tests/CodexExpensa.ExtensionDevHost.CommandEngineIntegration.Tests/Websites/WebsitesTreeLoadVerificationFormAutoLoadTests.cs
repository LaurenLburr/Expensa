using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesTreeLoadVerificationFormAutoLoadTests
{
    [Fact]
    public void VerificationForm_LoadsAutomaticallyOnShown()
    {
        string text = ReadFile(
        
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "WebsitesTreeLoadVerificationForm.cs");

        Assert.Contains("protected override async void OnShown", text);
        Assert.Contains("_hasAutoLoaded", text);
        Assert.Contains("await LoadWebsitesTreeAsync().ConfigureAwait(true);", text);
    }

    private static string ReadFile(params string[] parts)
    {
        string repositoryRoot = FindRepositoryRoot();
        string path = Path.Combine([repositoryRoot, .. parts]);

        Assert.True(File.Exists(path), $"File was not found: {path}");

        return File.ReadAllText(path);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
