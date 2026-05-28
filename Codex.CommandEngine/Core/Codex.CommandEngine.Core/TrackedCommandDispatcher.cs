using System.Diagnostics;

namespace Codex.CommandEngine.Core;

public sealed class TrackedCommandDispatcher : ICommandDispatcher
{
    private readonly ICommandDispatcher _innerDispatcher;
    private readonly ICommandExecutionHistorySink _historySink;

    public TrackedCommandDispatcher(
        ICommandDispatcher innerDispatcher,
        ICommandExecutionHistorySink historySink)
    {
        ArgumentNullException.ThrowIfNull(innerDispatcher);
        ArgumentNullException.ThrowIfNull(historySink);

        _innerDispatcher = innerDispatcher;
        _historySink = historySink;
    }

    public void Register(ICommandHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _innerDispatcher.Register(handler);
    }

    public async Task<CommandExecutionResult> ExecuteAsync(
        CommandExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CommandName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CorrelationId);

        string executionId = Guid.NewGuid().ToString("N");
        DateTimeOffset startedUtc = DateTimeOffset.UtcNow;
        Stopwatch stopwatch = Stopwatch.StartNew();

        _historySink.Started(request, executionId, startedUtc);

        CommandExecutionResult result;

        try
        {
            result = await _innerDispatcher
                .ExecuteAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            result = CommandExecutionResult.Cancelled(
                request.CommandName,
                request.CorrelationId);
        }
        catch (Exception exception)
        {
            result = CommandExecutionResult.Failed(
                request.CommandName,
                request.CorrelationId,
                exception.Message,
                exception);
        }

        stopwatch.Stop();

        _historySink.Completed(
            request,
            result,
            executionId,
            DateTimeOffset.UtcNow,
            stopwatch.ElapsedMilliseconds);

        return result;
    }
}
