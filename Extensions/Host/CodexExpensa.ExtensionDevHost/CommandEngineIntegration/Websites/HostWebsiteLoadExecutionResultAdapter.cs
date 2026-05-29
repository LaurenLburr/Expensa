using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteLoadExecutionResultAdapter
{
    public static HostWebsiteLoadResult FromExecutionResult(
        CommandExecutionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.Status != CommandExecutionStatus.Succeeded)
        {
            return new HostWebsiteLoadResult
            {
                Message = result.Message
            };
        }

        return HostWebsiteLoadResultParser.Parse(result.OutputJson);
    }

    public static void RenderExecutionResult(
        TreeView treeView,
        CommandExecutionResult result)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(result);

        HostWebsiteLoadResult loadResult =
            FromExecutionResult(result);

        HostWebsiteTreeViewRenderer.Render(treeView, loadResult);
    }
}
