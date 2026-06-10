using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionTreeLoadTestFormStructureTests
{
    [Fact]
    public void Form_InheritsTreeTestTemplate()
    {
        string sourceText =
            TestPathHelper.ReadHostFile(
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "ExtensionManager",
                "ExtensionTreeLoadTestForm.cs");

        Assert.Contains("using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Templates;", sourceText);
        Assert.Contains("ExtensionTreeLoadTestForm : TreeTestTemplate", sourceText);
    }

    [Fact]
    public void Designer_DoesNotAddDerivedControls()
    {
        string designerText =
            TestPathHelper.ReadHostFile(
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "ExtensionManager",
                "ExtensionTreeLoadTestForm.Designer.cs");

        Assert.DoesNotContain("loadTreeButton", designerText);
        Assert.DoesNotContain("statusLabel", designerText);
        Assert.DoesNotContain("testToolStrip", designerText);
        Assert.DoesNotContain("Controls.Add", designerText);
    }
}
