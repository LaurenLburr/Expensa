using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesTreeTagAssignmentStructureTests
{
    [Fact]
    public void TagPickerPanel_RaisesSeparateTypedAndListedTagEvents()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeTagPickerPanel.cs");

        Assert.Contains("TypedTagAccepted", text);
        Assert.Contains("ListedTagAccepted", text);
        Assert.Contains("AcceptTypedTag", text);
        Assert.Contains("AcceptListedTag", text);
        Assert.Contains("Keys.Enter", text);
        Assert.Contains("DoubleClick", text);
    }

    [Fact]
    public void TagAssignmentService_CreatesMissingTagAndAssignment()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "HostWebsiteTagAssignmentService.cs");

        Assert.Contains("INSERT INTO [Tag]", text);
        Assert.Contains("INSERT INTO [TagAssignment]", text);
        Assert.Contains("[EntityType]", text);
        Assert.Contains("'Website'", text);
        Assert.Contains("GetOrCreateTagId", text);
        Assert.Contains("EnsureWebsiteTagAssignment", text);
    }

    [Fact]
    public void Form_AssignsAcceptedTagToSelectedWebsite()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.TagPicker.cs");

        Assert.Contains("tagPickerPanel_TypedTagAccepted", text);
        Assert.Contains("tagPickerPanel_ListedTagAccepted", text);
        Assert.Contains("AssignTagToSelectedWebsiteAsync", text);
        Assert.Contains("HostWebsiteTreeNodeTagReader.GetWebsiteId", text);
        Assert.Contains("AddOrAssignTagToWebsite", text);
        Assert.Contains("LoadWebsitesTreeAsync", text);
    }

    private static string ReadFile(
        params string[] parts)
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string path =
            Path.Combine([repositoryRoot, .. parts]);

        Assert.True(
            File.Exists(path),
            $"File was not found: {path}");

        return File.ReadAllText(path);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory =
                directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}
