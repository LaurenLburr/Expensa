using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesTreeLoadVerificationFormStructureTests
{
    [Fact]
    public void VerificationForm_UsesTreeTestTemplateWithoutAddingDerivedControls()
    {
        string formText = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "WebsitesTreeLoadVerificationForm.cs");

        string designerText = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "WebsitesTreeLoadVerificationForm.Designer.cs");

        Assert.Contains("WebsitesTreeLoadVerificationForm : TreeTestTemplate", formText);
        Assert.Contains("TestTreeView", formText);
        Assert.Contains("NotesTextBox", formText);
        Assert.Contains("ContentSplitContainer", formText);

        Assert.DoesNotContain("filterLayoutPanel", designerText);
        Assert.DoesNotContain("loadButton", designerText);
        Assert.DoesNotContain("statusLabel", designerText);
        Assert.DoesNotContain("closeButton", designerText);
        Assert.DoesNotContain("Controls.Add", designerText);
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
