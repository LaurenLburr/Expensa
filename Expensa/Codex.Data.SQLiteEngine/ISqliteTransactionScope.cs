using Microsoft.Data.Sqlite;
using System.Data;

namespace Codex.Data.SQLiteEngine;

public interface ISqliteTransactionScope : IDisposable
{
    Guid TransactionId { get; }

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

    void Commit();

    void Rollback();
}
