namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionQueueRecoveryMapper
{
    public static CommandExecutionQueueItem ToRecoveredQueueItem(
        CommandExecutionPersistentRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        CommandExecutionQueueStatus status =
            ParseStatus(record.Status);

        if (status is CommandExecutionQueueStatus.Pending or CommandExecutionQueueStatus.Running)
        {
            status = CommandExecutionQueueStatus.Failed;
        }

        return new CommandExecutionQueueItem
        {
            QueueItemId = TryParseQueueItemId(record.ExecutionId),
            CreatedUtc = record.CreatedUtc,
            StartedUtc = record.StartedUtc,
            CompletedUtc = record.CompletedUtc ?? DateTimeOffset.UtcNow,
            CommandName = record.CommandName,
            ParameterJson = string.IsNullOrWhiteSpace(record.ParameterJson) ? "{}" : record.ParameterJson,
            Status = status,
            Message = BuildRecoveryMessage(record, status),
            CorrelationId = record.CorrelationId,
            OutputJson = record.OutputJson
        };
    }

    public static IReadOnlyList<CommandExecutionQueueItem> ToRecoveredQueueItems(
        IReadOnlyList<CommandExecutionPersistentRecord> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        return records
            .Where(static record => record.IsQueueItem == 1)
            .Select(ToRecoveredQueueItem)
            .ToList();
    }

    private static Guid TryParseQueueItemId(
        string executionId)
    {
        return Guid.TryParse(executionId, out Guid parsed)
            ? parsed
            : Guid.NewGuid();
    }

    private static CommandExecutionQueueStatus ParseStatus(
        string status)
    {
        return Enum.TryParse(status, ignoreCase: true, out CommandExecutionQueueStatus parsed)
            ? parsed
            : CommandExecutionQueueStatus.Failed;
    }

    private static string BuildRecoveryMessage(
        CommandExecutionPersistentRecord record,
        CommandExecutionQueueStatus recoveredStatus)
    {
        if (record.Status is "Pending" or "Running")
        {
            return "Recovered from previous app session. Item was not completed before shutdown.";
        }

        if (!string.IsNullOrWhiteSpace(record.Message))
        {
            return record.Message;
        }

        return recoveredStatus.ToString();
    }
}
