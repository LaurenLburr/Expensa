namespace Codex.CommandEngine.Core;

public interface IExtensionRuntimeHost
{
    ExtensionRuntimeHostState State { get; }

    ExtensionCommandRuntimeBootstrapResult? LastBootstrapResult { get; }

    ExtensionCommandRuntimeBootstrapResult Start(
        ExtensionCommandRuntimeBootstrapRequest request);

    ExtensionCommandRuntimeBootstrapResult Reload(
        ExtensionCommandRuntimeBootstrapRequest request);

    void Stop();

    ExtensionRuntimeHostSnapshot GetSnapshot();

    IReadOnlyList<RuntimeCommandDescriptor> ListCommands();

    Task<CommandExecutionResult> ExecuteCommandAsync(
        ExtensionRuntimeCommandRequest request,
        CancellationToken cancellationToken = default);

    Task<WorkflowExecutionResult> ExecuteWorkflowAsync(
        ExtensionRuntimeWorkflowRequest request,
        CancellationToken cancellationToken = default);
}
