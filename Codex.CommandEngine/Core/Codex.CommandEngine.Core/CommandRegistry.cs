using AbstractionsCommandHandlerDescriptor = Codex.CommandEngine.Abstractions.CommandHandlerDescriptor;
using AbstractionsICommandHandler = Codex.CommandEngine.Abstractions.ICommandHandler;
using AbstractionsICommandRegistry = Codex.CommandEngine.Abstractions.ICommandRegistry;

namespace Codex.CommandEngine.Core;

public sealed class CommandRegistry : AbstractionsICommandRegistry
{
    private readonly Dictionary<string, AbstractionsICommandHandler> _handlers =
        new(StringComparer.OrdinalIgnoreCase);

    public void Register(AbstractionsICommandHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(handler.Definition);
        ArgumentException.ThrowIfNullOrWhiteSpace(handler.Definition.CommandName);
        ArgumentException.ThrowIfNullOrWhiteSpace(handler.Definition.DisplayName);

        if (_handlers.ContainsKey(handler.Definition.CommandName))
        {
            throw new InvalidOperationException($"Command handler is already registered: {handler.Definition.CommandName}");
        }

        _handlers.Add(handler.Definition.CommandName, handler);
    }

    public bool TryResolve(string commandName, out AbstractionsICommandHandler handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

        if (_handlers.TryGetValue(commandName, out AbstractionsICommandHandler? resolvedHandler))
        {
            handler = resolvedHandler;
            return true;
        }

        handler = null!;
        return false;
    }

    public IReadOnlyList<AbstractionsCommandHandlerDescriptor> ListHandlers()
    {
        return _handlers.Values
            .OrderBy(static handler => handler.Definition.Category, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static handler => handler.Definition.CommandName, StringComparer.OrdinalIgnoreCase)
            .Select(static handler => new AbstractionsCommandHandlerDescriptor
            {
                Definition = handler.Definition,
                Handler = handler
            })
            .ToList();
    }
}
