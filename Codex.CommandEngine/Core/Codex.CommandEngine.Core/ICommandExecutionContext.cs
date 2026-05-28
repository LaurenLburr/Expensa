namespace Codex.CommandEngine.Core;

public interface ICommandExecutionContext
{
    string CorrelationId { get; }

    string ContextJson { get; }

    CancellationToken CancellationToken { get; }

    void WriteLog(string message);
}
