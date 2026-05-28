using System.Text;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionHistoryTextFormatter
{
    public static string Format(
        IReadOnlyList<CommandExecutionHistoryRecord> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        StringBuilder builder = new();

        builder.AppendLine("Command Execution History");
        builder.AppendLine("=========================");
        builder.AppendLine();

        if (records.Count == 0)
        {
            builder.AppendLine("No command executions have been recorded.");
            return builder.ToString();
        }

        foreach (CommandExecutionHistoryRecord record in records)
        {
            builder.AppendLine($"StartedUtc: {record.StartedUtc:u}");
            builder.AppendLine($"CompletedUtc: {record.CompletedUtc:u}");
            builder.AppendLine($"DurationMs: {record.Duration.TotalMilliseconds:0}");
            builder.AppendLine($"CommandName: {record.CommandName}");
            builder.AppendLine($"CorrelationId: {record.CorrelationId}");
            builder.AppendLine($"Status: {record.Status}");
            builder.AppendLine($"Message: {record.Message}");

            if (!string.IsNullOrWhiteSpace(record.ParameterJson))
            {
                builder.AppendLine("ParameterJson:");
                builder.AppendLine(record.ParameterJson);
            }

            if (!string.IsNullOrWhiteSpace(record.OutputJson))
            {
                builder.AppendLine("OutputJson:");
                builder.AppendLine(record.OutputJson);
            }

            builder.AppendLine();
            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine();
        }

        return builder.ToString();
    }
}
