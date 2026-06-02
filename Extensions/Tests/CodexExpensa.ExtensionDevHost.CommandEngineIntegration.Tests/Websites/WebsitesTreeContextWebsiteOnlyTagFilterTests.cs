using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesTreeContextWebsiteOnlyTagFilterTests
{
    [Fact]
    public void ContextMenu_ShowsOnlyForWebsiteNodes()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.ContextMenu.cs");

        Assert.Contains("IsWebsiteTreeNode(clickedNode)", text);
        Assert.Contains("Context menu is available only for website nodes.", text);
        Assert.Contains("HostWebsiteTreeNodeTagReader.GetWebsiteId(node)", text);
        Assert.Contains("websitesTreeView.ContextMenuStrip = null", text);
    }

    [Fact]
    public void TagPickerPopup_UsesTreeHeightAndFiltersAssignedTags()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.TagPicker.cs");

        Assert.Contains("websitesTreeView.ClientSize.Height", text);
        Assert.Contains("GetAvailableTreeTagNamesForWebsite", text);
        Assert.Contains("GetAssignedTagNamesForWebsite", text);
        Assert.Contains("assignedTags.Contains(tagName)", text);
        Assert.Contains("SetTags(", text);
    }

    [Fact]
    public void TagAssignmentService_ReadsAssignedTagNames()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "HostWebsiteTagAssignmentService.cs");

        Assert.Contains("GetAssignedTagNames", text);
        Assert.Contains("SELECT DISTINCT t.[TagName]", text);
        Assert.Contains("INNER JOIN [Tag] t", text);
        Assert.Contains("ta.[EntityType] = 'Website'", text);
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
