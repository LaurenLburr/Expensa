using System.Data;
using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class SqlCatalogRepository
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> RequiredSchema =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["SqlQuery"] =
            [
                "QueryName",
                "Description",
                "SqlText",
                "Category",
                "IsActive"
            ]
        };

    private readonly CommandEngineConnectionFactory _connectionFactory;

    public SqlCatalogRepository(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        _connectionFactory = connectionFactory;
        EnsureSchema();
    }

    public IReadOnlyList<SqlCatalogRecord> ListAll()
    {
        EnsureSchema();

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                QueryName,
                Description,
                SqlText,
                Category,
                IsActive
            FROM SqlQuery
            ORDER BY Category, QueryName;
            """;

        DataTable table = SqliteDataTableLoader.Load(command);

        List<SqlCatalogRecord> records = [];

        foreach (DataRow row in table.Rows)
        {
            records.Add(new SqlCatalogRecord(
                row.GetRequiredString("QueryName"),
                row.GetStringOrDefault("Description"),
                row.GetStringOrDefault("SqlText"),
                row.GetStringOrDefault("Category"),
                row.GetBooleanOrDefault("IsActive", true)));
        }

        return records;
    }

    private void EnsureSchema()
    {
        DatabaseSchemaGuard.RequireTablesAndColumns(_connectionFactory, RequiredSchema);
    }
}
