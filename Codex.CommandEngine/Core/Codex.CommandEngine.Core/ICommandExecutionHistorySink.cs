namespace Codex.CommandEngine.Core;

public interface ICommandExecutionHistorySink
{
    void Started(CommandExecutionRequest request, string executionId, DateTimeOffset startedUtc);

    void Completed(
        CommandExecutionRequest request,
        CommandExecutionResult result,
        string executionId,
        DateTimeOffset completedUtc,
        long durationMilliseconds);
}
