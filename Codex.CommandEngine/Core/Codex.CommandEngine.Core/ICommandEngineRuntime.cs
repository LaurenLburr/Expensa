namespace Codex.CommandEngine.Core;

public interface ICommandEngineRuntime
{
    void RegisterCommand(ICommandHandler handler);

    void RegisterCommand(RuntimeCommandRegistration registration);

    void RegisterCommands(IEnumerable<RuntimeCommandRegistration> registrations);

    IReadOnlyList<string> ListRegisteredCommandNames();

    IReadOnlyList<RuntimeCommandDescriptor> ListRegisteredCommands();

    bool TryGetRegisteredCommand(
        string commandName,
        out RuntimeCommandDescriptor descriptor);

    Task<CommandExecutionResult> ExecuteCommandAsync(
        CommandExecutionRequest request,
        CancellationToken cancellationToken = default);

    Task<WorkflowExecutionResult> ExecuteWorkflowAsync(
        WorkflowExecutionRequest request,
        CancellationToken cancellationToken = default);
}
