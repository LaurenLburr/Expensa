using System.Text;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionRetentionSqlBuilder
{
    public static CommandExecutionPersistentRecordQuerySql BuildDeleteSql(
        CommandExecutionRetentionOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        Dictionary<string, object> parameters = [];
        StringBuilder builder = new();

        builder.AppendLine("DELETE FROM CommandExecution");
        builder.AppendLine("WHERE 1 = 1");

        if (options.SourceFilter == CommandExecutionPersistentRecordSourceFilter.Queue)
        {
            builder.AppendLine("  AND IsQueueItem = 1");
        }

        if (options.SourceFilter == CommandExecutionPersistentRecordSourceFilter.History)
        {
            builder.AppendLine("  AND IsHistory = 1");
        }

        if (!string.IsNullOrWhiteSpace(options.Status))
        {
            builder.AppendLine("  AND Status = @Status");
            parameters["@Status"] = options.Status;
        }

        if (options.CreatedBeforeUtc is not null)
        {
            builder.AppendLine("  AND CreatedUtc < @CreatedBeforeUtc");
            parameters["@CreatedBeforeUtc"] = options.CreatedBeforeUtc.Value.ToString("O");
        }

        builder.AppendLine(";");

        return new CommandExecutionPersistentRecordQuerySql
        {
            SqlText = builder.ToString(),
            Parameters = parameters
        };
    }
}
