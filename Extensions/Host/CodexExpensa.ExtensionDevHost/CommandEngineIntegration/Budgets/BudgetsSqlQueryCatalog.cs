using Microsoft.Data.Sqlite;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

/// <summary>
/// Resolves named SQL statements from the SqlQuery catalog in the already-open
/// Budgets in-memory database. The only SQL held here is the catalog bootstrap
/// lookup needed to retrieve all other named SQL.
/// </summary>
internal sealed class BudgetsSqlQueryCatalog
{
    private readonly SqliteConnection _connection;

    public BudgetsSqlQueryCatalog(SqliteConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public string GetRequiredSqlText(string queryName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queryName);

        using SqliteCommand command = _connection.CreateCommand();
        command.CommandText =
            "SELECT [SqlText] " +
            "FROM [SqlQuery] " +
            "WHERE [QueryName] = @QueryName " +
            "  AND [IsActive] = 1 " +
            "LIMIT 1;";
        command.Parameters.AddWithValue("@QueryName", queryName);

        object? value = command.ExecuteScalar();
        string sql = Convert.ToString(value) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(sql))
        {
            throw new InvalidOperationException(
                $"Required SQL catalog query was not found or is inactive: {queryName}");
        }

        return sql;
    }
}
