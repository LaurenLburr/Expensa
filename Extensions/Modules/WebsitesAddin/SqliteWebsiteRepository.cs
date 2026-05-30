using System.Data;
using Microsoft.Data.Sqlite;

namespace WebsitesAddin;

public sealed class SqliteWebsiteRepository : IWebsiteRepository
{
    private readonly string _databasePath;
    private readonly WebsiteSqlQueryCatalog _queryCatalog;

    public SqliteWebsiteRepository(
        WebsiteDatabaseOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.DatabasePath);

        _databasePath = options.DatabasePath;
        _queryCatalog = new WebsiteSqlQueryCatalog(options);
    }

    public IReadOnlyList<WebsiteTreeNode> LoadWebsites(
        WebsiteLoadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        string queryName =
            GetQueryName(request);

        string sqlText =
            _queryCatalog.GetSqlText(queryName);

        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = sqlText;

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            command.Parameters.AddWithValue(
                "@SearchText",
                $"%{request.SearchText.Trim()}%");
        }

        DataTable table = new();

        using SqliteDataReader reader = command.ExecuteReader();

        table.Load(reader);

        return BuildTreeNodes(table, request.MaximumRows);
    }

    private static string GetQueryName(
        WebsiteLoadRequest request)
    {
        bool hasSearch =
            !string.IsNullOrWhiteSpace(request.SearchText);

        if (hasSearch && request.IncludeDisabled)
        {
            return "Website.Select.Search.All";
        }

        if (hasSearch)
        {
            return "Website.Select.Search.Enabled";
        }

        if (request.IncludeDisabled)
        {
            return "Website.Select.All";
        }

        return "Website.Select.Enabled";
    }

    private static IReadOnlyList<WebsiteTreeNode> BuildTreeNodes(
        DataTable table,
        int maximumRows)
    {
        int rowLimit =
            Math.Max(1, maximumRows);

        List<WebsiteTreeNode> websiteNodes =
            table.Rows
                .Cast<DataRow>()
                .Take(rowLimit)
                .Select(static row => new WebsiteTreeNode
                {
                    NodeId = Convert.ToString(row["WebsiteId"]) ?? string.Empty,
                    DisplayText = Convert.ToString(row["DisplayName"]) ?? string.Empty,
                    Url = Convert.ToString(row["Url"]) ?? string.Empty,
                    Category = Convert.ToString(row["Category"]) ?? string.Empty,
                    IsEnabled = Convert.ToInt32(row["IsEnabled"]) == 1
                })
                .ToList();

        return websiteNodes
            .GroupBy(static node => string.IsNullOrWhiteSpace(node.Category) ? "Websites" : node.Category)
            .OrderBy(static group => group.Key)
            .Select(static group => new WebsiteTreeNode
            {
                NodeId = $"category.{group.Key}",
                DisplayText = group.Key,
                Category = "Root",
                Children = group
                    .OrderBy(static node => node.DisplayText)
                    .ToList()
            })
            .ToList();
    }

    private SqliteConnection OpenConnection()
    {
        SqliteConnection connection = new($"Data Source={_databasePath}");
        connection.Open();
        return connection;
    }
}
