namespace Codex.CommandEngine.Core;

public interface ICommandHandler
{
    string CommandName { get; }

    Task<CommandExecutionResult> ExecuteAsync(CommandExecutionRequest request, ICommandExecutionContext context);
}
