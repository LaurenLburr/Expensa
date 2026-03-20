using Microsoft.Data.Sqlite;
using System.Data;

namespace Codex.Data.SQLiteEngine;

public interface ISqliteEngine
{
    event EventHandler<SqliteCommandEventArgs>? CommandExecuting;

    event EventHandler<SqliteCommandEventArgs>? CommandExecuted;

    event EventHandler<SqliteCommandEventArgs>? CommandFailed;

    event EventHandler<SqliteTransactionEventArgs>? TransactionBegan;

    event EventHandler<SqliteTransactionEventArgs>? TransactionCommitted;

    event EventHandler<SqliteTransactionEventArgs>? TransactionRolledBack;

    event EventHandler<SqliteTransactionEventArgs>? TransactionFailed;

    int ExecuteNonQuery(string sql, IReadOnlyList<SqliteParameter>? parameters = null, string? queryName = null);

    T? ExecuteScalar<T>(string sql, IReadOnlyList<SqliteParameter>? parameters = null, string? queryName = null);

    IReadOnlyList<T> Query<T>(
        string sql,
        Func<SqliteDataReader, T> map,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null);

    DataTable QueryDataTable(
        string sql,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null);

    ISqliteTransactionScope BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable);
}
