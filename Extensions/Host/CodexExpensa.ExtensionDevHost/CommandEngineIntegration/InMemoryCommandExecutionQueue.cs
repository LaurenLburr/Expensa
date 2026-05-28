using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class InMemoryCommandExecutionQueue : ICommandExecutionQueue
{
    private readonly ExtensionRuntimeDashboardController _controller;
    private readonly object _syncRoot = new();
    private readonly List<CommandExecutionQueueItem> _items = [];
    private readonly Dictionary<Guid, CancellationTokenSource> _cancellationTokenSources = [];

    public InMemoryCommandExecutionQueue(
        ExtensionRuntimeDashboardController controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        _controller = controller;
    }

    public event EventHandler? Changed;

    public CommandExecutionQueueItem Enqueue(
        string commandName,
        string parameterJson)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

        CommandExecutionQueueItem item = new()
        {
            CommandName = commandName,
            ParameterJson = string.IsNullOrWhiteSpace(parameterJson) ? "{}" : parameterJson
        };

        CancellationTokenSource cancellationTokenSource = new();

        lock (_syncRoot)
        {
            _items.Add(item);
            _cancellationTokenSources[item.QueueItemId] = cancellationTokenSource;
        }

        OnChanged();

        _ = Task.Run(
            async () => await RunItemAsync(item, cancellationTokenSource.Token).ConfigureAwait(false),
            CancellationToken.None);

        return item;
    }

    public void AddRecoveredItem(
        CommandExecutionQueueItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (_syncRoot)
        {
            bool alreadyExists =
                _items.Any(candidate => candidate.QueueItemId == item.QueueItemId);

            if (!alreadyExists)
            {
                _items.Add(item);
            }
        }

        OnChanged();
    }

    public CommandExecutionQueueSnapshot GetSnapshot()
    {
        lock (_syncRoot)
        {
            return new CommandExecutionQueueSnapshot
            {
                Items = _items
                    .OrderByDescending(static item => item.CreatedUtc)
                    .ToList()
            };
        }
    }

    public bool TryCancel(
        Guid queueItemId)
    {
        CancellationTokenSource? cancellationTokenSource;

        lock (_syncRoot)
        {
            CommandExecutionQueueItem? item =
                _items.FirstOrDefault(candidate => candidate.QueueItemId == queueItemId);

            if (item is null)
            {
                return false;
            }

            if (item.Status == CommandExecutionQueueStatus.Completed ||
                item.Status == CommandExecutionQueueStatus.Failed ||
                item.Status == CommandExecutionQueueStatus.Cancelled)
            {
                return false;
            }

            item.Status = CommandExecutionQueueStatus.Cancelled;
            item.CompletedUtc = DateTimeOffset.UtcNow;
            item.Message = "Cancelled by user.";

            _cancellationTokenSources.TryGetValue(queueItemId, out cancellationTokenSource);
        }

        cancellationTokenSource?.Cancel();
        OnChanged();

        return true;
    }

    public void ClearCompleted()
    {
        lock (_syncRoot)
        {
            _items.RemoveAll(static item =>
                item.Status == CommandExecutionQueueStatus.Completed ||
                item.Status == CommandExecutionQueueStatus.Failed ||
                item.Status == CommandExecutionQueueStatus.Cancelled);
        }

        OnChanged();
    }

    private async Task RunItemAsync(
        CommandExecutionQueueItem item,
        CancellationToken cancellationToken)
    {
        try
        {
            MarkRunning(item);

            IReadOnlyDictionary<string, object?> parameters =
                CommandParameterJsonParser.Parse(item.ParameterJson);

            CommandExecutionResult result =
                parameters.Count == 0
                    ? await _controller.ExecuteCommandAsync(item.CommandName, cancellationToken).ConfigureAwait(false)
                    : await _controller.ExecuteCommandAsync(item.CommandName, parameters, cancellationToken).ConfigureAwait(false);

            MarkCompleted(item, result);
        }
        catch (OperationCanceledException)
        {
            MarkCancelled(item);
        }
        catch (Exception exception)
        {
            MarkFailed(item, exception);
        }
        finally
        {
            lock (_syncRoot)
            {
                _cancellationTokenSources.Remove(item.QueueItemId);
            }

            OnChanged();
        }
    }

    private void MarkRunning(
        CommandExecutionQueueItem item)
    {
        lock (_syncRoot)
        {
            if (item.Status == CommandExecutionQueueStatus.Cancelled)
            {
                return;
            }

            item.Status = CommandExecutionQueueStatus.Running;
            item.StartedUtc = DateTimeOffset.UtcNow;
            item.Message = "Running.";
        }

        OnChanged();
    }

    private void MarkCompleted(
        CommandExecutionQueueItem item,
        CommandExecutionResult result)
    {
        lock (_syncRoot)
        {
            if (item.Status == CommandExecutionQueueStatus.Cancelled)
            {
                return;
            }

            item.Status = result.Status == CommandExecutionStatus.Succeeded
                ? CommandExecutionQueueStatus.Completed
                : CommandExecutionQueueStatus.Failed;

            item.CompletedUtc = DateTimeOffset.UtcNow;
            item.Message = result.Message;
            item.CorrelationId = result.CorrelationId;
            item.OutputJson = result.OutputJson;
        }
    }

    private void MarkCancelled(
        CommandExecutionQueueItem item)
    {
        lock (_syncRoot)
        {
            item.Status = CommandExecutionQueueStatus.Cancelled;
            item.CompletedUtc = DateTimeOffset.UtcNow;
            item.Message = "Cancelled.";
        }
    }

    private void MarkFailed(
        CommandExecutionQueueItem item,
        Exception exception)
    {
        lock (_syncRoot)
        {
            item.Status = CommandExecutionQueueStatus.Failed;
            item.CompletedUtc = DateTimeOffset.UtcNow;
            item.Message = exception.Message;
        }
    }

    private void OnChanged()
    {
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
