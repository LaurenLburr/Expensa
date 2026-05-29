using System.Text;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionPersistentRecordQueryBuilder
{
    public static CommandExecutionPersistentRecordQuerySql Build(
        CommandExecutionPersistentRecordQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);

        Dictionary<string, object> parameters = [];
        StringBuilder builder = new();

        builder.AppendLine("""
        SELECT
            ExecutionId,
            SourceKind,
            CommandName,
            ParameterJson,
            Status,
            Message,
            CorrelationId,
            OutputJson,
            CreatedUtc,
            StartedUtc,
            CompletedUtc,
            IsHistory,
            IsQueueItem
        FROM CommandExecution
        WHERE 1 = 1
        """);

        if (query.SourceFilter == CommandExecutionPersistentRecordSourceFilter.Queue)
        {
            builder.AppendLine("  AND IsQueueItem = 1");
        }

        if (query.SourceFilter == CommandExecutionPersistentRecordSourceFilter.History)
        {
            builder.AppendLine("  AND IsHistory = 1");
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            builder.AppendLine("  AND Status = @Status");
            parameters["@Status"] = query.Status;
        }

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            builder.AppendLine("""
              AND (
                    CommandName LIKE @SearchText
                 OR CorrelationId LIKE @SearchText
                 OR Message LIKE @SearchText
                 OR SourceKind LIKE @SearchText
              )
            """);

            parameters["@SearchText"] = $"%{query.SearchText.Trim()}%";
        }

        if (query.CreatedFromUtc is not null)
        {
            builder.AppendLine("  AND CreatedUtc >= @CreatedFromUtc");
            parameters["@CreatedFromUtc"] = query.CreatedFromUtc.Value.ToString("O");
        }

        if (query.CreatedToUtc is not null)
        {
            builder.AppendLine("  AND CreatedUtc <= @CreatedToUtc");
            parameters["@CreatedToUtc"] = query.CreatedToUtc.Value.ToString("O");
        }

        builder.AppendLine(GetOrderBy(query.SortMode));
        builder.AppendLine("LIMIT @MaximumRows;");
        parameters["@MaximumRows"] = Math.Max(1, query.MaximumRows);

        return new CommandExecutionPersistentRecordQuerySql
        {
            SqlText = builder.ToString(),
            Parameters = parameters
        };
    }

    private static string GetOrderBy(
        CommandExecutionPersistentRecordSortMode sortMode)
    {
        return sortMode switch
        {
            CommandExecutionPersistentRecordSortMode.OldestFirst =>
                "ORDER BY CreatedUtc ASC",

            CommandExecutionPersistentRecordSortMode.FailedFirst =>
                """
                ORDER BY
                    CASE WHEN Status = 'Failed' THEN 0 ELSE 1 END,
                    CreatedUtc DESC
                """,

            CommandExecutionPersistentRecordSortMode.CommandName =>
                "ORDER BY CommandName ASC, CreatedUtc DESC",

            _ =>
                "ORDER BY CreatedUtc DESC"
        };
    }
}
