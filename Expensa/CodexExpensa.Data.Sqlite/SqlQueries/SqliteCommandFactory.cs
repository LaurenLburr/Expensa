using System;
using CodexExpensa.Core.Abstractions;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.Data.Sqlite.SqlQueries;

public sealed class SqliteCommandFactory : ISqliteCommandFactory
{
    private readonly SqliteConnection _connection;
    private readonly ISqlQueryProvider _sqlQueryProvider;

    public SqliteCommandFactory(
        SqliteConnection connection,
        ISqlQueryProvider sqlQueryProvider)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _sqlQueryProvider = sqlQueryProvider ?? throw new ArgumentNullException(nameof(sqlQueryProvider));
    }

    public SqliteCommand Create(string queryName, SqliteTransaction? transaction = null)
    {
        if (string.IsNullOrWhiteSpace(queryName))
        {
            throw new ArgumentException("Query name is required.", nameof(queryName));
        }

        string sql = _sqlQueryProvider.GetSql(queryName);

        SqliteCommand command = _connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;

        return command;
    }
}