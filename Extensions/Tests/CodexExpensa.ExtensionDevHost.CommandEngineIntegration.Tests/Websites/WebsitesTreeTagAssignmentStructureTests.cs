using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesTreeTagAssignmentStructureTests
{
    [Fact]
    public void TagPickerPanel_RaisesSeparateTypedAndListedTagEvents()
    {
        string text =
            ReadFile(
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
        Assert.Contains("SafeSqliteConnection.OpenFromFile", text);
        Assert.Contains("SafeSqliteConnection.SaveToFile", text);
    }

    [Fact]
    public void TagAssignmentService_RemovesWebsiteTagAssignment()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "HostWebsiteTagAssignmentService.cs");

        Assert.Contains("RemoveWebsiteTagAssignments", text);
        Assert.Contains("UPDATE [TagAssignment]", text);
        Assert.Contains("SET [IsActive] = 0", text);
        Assert.Contains("SafeSqliteConnection.OpenFromFile", text);
        Assert.Contains("SafeSqliteConnection.SaveToFile", text);
    }

    [Fact]
    public void TagAssignmentService_DeletesWebsiteNode()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "HostWebsiteTagAssignmentService.cs");

        Assert.Contains("DeleteWebsiteNode", text);
        Assert.Contains("UPDATE [Website]", text);
        Assert.Contains("SET [IsEnabled] = 0", text);
        Assert.Contains("UPDATE [TagAssignment]", text);
        Assert.Contains("SafeSqliteConnection.OpenFromFile", text);
        Assert.Contains("SafeSqliteConnection.SaveToFile", text);
    }

    [Fact]
    public void Form_AssignsAcceptedTagToSelectedWebsite()
    {
        string text =
            ReadFile(
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

    [Fact]
    public void Form_RemovesTagAssociationFromSelectedWebsite()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.TagPicker.cs");

        Assert.Contains("removeTagAssociationMenuItem_Click", text);
        Assert.Contains("RemoveTagAssociationFromSelectedWebsiteAsync", text);
        Assert.Contains("RemoveWebsiteTagAssignments", text);
        Assert.Contains("Remove Website Tag Association", text);
        Assert.Contains("LoadWebsitesTreeAsync", text);
    }

    [Fact]
    public void Form_DeletesSelectedWebsiteNode()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.TagPicker.cs");

        Assert.Contains("deleteNodeMenuItem_Click", text);
        Assert.Contains("DeleteSelectedWebsiteNodeAsync", text);
        Assert.Contains("DeleteWebsiteNode", text);
        Assert.Contains("MessageBoxButtons.YesNo", text);
        Assert.Contains("Delete Website Node", text);
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
        return TestPathHelper.ExtensionsRoot;
    }
}
