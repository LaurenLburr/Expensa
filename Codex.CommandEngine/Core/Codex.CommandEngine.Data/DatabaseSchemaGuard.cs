using System.Data;

namespace Codex.CommandEngine.Data;

public static class DatabaseSchemaGuard
{
    public static void RequireTablesAndColumns(
        CommandEngineConnectionFactory connectionFactory,
        IReadOnlyDictionary<string, IReadOnlyList<string>> requiredSchema)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        ArgumentNullException.ThrowIfNull(requiredSchema);

        DataTableQueryExecutor executor = new(connectionFactory);

        foreach (KeyValuePair<string, IReadOnlyList<string>> tableRequirement in requiredSchema)
        {
            string tableName = tableRequirement.Key;

            if (!TableExists(executor, tableName))
            {
                throw new DatabaseSchemaException(
                    $"Required database table '{tableName}' was not found. Rebuild the dev database from the committed schema script.");
            }

            HashSet<string> actualColumns =
                GetColumns(executor, tableName);

            foreach (string columnName in tableRequirement.Value)
            {
                if (!actualColumns.Contains(columnName))
                {
                    throw new DatabaseSchemaException(
                        $"Required database column '{tableName}.{columnName}' was not found. Rebuild the dev database from the committed schema script.");
                }
            }
        }
    }

    private static bool TableExists(
        DataTableQueryExecutor executor,
        string tableName)
    {
        DataTable table =
            executor.ExecuteQuery(
                """
                SELECT name
                FROM sqlite_master
                WHERE type = 'table'
                  AND name = $TableName;
                """,
                new Dictionary<string, object?>
                {
                    ["TableName"] = tableName
                });

        return table.Rows.Count > 0;
    }

    private static HashSet<string> GetColumns(
        DataTableQueryExecutor executor,
        string tableName)
    {
        string safeTableName =
            tableName.Replace("'", "''", StringComparison.Ordinal);

        DataTable table =
            executor.ExecuteQuery(
                $"SELECT name FROM pragma_table_info('{safeTableName}');");

        HashSet<string> columns =
            new(StringComparer.OrdinalIgnoreCase);

        foreach (DataRow row in table.Rows)
        {
            columns.Add(row.GetRequiredString("name"));
        }

        return columns;
    }
}
