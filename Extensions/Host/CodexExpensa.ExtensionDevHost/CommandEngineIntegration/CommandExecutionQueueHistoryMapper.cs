namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionQueueHistoryMapper
{
    public static CommandExecutionHistoryRecord ToHistoryRecord(
        CommandExecutionQueueItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new CommandExecutionHistoryRecord
        {
            StartedUtc = item.StartedUtc ?? item.CreatedUtc,
            CompletedUtc = item.CompletedUtc ?? DateTimeOffset.UtcNow,
            CommandName = item.CommandName,
            CorrelationId = item.CorrelationId,
            Status = item.Status.ToString(),
            Message = item.Message,
            ParameterJson = string.IsNullOrWhiteSpace(item.ParameterJson) ? "{}" : item.ParameterJson,
            OutputJson = item.OutputJson
        };
    }

    public static bool ShouldRecordToHistory(
        CommandExecutionQueueItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return item.Status is
            CommandExecutionQueueStatus.Completed or
            CommandExecutionQueueStatus.Failed or
            CommandExecutionQueueStatus.Cancelled;
    }
}
