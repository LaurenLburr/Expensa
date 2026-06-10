using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.CommonTree;

public sealed class AddinTreeRendererExpansionStateTests
{
    [Fact]
    public void Renderer_CapturesAndRestoresExpandedNodeNamesWhenExpandAllIsFalse()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "CommonTree",
                "AddinTreeRenderer.cs");

        Assert.Contains("CaptureExpandedNodeNames", text);
        Assert.Contains("RestoreExpandedNodeNames", text);
        Assert.Contains("expandedNodeNames", text);
        Assert.Contains("if (expandAll)", text);
        Assert.Contains("RestoreExpandedNodeNames(treeView, expandedNodeNames)", text);
    }

    [Fact]
    public void Renderer_AddsContextMenuForNodesWithChildren()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "CommonTree",
                "AddinTreeRenderer.cs");

        Assert.Contains("EnsureNodeContextMenu", text);
        Assert.Contains("ContextMenuStrip", text);
        Assert.Contains("Expand All", text);
        Assert.Contains("Collapse All", text);
        Assert.Contains("node.Nodes.Count == 0", text);
        Assert.Contains("treeView.SelectedNode = node", text);
    }

    [Fact]
    public void Renderer_InstallsContextMenuDuringRender()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "CommonTree",
                "AddinTreeRenderer.cs");

        Assert.Contains("EnsureNodeContextMenu(treeView)", text);
    }

    private static string ReadFile(params string[] parts)
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string path =
            Path.Combine([repositoryRoot, .. parts]);

        Assert.True(File.Exists(path), $"File was not found: {path}");

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

        throw new DirectoryNotFoundException("Could not find repository root containing Extensions folder.");
    }
}
