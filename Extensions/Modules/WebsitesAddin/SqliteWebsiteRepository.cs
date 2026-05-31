using System.Data;
using Microsoft.Data.Sqlite;

namespace WebsitesAddin;

public sealed class SqliteWebsiteRepository : IWebsiteRepository, IDisposable
{
    private readonly WebsiteDatabaseOptions _options;
    private SqliteConnection? _ownedConnection;
    private bool _disposed;

    public SqliteWebsiteRepository(WebsiteDatabaseOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Connection is null && string.IsNullOrWhiteSpace(options.DatabasePath))
        {
            throw new ArgumentException(
                "A database path or SQLite connection is required.",
                nameof(options));
        }

        _options = options;
    }

    public IReadOnlyList<WebsiteTreeNode> LoadWebsites(WebsiteLoadRequest request)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(request);

        using SqliteCommand command = CreateCommand(request);

        DataTable table = ExecuteToUnconstrainedDataTable(command);

        return BuildTreeNodes(table);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_options.OwnsConnection)
        {
            _options.Connection?.Dispose();
            _ownedConnection?.Dispose();
        }

        _disposed = true;
    }

    private SqliteCommand CreateCommand(WebsiteLoadRequest request)
    {
        SqliteConnection connection = GetOpenConnection();

        SqliteCommand command = connection.CreateCommand();

        bool hasSearch = !string.IsNullOrWhiteSpace(request.SearchText);

        command.CommandText = GetSqlText(request.IncludeDisabled, hasSearch);

        if (hasSearch)
        {
            command.Parameters.AddWithValue(
                "@SearchText",
                $"%{request.SearchText.Trim()}%");
        }

        command.Parameters.AddWithValue(
            "@MaximumRows",
            request.MaximumRows);

        return command;
    }

    private SqliteConnection GetOpenConnection()
    {
        SqliteConnection connection;

        if (_options.Connection is not null)
        {
            connection = _options.Connection;
        }
        else
        {
            _ownedConnection ??= new SqliteConnection($"Data Source={_options.DatabasePath}");
            connection = _ownedConnection;
        }

        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        return connection;
    }

    private static DataTable ExecuteToUnconstrainedDataTable(SqliteCommand command)
    {
        using SqliteDataReader reader = command.ExecuteReader();

        DataTable table = new();

        for (int index = 0; index < reader.FieldCount; index++)
        {
            table.Columns.Add(reader.GetName(index), typeof(object));
        }

        table.BeginLoadData();

        while (reader.Read())
        {
            DataRow row = table.NewRow();

            for (int index = 0; index < reader.FieldCount; index++)
            {
                row[index] = reader.IsDBNull(index)
                    ? DBNull.Value
                    : reader.GetValue(index);
            }

            table.Rows.Add(row);
        }

        table.EndLoadData();

        return table;
    }

    private static string GetSqlText(bool includeDisabled, bool hasSearch)
    {
        string activeFilter = includeDisabled ? string.Empty : "WHERE w.[IsActive] = 1";

        string searchPrefix = hasSearch
            ? includeDisabled ? "WHERE" : "AND"
            : string.Empty;

        string searchClause = hasSearch
            ? $" {searchPrefix} (w.[Name] LIKE @SearchText OR w.[Url] LIKE @SearchText OR t.[TagName] LIKE @SearchText)"
            : string.Empty;

        return $"""
SELECT
    w.[WebsiteId],
    w.[Name],
    w.[Url],
    w.[SortIndex],
    w.[IsActive],
    t.[TagId],
    COALESCE(t.[TagName], 'Uncategorized') AS [TagName],
    COALESCE(t.[SortIndex], 999999) AS [TagSortIndex],
    COALESCE(ta.[SortIndex], 0) AS [AssignmentSortIndex]
FROM [Website] w
LEFT JOIN [TagAssignment] ta
    ON ta.[EntityType] = 'Website'
    AND ta.[EntityId] = w.[WebsiteId]
    AND ta.[IsActive] = 1
LEFT JOIN [Tag] t
    ON t.[TagId] = ta.[TagId]
    AND t.[IsActive] = 1
{activeFilter}
{searchClause}
ORDER BY
    [TagSortIndex],
    [TagName],
    [AssignmentSortIndex],
    w.[SortIndex],
    w.[Name]
LIMIT @MaximumRows;
""";
    }

    private static IReadOnlyList<WebsiteTreeNode> BuildTreeNodes(DataTable table)
    {
        Dictionary<string, List<WebsiteTreeNode>> childrenByTag =
            new(StringComparer.OrdinalIgnoreCase);

        foreach (DataRow row in table.Rows)
        {
            string tagName = Convert.ToString(row["TagName"]) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tagName))
            {
                tagName = "Uncategorized";
            }

            WebsiteTreeNode websiteNode = new()
            {
                NodeId = Convert.ToString(row["WebsiteId"]) ?? Guid.NewGuid().ToString("N"),
                DisplayText = Convert.ToString(row["Name"]) ?? string.Empty,
                Url = Convert.ToString(row["Url"]) ?? string.Empty,
                Category = tagName,
                IsEnabled = Convert.ToInt32(row["IsActive"]) != 0
            };

            if (!childrenByTag.TryGetValue(tagName, out List<WebsiteTreeNode>? children))
            {
                children = [];
                childrenByTag[tagName] = children;
            }

            children.Add(websiteNode);
        }

        return childrenByTag
            .OrderBy(static pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(static pair => new WebsiteTreeNode
            {
                NodeId = $"tag:{pair.Key}",
                DisplayText = pair.Key,
                Category = pair.Key,
                Children = pair.Value
            })
            .ToList();
    }
}
