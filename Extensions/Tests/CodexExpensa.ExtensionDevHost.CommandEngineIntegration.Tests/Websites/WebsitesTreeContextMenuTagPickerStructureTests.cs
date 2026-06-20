using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesTreeContextMenuTagPickerStructureTests
{
    [Fact]
    public void ContextMenu_AddsTagAssignmentMenuItems()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.TagPicker.cs");

        Assert.Contains("new ToolStripMenuItem(\"Assign Tag...\")", text);
        Assert.Contains("new ToolStripMenuItem(\"Remove Tag Association\")", text);
        Assert.Contains("new ToolStripMenuItem(\"Delete Node\")", text);
        Assert.Contains("AddTagPickerMenuItem", text);
        Assert.Contains("ShowTagPickerPopup", text);
        Assert.Contains("RemoveTagAssociationFromSelectedWebsiteAsync", text);
        Assert.Contains("DeleteSelectedWebsiteNodeAsync", text);
        Assert.Contains("WebsitesTreeTagPickerPanel", text);
        Assert.Contains("_tagPickerPopupForm.Show(this)", text);
        Assert.DoesNotContain("ToolStripControlHost", text);
    }

    [Fact]
    public void ContextMenu_AttachesTagPickerAfterSortMenuItems()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.ContextMenu.cs");

        Assert.Contains("Sort ASC", text);
        Assert.Contains("Sort DESC", text);
        Assert.Contains("AddTagPickerMenuItem(_runtimeTreeContextMenuStrip)", text);
        Assert.Contains("RememberTreeContextMenuLocation(e.Location)", text);
    }

    [Fact]
    public void TagPickerPanel_FiltersTagsAndRaisesTypedOrListedTagEvents()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeTagPickerPanel.cs");

        Assert.Contains("TextBox", text);
        Assert.Contains("ListBox", text);
        Assert.Contains("tagTextBox_TextChanged", text);
        Assert.Contains("tag.Contains(", text);
        Assert.Contains("TypedTagAccepted", text);
        Assert.Contains("ListedTagAccepted", text);
        Assert.Contains("AcceptTypedTag", text);
        Assert.Contains("AcceptListedTag", text);
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
