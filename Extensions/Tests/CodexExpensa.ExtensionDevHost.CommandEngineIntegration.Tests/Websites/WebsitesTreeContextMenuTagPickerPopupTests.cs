using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesTreeContextMenuTagPickerPopupTests
{
    [Fact]
    public void TagPicker_UsesPopupFormInsteadOfToolStripControlHost()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.TagPicker.cs");

        Assert.Contains("ShowTagPickerPopup", text);
        Assert.Contains("new Form", text);
        Assert.Contains("WebsitesTreeTagPickerPanel", text);
        Assert.Contains("_tagPickerPopupForm.Show(this)", text);
        Assert.Contains("FocusTextBox", text);
        Assert.DoesNotContain("ToolStripControlHost", text);
    }

    [Fact]
    public void ContextMenu_RemembersRightClickLocationBeforeShowingPopup()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.ContextMenu.cs");

        Assert.Contains("RememberTreeContextMenuLocation(e.Location)", text);
        Assert.Contains("_runtimeTreeContextMenuStrip?.Show", text);
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
