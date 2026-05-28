namespace Codex.CommandEngine.Core;

public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<string, ICommandHandler> _handlers =
        new(StringComparer.OrdinalIgnoreCase);

    public void Register(ICommandHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentException.ThrowIfNullOrWhiteSpace(handler.CommandName);

        if (_handlers.ContainsKey(handler.CommandName))
        {
            throw new InvalidOperationException($"A command handler is already registered for '{handler.CommandName}'.");
        }

        _handlers.Add(handler.CommandName, handler);
    }

    public async Task<CommandExecutionResult> ExecuteAsync(CommandExecutionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CommandName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CorrelationId);

        if (cancellationToken.IsCancellationRequested)
        {
            return CommandExecutionResult.Cancelled(request.CommandName, request.CorrelationId);
        }

        if (!_handlers.TryGetValue(request.CommandName, out ICommandHandler? handler))
        {
            return CommandExecutionResult.Failed(request.CommandName, request.CorrelationId, $"No command handler is registered for '{request.CommandName}'.");
        }

        CommandExecutionContext context =
            new(request.CorrelationId, request.ContextJson, cancellationToken);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await handler.ExecuteAsync(request, context).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return CommandExecutionResult.Cancelled(request.CommandName, request.CorrelationId);
        }
        catch (Exception exception)
        {
            return CommandExecutionResult.Failed(request.CommandName, request.CorrelationId, exception.Message, exception);
        }
    }
}
