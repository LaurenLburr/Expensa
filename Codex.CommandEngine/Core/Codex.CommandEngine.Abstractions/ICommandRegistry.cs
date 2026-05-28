namespace Codex.CommandEngine.Abstractions;

public interface ICommandRegistry
{
    void Register(ICommandHandler handler);

    bool TryResolve(string commandName, out ICommandHandler handler);

    IReadOnlyList<CommandHandlerDescriptor> ListHandlers();
}
