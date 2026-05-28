namespace Codex.CommandEngine.Core;

public sealed class CommandExecutionResult
{
    public required string CommandName { get; init; }

    public required string CorrelationId { get; init; }

    public CommandExecutionStatus Status { get; init; }

    public string Message { get; init; } = string.Empty;

    public string OutputJson { get; init; } = "{}";

    public Exception? Exception { get; init; }

    public static CommandExecutionResult Succeeded(string commandName, string correlationId, string message = "", string outputJson = "{}")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);

        return new CommandExecutionResult
        {
            CommandName = commandName,
            CorrelationId = correlationId,
            Status = CommandExecutionStatus.Succeeded,
            Message = message,
            OutputJson = outputJson
        };
    }

    public static CommandExecutionResult Failed(string commandName, string correlationId, string message, Exception? exception = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return new CommandExecutionResult
        {
            CommandName = commandName,
            CorrelationId = correlationId,
            Status = CommandExecutionStatus.Failed,
            Message = message,
            Exception = exception
        };
    }

    public static CommandExecutionResult Cancelled(string commandName, string correlationId, string message = "Command execution was cancelled.")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);

        return new CommandExecutionResult
        {
            CommandName = commandName,
            CorrelationId = correlationId,
            Status = CommandExecutionStatus.Cancelled,
            Message = message
        };
    }
}
