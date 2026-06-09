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

        using SqliteConnection connection = new($"Data Source={request.DatabasePath};Mode=ReadOnly");
        connection.Open();

        string? tableName = FindPayeeTableName(connection);

        if (string.IsNullOrWhiteSpace(tableName))
        {
            return CreateEmptyResult("No Payee or Payees table was found.");
        }

        DataTable rows = LoadPayeeRows(connection, tableName, request);
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

    private static DataTable LoadPayeeRows(SqliteConnection connection, string tableName, PayeeLoadRequest request)
    {
        HashSet<string> columns = GetColumns(connection, tableName);

        string idColumn = PickColumn(columns, "PayeeId", "Id", tableName + "Id");
        string nameColumn = PickColumn(columns, "PayeeName", "Name", "DisplayName");
        string activeExpression = columns.Contains("IsActive") ? "[IsActive]" : "1";

        string whereClause =
            string.IsNullOrWhiteSpace(request.SearchText)
                ? string.Empty
                : $"WHERE [{nameColumn}] LIKE @SearchText";

        if (!request.IncludeInactive && columns.Contains("IsActive"))
        {
            whereClause = string.IsNullOrWhiteSpace(whereClause)
                ? "WHERE [IsActive] <> 0"
                : whereClause + " AND [IsActive] <> 0";
        }

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            $"""
            SELECT
                [{idColumn}] AS [PayeeId],
                [{nameColumn}] AS [PayeeName],
                {activeExpression} AS [IsActive]
            FROM [{tableName}]
            {whereClause}
            ORDER BY [{nameColumn}]
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
        List<PayeeTreeNode> payeeNodes = [];

        foreach (DataRow row in rows.Rows)
        {
            string payeeId = Convert.ToString(row["PayeeId"], CultureInfo.InvariantCulture) ?? string.Empty;
            string payeeName = Convert.ToString(row["PayeeName"], CultureInfo.InvariantCulture) ?? string.Empty;

            bool isActive =
                row["IsActive"] is not DBNull &&
                Convert.ToBoolean(row["IsActive"], CultureInfo.InvariantCulture);

            if (string.IsNullOrWhiteSpace(payeeName))
            {
                payeeName = payeeId;
            }

            payeeNodes.Add(
                new PayeeTreeNode
                {
                    NodeId = "payee:" + payeeId,
                    DisplayText = payeeName,
                    NodeType = PayeeTreeNodeTypes.Payee,
                    PayeeId = payeeId,
                    PayeeName = payeeName,
                    IsActive = isActive,
                    Children = []
                });
        }

        return
        [
            new PayeeTreeNode
            {
                NodeId = "payees",
                DisplayText = "Payees",
                NodeType = PayeeTreeNodeTypes.Root,
                Children =
                [
                    new PayeeTreeNode
                    {
                        NodeId = "payees.by-name",
                        DisplayText = "By Name",
                        NodeType = PayeeTreeNodeTypes.Group,
                        Children = payeeNodes
                    }
                ]
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

        while (reader.Read())
        {
            columns.Add(reader.GetString(0));
        }

        return columns;
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
