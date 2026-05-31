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

        DataTable table = ExecuteToDataTable(command);

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

    private static DataTable ExecuteToDataTable(SqliteCommand command)
    {
        using SqliteDataReader reader = command.ExecuteReader();

        DataTable table = new();

        table.Load(reader);

        return table;
    }

    private static string GetSqlText(bool includeDisabled, bool hasSearch)
    {
        string whereClause = includeDisabled ? string.Empty : "WHERE [IsEnabled] = 1";

        string searchPrefix = hasSearch
            ? includeDisabled ? "WHERE" : "AND"
            : string.Empty;

        string searchClause = hasSearch
            ? $" {searchPrefix} ([DisplayName] LIKE @SearchText OR [Url] LIKE @SearchText OR [Category] LIKE @SearchText)"
            : string.Empty;

        return $"""
SELECT
    [WebsiteId],
    [DisplayName],
    [Url],
    [Category],
    [IsEnabled],
    [SortOrder]
FROM [Website]
{whereClause}
{searchClause}
ORDER BY
    [Category],
    [SortOrder],
    [DisplayName]
LIMIT @MaximumRows;
""";
    }

    private static IReadOnlyList<WebsiteTreeNode> BuildTreeNodes(DataTable table)
    {
        Dictionary<string, List<WebsiteTreeNode>> childrenByCategory =
            new(StringComparer.OrdinalIgnoreCase);

        foreach (DataRow row in table.Rows)
        {
            string category = Convert.ToString(row["Category"]) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(category))
            {
                category = "Uncategorized";
            }

            WebsiteTreeNode websiteNode = new()
            {
                NodeId = Convert.ToString(row["WebsiteId"]) ?? Guid.NewGuid().ToString("N"),
                DisplayText = Convert.ToString(row["DisplayName"]) ?? string.Empty,
                Url = Convert.ToString(row["Url"]) ?? string.Empty,
                Category = category,
                IsEnabled = Convert.ToInt32(row["IsEnabled"]) != 0
            };

            if (!childrenByCategory.TryGetValue(category, out List<WebsiteTreeNode>? children))
            {
                children = [];
                childrenByCategory[category] = children;
            }

            children.Add(websiteNode);
        }

        return childrenByCategory
            .OrderBy(static pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(static pair => new WebsiteTreeNode
            {
                NodeId = $"category:{pair.Key}",
                DisplayText = pair.Key,
                Category = pair.Key,
                Children = pair.Value
            })
            .ToList();
    }
}
