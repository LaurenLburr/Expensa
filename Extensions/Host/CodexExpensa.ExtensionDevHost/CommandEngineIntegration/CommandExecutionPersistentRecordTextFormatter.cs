using System.Text;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionPersistentRecordTextFormatter
{
    public static string Format(
        string title,
        IReadOnlyList<CommandExecutionPersistentRecord> records)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(records);

        StringBuilder builder = new();

        builder.AppendLine(title);
        builder.AppendLine(new string('=', title.Length));
        builder.AppendLine();
        builder.AppendLine($"Count: {records.Count}");

        foreach (CommandExecutionPersistentRecord record in records)
        {
            builder.AppendLine();
            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine($"ExecutionId: {record.ExecutionId}");
            builder.AppendLine($"SourceKind: {record.SourceKind}");
            builder.AppendLine($"CommandName: {record.CommandName}");
            builder.AppendLine($"Status: {record.Status}");
            builder.AppendLine($"CreatedUtc: {record.CreatedUtc:u}");
            builder.AppendLine($"StartedUtc: {record.StartedUtc:u}");
            builder.AppendLine($"CompletedUtc: {record.CompletedUtc:u}");
            builder.AppendLine($"CorrelationId: {record.CorrelationId}");
            builder.AppendLine($"Message: {record.Message}");

            if (!string.IsNullOrWhiteSpace(record.ParameterJson))
            {
                builder.AppendLine();
                builder.AppendLine("ParameterJson");
                builder.AppendLine("-------------");
                builder.AppendLine(record.ParameterJson);
            }

            if (!string.IsNullOrWhiteSpace(record.OutputJson))
            {
                builder.AppendLine();
                builder.AppendLine("OutputJson");
                builder.AppendLine("----------");
                builder.AppendLine(record.OutputJson);
            }
        }

        return builder.ToString();
    }
}
