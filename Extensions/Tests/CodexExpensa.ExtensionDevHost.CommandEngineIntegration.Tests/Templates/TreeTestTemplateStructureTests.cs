using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Templates;

public sealed class TreeTestTemplateStructureTests
{
    [Fact]
    public void TreeTestTemplate_ExposesSharedTreeTestControls()
    {
        string designerText = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Templates",
            "TreeTestTemplate.Designer.cs");

        Assert.Contains("protected SplitContainer splitContainer1", designerText);
        Assert.Contains("protected SplitContainer splitContainer2", designerText);
        Assert.Contains("protected TreeView tree", designerText);
        Assert.Contains(
            "protected Krypton.Toolkit.Suite.Extended.TreeGridView.KryptonTreeGridView gridData",
            designerText);
        Assert.Contains("protected TextBox textNotes", designerText);
        Assert.Contains("protected Label labelAdd_in_Name", designerText);
    }

    [Fact]
    public void TreeTestTemplate_ProvidesFlatAndHierarchicalGridSurfaces()
    {
        string codeText = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Templates",
            "TreeTestTemplate.cs");

        Assert.Contains(
            "protected DataGridView ResultGrid => gridData;",
            codeText);

        Assert.Contains(
            "protected KryptonTreeGridView HierarchyGrid => gridData;",
            codeText);

        Assert.Contains(
            "protected void SetHierarchicalGridData(",
            codeText);

        Assert.Contains(
            "gridData.UseParentRelationship = true;",
            codeText);

        Assert.Contains(
            "gridData.IdColumnName = idColumnName;",
            codeText);

        Assert.Contains(
            "gridData.ParentIdColumnName = parentIdColumnName;",
            codeText);

        Assert.Contains(
            "gridData.ExpandAll();",
            codeText);

        Assert.Contains(
            "gridData.CollapseAll();",
            codeText);
    }

    [Fact]
    public void TreeTestTemplate_CopiesDataTablesBeforeBinding()
    {
        string codeText = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Templates",
            "TreeTestTemplate.cs");

        Assert.Contains(
            "gridData.DataSource = table.Copy();",
            codeText);

        Assert.Contains(
            "KryptonTreeGridView owns and disposes its DataTable",
            codeText);
    }

    [Fact]
    public void TreeTestTemplate_ClearGridUsesOneColumnPlaceholderTable()
    {
        string codeText = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Templates",
            "TreeTestTemplate.cs");

        Assert.Contains(
            "placeholder.Columns.Add(",
            codeText);

        Assert.Contains(
            "\"__Empty\"",
            codeText);

        Assert.Contains(
            "gridData.DataSource = placeholder;",
            codeText);

        Assert.Contains(
            "gridData.Columns[\"__Empty\"]!.Visible = false;",
            codeText);

        Assert.DoesNotContain(
            "new DataTable(\"EmptyTreeGrid\")",
            codeText);
    }

    [Fact]
    public void TreeTestTemplate_SetsPageLabelFromConfiguredTitle()
    {
        string codeText = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Templates",
            "TreeTestTemplate.cs");

        Assert.Contains("labelAdd_in_Name.Text = title;", codeText);
        Assert.Contains("Text = title;", codeText);
        Assert.Contains("textNotes.Text = notesText;", codeText);
    }

    [Fact]
    public void TreeTestTemplate_UsesDesignerPattern()
    {
        string codeText = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Templates",
            "TreeTestTemplate.cs");

        string designerText = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Templates",
            "TreeTestTemplate.Designer.cs");

        Assert.Contains("public partial class TreeTestTemplate : Form", codeText);
        Assert.Contains("InitializeComponent();", codeText);
        Assert.Contains("partial class TreeTestTemplate", designerText);
        Assert.Contains("private void InitializeComponent()", designerText);
    }
}
