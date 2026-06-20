using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;

namespace PayeesAddin;

public sealed class SqlitePayeeRepository
{
    public PayeeLoadResult LoadPayees(PayeeLoadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.DatabasePath))
        {
            return CreateEmptyResult("DatabasePath is required.");
        }

        if (!File.Exists(request.DatabasePath))
        {
            return CreateEmptyResult($"Database not found: {request.DatabasePath}");
        }

        using SqliteConnection connection = new($"Data Source={request.DatabasePath};Mode=ReadOnly;Pooling=False");
        connection.Open();

        string? tableName = FindPayeeTableName(connection);

        if (string.IsNullOrWhiteSpace(tableName))
        {
            return CreateEmptyResult("No Payee or Payees table was found.");
        }

        bool hasTagTables = TableExists(connection, "Tag") && TableExists(connection, "TagAssignment");

        DataTable rows = LoadPayeeRows(connection, tableName, request, hasTagTables);
        IReadOnlyList<PayeeTreeNode> nodes = BuildPayeeTree(rows);

        int totalCount = nodes
            .SelectMany(static node => node.Children)
            .SelectMany(static node => node.Children)
            .Count();

        return new PayeeLoadResult
        {
            Status = "Succeeded",
            Message = $"Loaded {totalCount} payee row(s) from {tableName}.",
            TotalCount = totalCount,
            Nodes = nodes
        };
    }

    private static PayeeLoadResult CreateEmptyResult(string message)
    {
        return new PayeeLoadResult
        {
            Status = "Succeeded",
            Message = message,
            TotalCount = 0,
            Nodes =
            [
                new PayeeTreeNode
                {
                    NodeId = "payees",
                    DisplayText = "Payees",
                    NodeType = PayeeTreeNodeTypes.Root,
                    Children = []
                }
            ]
        };
    }

    private static string? FindPayeeTableName(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT [name]
            FROM [sqlite_master]
            WHERE [type] = 'table'
              AND [name] IN ('Payee', 'Payees')
            ORDER BY
                CASE [name]
                    WHEN 'Payee' THEN 1
                    WHEN 'Payees' THEN 2
                    ELSE 3
                END
            LIMIT 1;
            """;

        return Convert.ToString(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static DataTable LoadPayeeRows(SqliteConnection connection, string tableName, PayeeLoadRequest request, bool hasTagTables)
    {
        HashSet<string> columns = GetColumns(connection, tableName);

        string idColumn = PickColumn(columns, "PayeeId", "Id", tableName + "Id");
        string nameColumn = PickColumn(columns, "PayeeName", "Name", "DisplayName");
        string activeExpression = columns.Contains("IsActive") ? "p.[IsActive]" : "1";
        string tagSelect = hasTagTables
            ? "COALESCE(t.[TagName], 'Uncategorized') AS [TagName], COALESCE(t.[SortIndex], 999999) AS [TagSortIndex], COALESCE(ta.[SortIndex], 0) AS [AssignmentSortIndex]"
            : "'Uncategorized' AS [TagName], 999999 AS [TagSortIndex], 0 AS [AssignmentSortIndex]";
        string tagJoin = hasTagTables
            ? $"""
            LEFT JOIN [TagAssignment] ta
                ON ta.[EntityType] = 'Payee'
                AND ta.[EntityId] = p.[{idColumn}]
                AND ta.[IsActive] = 1
            LEFT JOIN [Tag] t
                ON t.[TagId] = ta.[TagId]
                AND t.[IsActive] = 1
            """
            : string.Empty;

        string whereClause =
            string.IsNullOrWhiteSpace(request.SearchText)
                ? string.Empty
                : hasTagTables
                    ? $"WHERE (p.[{nameColumn}] LIKE @SearchText OR t.[TagName] LIKE @SearchText)"
                    : $"WHERE p.[{nameColumn}] LIKE @SearchText";

        if (!request.IncludeInactive && columns.Contains("IsActive"))
        {
            whereClause = string.IsNullOrWhiteSpace(whereClause)
                ? "WHERE p.[IsActive] <> 0"
                : whereClause + " AND p.[IsActive] <> 0";
        }

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            $"""
            SELECT
                p.[{idColumn}] AS [PayeeId],
                p.[{nameColumn}] AS [PayeeName],
                {activeExpression} AS [IsActive],
                {tagSelect}
            FROM [{tableName}] p
            {tagJoin}
            {whereClause}
            ORDER BY
                [TagSortIndex],
                [TagName],
                [AssignmentSortIndex],
                p.[{nameColumn}]
            LIMIT @MaximumRows;
            """;

        command.Parameters.AddWithValue("@MaximumRows", request.MaximumRows <= 0 ? 500 : request.MaximumRows);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            command.Parameters.AddWithValue("@SearchText", "%" + request.SearchText.Trim() + "%");
        }

        using SqliteDataReader reader = command.ExecuteReader();

        DataTable table = new();
        table.Load(reader);

        return table;
    }

    private static IReadOnlyList<PayeeTreeNode> BuildPayeeTree(DataTable rows)
    {
        Dictionary<string, List<PayeeTreeNode>> childrenByTag =
            new(StringComparer.OrdinalIgnoreCase);

        foreach (DataRow row in rows.Rows)
        {
            string payeeId = Convert.ToString(row["PayeeId"], CultureInfo.InvariantCulture) ?? string.Empty;
            string payeeName = Convert.ToString(row["PayeeName"], CultureInfo.InvariantCulture) ?? string.Empty;
            string tagName = Convert.ToString(row["TagName"], CultureInfo.InvariantCulture) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tagName))
            {
                tagName = "Uncategorized";
            }

            bool isActive =
                row["IsActive"] is not DBNull &&
                Convert.ToBoolean(row["IsActive"], CultureInfo.InvariantCulture);

            if (string.IsNullOrWhiteSpace(payeeName))
            {
                payeeName = payeeId;
            }

            PayeeTreeNode payeeNode =
                new()
                {
                    NodeId = "payee:" + payeeId,
                    DisplayText = payeeName,
                    NodeType = PayeeTreeNodeTypes.Payee,
                    PayeeId = payeeId,
                    PayeeName = payeeName,
                    IsActive = isActive,
                    Children = []
                };

            if (!childrenByTag.TryGetValue(tagName, out List<PayeeTreeNode>? children))
            {
                children = [];
                childrenByTag[tagName] = children;
            }

            children.Add(payeeNode);
        }

        IReadOnlyList<PayeeTreeNode> tagNodes = childrenByTag
            .OrderBy(static pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(static pair => new PayeeTreeNode
            {
                NodeId = $"payees.tag:{pair.Key}",
                DisplayText = pair.Key,
                NodeType = PayeeTreeNodeTypes.Group,
                Children = pair.Value
            })
            .ToList();

        return
        [
            new PayeeTreeNode
            {
                NodeId = "payees",
                DisplayText = "Payees",
                NodeType = PayeeTreeNodeTypes.Root,
                Children = tagNodes
            }
        ];
    }

    private static HashSet<string> GetColumns(SqliteConnection connection, string tableName)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT [name] FROM pragma_table_info(@TableName);";
        command.Parameters.AddWithValue("@TableName", tableName);

        HashSet<string> columns = new(StringComparer.OrdinalIgnoreCase);

        using SqliteDataReader reader = command.ExecuteReader();

        DataTable table = new();
        table.Load(reader);

        foreach (DataRow row in table.Rows)
        {
            string columnName = Convert.ToString(row["name"], CultureInfo.InvariantCulture) ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(columnName))
            {
                columns.Add(columnName);
            }
        }

        return columns;
    }

    private static bool TableExists(SqliteConnection connection, string tableName)
    {
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT 1
            FROM [sqlite_master]
            WHERE [type] = 'table'
              AND [name] = @TableName
            LIMIT 1;
            """;

        command.Parameters.AddWithValue("@TableName", tableName);

        object? result = command.ExecuteScalar();
        return result is not null && result is not DBNull;
    }

    private static string PickColumn(HashSet<string> columns, params string[] candidates)
    {
        foreach (string candidate in candidates)
        {
            if (columns.Contains(candidate))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Could not find a matching column. Candidates: " + string.Join(", ", candidates));
    }
}
