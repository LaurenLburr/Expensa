using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;
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
        Assert.Contains("protected DataGridView gridData", designerText);
        Assert.Contains("protected TextBox textNotes", designerText);
        Assert.Contains("protected Label labelAdd_in_Name", designerText);
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
