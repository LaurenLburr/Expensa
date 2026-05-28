namespace Codex.CommandEngine.Core;

public sealed class ExtensionCommandRuntime : IExtensionCommandRuntime
{
    private readonly ICommandEngineRuntime _runtime;

    public ExtensionCommandRuntime(ICommandEngineRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(runtime);

        _runtime = runtime;
    }

    public IReadOnlyList<RuntimeCommandDescriptor> ListCommands()
    {
        return _runtime.ListRegisteredCommands();
    }

    public bool TryGetCommand(
        string commandName,
        out RuntimeCommandDescriptor descriptor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

        return _runtime.TryGetRegisteredCommand(commandName, out descriptor);
    }

    public Task<CommandExecutionResult> ExecuteCommandAsync(
        ExtensionRuntimeCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CommandName);

        return _runtime.ExecuteCommandAsync(
            new CommandExecutionRequest
            {
                CommandName = request.CommandName,
                CorrelationId = request.CorrelationId,
                ContextJson = request.ContextJson,
                Parameters = request.Parameters
            },
            cancellationToken);
    }

    public Task<WorkflowExecutionResult> ExecuteWorkflowAsync(
        ExtensionRuntimeWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkflowName);

        return _runtime.ExecuteWorkflowAsync(
            new WorkflowExecutionRequest
            {
                WorkflowName = request.WorkflowName,
                CorrelationId = request.CorrelationId,
                ContextJson = request.ContextJson,
                Steps = request.Steps
            },
            cancellationToken);
    }
}
