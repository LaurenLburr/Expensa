using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesTreeContextMenuRuntimeAttachTests
{
    [Fact]
    public void Form_AttachesRuntimeContextMenuOnLoad()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.ContextMenu.cs");

        Assert.Contains("protected override void OnLoad", text);
        Assert.Contains("AttachRuntimeTreeContextMenu", text);
        Assert.Contains("websitesTreeView.ContextMenuStrip", text);
        Assert.Contains("Sort ASC", text);
        Assert.Contains("Sort DESC", text);
    }

    [Fact]
    public void Form_ShowsContextMenuOnRightMouseUp()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.ContextMenu.cs");

        Assert.Contains("websitesTreeView_RuntimeMouseUp", text);
        Assert.Contains("MouseButtons.Right", text);
        Assert.Contains("websitesTreeView.GetNodeAt", text);
        Assert.Contains("_runtimeTreeContextMenuStrip?.Show", text);
    }

    [Fact]
    public void Form_SortsContextMenuTargetNodes()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.ContextMenu.cs");

        Assert.Contains("SortTreeNodesFromContextMenu", text);
        Assert.Contains("websitesTreeView.SelectedNode?.Parent?.Nodes ?? websitesTreeView.Nodes", text);
        Assert.Contains("OrderBy(", text);
        Assert.Contains("sortedNodes.Reverse();", text);
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
