namespace Codex.CommandEngine.Core;

public interface ICommandDispatcher
{
    void Register(ICommandHandler handler);

    Task<CommandExecutionResult> ExecuteAsync(CommandExecutionRequest request, CancellationToken cancellationToken = default);
}
