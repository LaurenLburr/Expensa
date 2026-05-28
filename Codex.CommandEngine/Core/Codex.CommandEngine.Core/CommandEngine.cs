using AbstractionsCommandExecutionResult = Codex.CommandEngine.Abstractions.CommandExecutionResult;
using AbstractionsCommandRequest = Codex.CommandEngine.Abstractions.CommandRequest;
using AbstractionsICommandEngine = Codex.CommandEngine.Abstractions.ICommandEngine;
using AbstractionsICommandHandler = Codex.CommandEngine.Abstractions.ICommandHandler;
using AbstractionsICommandRegistry = Codex.CommandEngine.Abstractions.ICommandRegistry;
using Codex.CommandEngine.Abstractions;

namespace Codex.CommandEngine.Core;

public sealed class CommandEngine : AbstractionsICommandEngine
{
    private readonly AbstractionsICommandRegistry _registry;

    public CommandEngine()
        : this(new CommandRegistry())
    {
    }

    public CommandEngine(AbstractionsICommandRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);

        _registry = registry;
    }

    public void Register(
        string commandName,
        Func<AbstractionsCommandRequest, CancellationToken, Task<AbstractionsCommandExecutionResult>> handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);
        ArgumentNullException.ThrowIfNull(handler);

        Register(new DelegateCommandHandler(
            new CommandDefinition
            {
                CommandName = commandName,
                DisplayName = commandName,
                HandlerType = typeof(DelegateCommandHandler).FullName ?? nameof(DelegateCommandHandler)
            },
            handler));
    }

    public void Register(AbstractionsICommandHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _registry.Register(handler);
    }

    public Task<AbstractionsCommandExecutionResult> ExecuteAsync(
        AbstractionsCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CommandName);

        if (!_registry.TryResolve(request.CommandName, out AbstractionsICommandHandler handler))
        {
            return Task.FromResult(AbstractionsCommandExecutionResult.Failure($"Command not found: {request.CommandName}"));
        }

        return handler.ExecuteAsync(request, cancellationToken);
    }
}
