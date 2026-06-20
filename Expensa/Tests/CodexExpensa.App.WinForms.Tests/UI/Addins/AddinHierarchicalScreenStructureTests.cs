using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class AddinHierarchicalScreenStructureTests
{
    [Fact]
    public void ScreenModel_DeclaresHierarchyContract()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI", "Addins", "AddinScreenModel.cs");

        Assert.Contains("public bool IsHierarchical", text);
        Assert.Contains("public string IdColumnName", text);
        Assert.Contains("public string ParentIdColumnName", text);
        Assert.Contains("public IReadOnlyList<string> HiddenColumns", text);
    }

    [Fact]
    public void ScreenForm_UsesKryptonTreeGridForHierarchicalDocuments()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI", "Addins", "AddinScreenForm.cs");

        Assert.Contains("KryptonTreeGridView", text);
        Assert.Contains("UseParentRelationship = true", text);
        Assert.Contains("IdColumnName = screen.IdColumnName", text);
        Assert.Contains("ParentIdColumnName = screen.ParentIdColumnName", text);
        Assert.Contains("grid.ExpandAll();", text);
    }

    [Fact]
    public void ScreenForm_PreservesFlatGridForNonHierarchicalDocuments()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI", "Addins", "AddinScreenForm.cs");

        Assert.Contains("screen.IsHierarchical", text);
        Assert.Contains("CreateHierarchyGrid(screen)", text);
        Assert.Contains("CreateFlatGrid(screen)", text);
    }
}
