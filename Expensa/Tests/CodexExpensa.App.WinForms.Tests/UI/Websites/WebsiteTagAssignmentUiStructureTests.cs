using CodexExpensa.App.WinForms.Tests.UI.Addins;
using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Websites;

public sealed class WebsiteTagAssignmentUiStructureTests
{
    [Fact]
    public void MainForm_AddsWebsiteNodeContextMenuForTagAssignments()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "MainForm.cs");

        Assert.Contains("_ctxWebsiteNode", text);
        Assert.Contains("BuildWebsiteNodeMenu", text);
        Assert.Contains("IsWebsiteNavigationNode(e.Node)", text);
        Assert.Contains("TryGetSelectedWebsiteForTagging", text);
        Assert.Contains("selectedNode.Nodes.Count == 0", text);
        Assert.Contains("current.Name, \"addin.websites\"", text);
        Assert.Contains("Assign Tag...", text);
        Assert.Contains("Remove Tag Association", text);
        Assert.Contains("ShowWebsiteTagPicker", text);
        Assert.Contains("RemoveSelectedWebsiteTagAssociationsAsync", text);
    }

    [Fact]
    public void TagPickerPanel_FiltersListAndAcceptsTags()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI",
            "Websites",
            "WebsiteTagPickerPanel.cs");

        Assert.Contains("TextBox", text);
        Assert.Contains("ListBox", text);
        Assert.Contains("TextChanged", text);
        Assert.Contains("tag.Contains(typedText", text);
        Assert.Contains("DoubleClick", text);
        Assert.Contains("Keys.Enter", text);
        Assert.Contains("TypedTagAccepted", text);
        Assert.Contains("ListedTagAccepted", text);
    }

    [Fact]
    public void TagAssignmentService_UsesWebsiteTagAssignmentTables()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI",
            "Websites",
            "WebsiteTagAssignmentService.cs");

        Assert.Contains("[Tag]", text);
        Assert.Contains("[TagAssignment]", text);
        Assert.Contains("EntityType", text);
        Assert.Contains("\"Website\"", text);
        Assert.Contains("AssignTagToWebsite", text);
        Assert.Contains("RemoveWebsiteTagAssociations", text);
        Assert.Contains("GetActiveTagNames", text);
    }
}
