using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsiteDetailsPanelStructureTests
{
    [Fact]
    public void DetailsPanel_LoadsWebsitePayloadDetails()
    {
        string text = ReadFile(
            "Extensions", "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsitesDetailsPanel.cs");

        Assert.Contains("ShowWebsite", text);
        Assert.Contains("HostWebsiteTreeNodePayload", text);
        Assert.Contains("WebsiteId:", text);
        Assert.Contains("Url:", text);
        Assert.Contains("Tag:", text);
    }

    [Fact]
    public void Form_LoadsDetailsOnTreeSelection()
    {
        string formText = ReadFile(
            "Extensions", "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsitesTreeLoadVerificationForm.cs");

        string detailsText = ReadFile(
            "Extensions", "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsitesTreeLoadVerificationForm.Details.cs");

        Assert.Contains("websitesTreeView_AfterSelect", formText);
        Assert.Contains("ShowSelectedTreeNodeDetails", formText);
        Assert.Contains("EnsureWebsiteDetailsPanel", detailsText);
        Assert.Contains("ShowWebsite", detailsText);
        Assert.Contains("ShowGroup", detailsText);
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
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException();
    }
}
