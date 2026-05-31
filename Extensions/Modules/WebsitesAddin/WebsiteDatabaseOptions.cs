using Microsoft.Data.Sqlite;

namespace WebsitesAddin;

public sealed class WebsiteDatabaseOptions
{
    public string DatabasePath { get; init; } = string.Empty;

    public SqliteConnection? Connection { get; init; }

    public bool OwnsConnection { get; init; }
}
