namespace Codex.CommandEngine.Core;

public sealed class CommandExecutionContext : ICommandExecutionContext
{
    private readonly List<string> _logMessages = [];

    public CommandExecutionContext(string correlationId, string contextJson, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        ArgumentNullException.ThrowIfNull(contextJson);

        CorrelationId = correlationId;
        ContextJson = contextJson;
        CancellationToken = cancellationToken;
    }

    public string CorrelationId { get; }

    public string ContextJson { get; }

    public CancellationToken CancellationToken { get; }

    public IReadOnlyList<string> LogMessages => _logMessages;

    public void WriteLog(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        _logMessages.Add(message);
    }
}
