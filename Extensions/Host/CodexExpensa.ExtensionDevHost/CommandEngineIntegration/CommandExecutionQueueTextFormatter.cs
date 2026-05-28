using System.Text;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionQueueTextFormatter
{
    public static string Format(
        CommandExecutionQueueSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        StringBuilder builder = new();

        builder.AppendLine("Command Execution Queue");
        builder.AppendLine("=======================");
        builder.AppendLine();
        builder.AppendLine($"Pending: {snapshot.PendingCount}");
        builder.AppendLine($"Running: {snapshot.RunningCount}");
        builder.AppendLine($"Completed: {snapshot.CompletedCount}");
        builder.AppendLine($"Failed: {snapshot.FailedCount}");
        builder.AppendLine($"Cancelled: {snapshot.CancelledCount}");

        foreach (CommandExecutionQueueItem item in snapshot.Items)
        {
            builder.AppendLine();
            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine($"QueueItemId: {item.QueueItemId}");
            builder.AppendLine($"CommandName: {item.CommandName}");
            builder.AppendLine($"Status: {item.Status}");
            builder.AppendLine($"CreatedUtc: {item.CreatedUtc:u}");
            builder.AppendLine($"StartedUtc: {item.StartedUtc:u}");
            builder.AppendLine($"CompletedUtc: {item.CompletedUtc:u}");
            builder.AppendLine($"DurationMs: {(item.Duration?.TotalMilliseconds.ToString("0") ?? string.Empty)}");
            builder.AppendLine($"CorrelationId: {item.CorrelationId}");
            builder.AppendLine($"Message: {item.Message}");

            if (!string.IsNullOrWhiteSpace(item.ParameterJson))
            {
                builder.AppendLine();
                builder.AppendLine("ParameterJson");
                builder.AppendLine("-------------");
                builder.AppendLine(item.ParameterJson);
            }

            if (!string.IsNullOrWhiteSpace(item.OutputJson))
            {
                builder.AppendLine();
                builder.AppendLine("OutputJson");
                builder.AppendLine("----------");
                builder.AppendLine(item.OutputJson);
            }
        }

        return builder.ToString();
    }
}
