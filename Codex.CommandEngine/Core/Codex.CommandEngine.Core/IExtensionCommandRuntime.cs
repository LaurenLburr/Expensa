namespace Codex.CommandEngine.Core;

public interface IExtensionCommandRuntime
{
    IReadOnlyList<RuntimeCommandDescriptor> ListCommands();

    bool TryGetCommand(
        string commandName,
        out RuntimeCommandDescriptor descriptor);

    Task<CommandExecutionResult> ExecuteCommandAsync(
        ExtensionRuntimeCommandRequest request,
        CancellationToken cancellationToken = default);

    Task<WorkflowExecutionResult> ExecuteWorkflowAsync(
        ExtensionRuntimeWorkflowRequest request,
        CancellationToken cancellationToken = default);
}
