using System.Windows.Forms;
using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteLoadExecutionResultAdapterTests
{
    [Fact]
    public void FromExecutionResult_WhenSucceeded_ParsesOutputJson()
    {
        CommandExecutionResult executionResult =
            CommandExecutionResult.Succeeded(
                "websites.load",
                "abc",
                "Loaded.",
                """
                {
                  "nodes": [
                    {
                      "nodeId": "banking",
                      "displayText": "Banking",
                      "children": []
                    }
                  ],
                  "message": "Loaded."
                }
                """);

        HostWebsiteLoadResult result =
            HostWebsiteLoadExecutionResultAdapter.FromExecutionResult(executionResult);

        Assert.Single(result.Nodes);
        Assert.Equal("Banking", result.Nodes[0].DisplayText);
    }

    [Fact]
    public void RenderExecutionResult_WhenSucceeded_PopulatesTreeView()
    {
        using TreeView treeView = new();

        CommandExecutionResult executionResult =
            CommandExecutionResult.Succeeded(
                "websites.load",
                "abc",
                "Loaded.",
                """
                {
                  "nodes": [
                    {
                      "nodeId": "banking",
                      "displayText": "Banking",
                      "children": []
                    }
                  ],
                  "message": "Loaded."
                }
                """);

        HostWebsiteLoadExecutionResultAdapter.RenderExecutionResult(
            treeView,
            executionResult);

        Assert.Single(treeView.Nodes);
    }
}
