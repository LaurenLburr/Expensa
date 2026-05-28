namespace Codex.CommandEngine.Abstractions;

public sealed class CommandExecutionResult
{
    public required bool Succeeded { get; init; }

    public required string Message { get; init; }

    public static CommandExecutionResult Success(string message)
    {
        return new CommandExecutionResult
        {
            Succeeded = true,
            Message = message
        };
    }

    public static CommandExecutionResult Failure(string message)
    {
        return new CommandExecutionResult
        {
            Succeeded = false,
            Message = message
        };
    }
}
