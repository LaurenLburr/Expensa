using Codex.CommandEngine.Core;

namespace Codex.CommandEngine.App;

public sealed class RuntimeOperationsHostViewBuilder
{
    private readonly Func<RuntimeOperationsViewService?> _viewServiceFactory;

    public RuntimeOperationsHostViewBuilder(
        Func<RuntimeOperationsViewService?> viewServiceFactory)
    {
        ArgumentNullException.ThrowIfNull(viewServiceFactory);

        _viewServiceFactory = viewServiceFactory;
    }

    public string BuildTextForNode(string nodeKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nodeKey);

        RuntimeOperationsViewService? viewService =
            _viewServiceFactory();

        if (viewService is null)
        {
            return BuildUnavailableText();
        }

        try
        {
            return nodeKey switch
            {
                HostNodeKeys.RuntimeResumableWorkflows =>
                    viewService.BuildResumableWorkflowsText(),

                HostNodeKeys.RuntimeStaleWorkflows =>
                    viewService.BuildStaleWorkflowsText(DateTimeOffset.UtcNow),

                HostNodeKeys.RuntimeIncompleteWorkflows =>
                    viewService.BuildIncompleteWorkflowsText(),

                HostNodeKeys.RuntimeOperations =>
                    BuildOverviewText(),

                _ =>
                    BuildUnknownNodeText(nodeKey)
            };
        }
        catch (Exception exception)
        {
            return BuildErrorText(nodeKey, exception);
        }
    }

    private static string BuildOverviewText()
    {
        return
            """
            Runtime Operations
            ==================

            Select a child node to inspect workflow runtime state.

            Available views:
            - Resumable Workflows
            - Stale Workflows
            - Incomplete Workflows

            This view is read-only for now. Resume/abandon buttons come later, after the display layer behaves itself.
            """;
    }

    private static string BuildUnavailableText()
    {
        return
            """
            Runtime Operations Unavailable
            ==============================

            Runtime operations are not currently configured.

            The Host needs an IWorkflowRuntimeOperations implementation before this view can display live workflow runtime data.
            """;
    }

    private static string BuildUnknownNodeText(string nodeKey)
    {
        return
            $"""
            Runtime Operations
            ==================

            Unknown runtime operations node:

            {nodeKey}
            """;
    }

    private static string BuildErrorText(string nodeKey, Exception exception)
    {
        return
            $"""
            Runtime Operations Error
            ========================

            Node:
            {nodeKey}

            Error:
            {exception.Message}

            Details:
            {exception}
            """;
    }
}
