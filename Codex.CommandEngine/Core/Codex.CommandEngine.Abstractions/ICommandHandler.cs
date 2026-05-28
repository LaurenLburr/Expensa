namespace Codex.CommandEngine.Abstractions;

public interface ICommandHandler
{
    CommandDefinition Definition { get; }

    Task<CommandExecutionResult> ValidateAsync(
        CommandRequest request,
        CancellationToken cancellationToken = default);

    Task<CommandExecutionResult> ExecuteAsync(
        CommandRequest request,
        CancellationToken cancellationToken = default);
}
