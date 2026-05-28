namespace Codex.CommandEngine.Abstractions;

public interface ICommandEngine
{
    Task<CommandExecutionResult> ExecuteAsync(
        CommandRequest request,
        CancellationToken cancellationToken = default);
}
