using System.Data;
using System.Data.Common;
using Codex.Data.SQLiteEngine;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Data.Sqlite.SqlQueries;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.Data.Sqlite.Db;

/// <summary>
/// SQLite database session used by the app.
/// Supports:
/// - File-backed DB (direct)
/// - In-memory DB seeded from a file DB, with Save() flushing memory back to disk.
/// </summary>
public sealed class SqliteDatabase : IDatabaseSession, IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SqliteEngine _engine;
    private readonly SqliteQueryCatalog _queryCatalog;
    private readonly string? _persistedFilePath;
    private bool _disposed;

    private SqliteDatabase(SqliteConnection connection, string? persistedFilePath)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _persistedFilePath = persistedFilePath;
        _queryCatalog = new SqliteQueryCatalog(_connection);
        _engine = new SqliteEngine(_connection);
    }

    /// <summary>
    /// Opens a file-backed SQLite database and keeps a single open connection for the lifetime of this instance.
    /// </summary>
    public static SqliteDatabase OpenFile(string dbPath)
    {
        if (string.IsNullOrWhiteSpace(dbPath))
            throw new ArgumentException("dbPath is required.", nameof(dbPath));

        string? folder = Path.GetDirectoryName(dbPath);
        if (string.IsNullOrWhiteSpace(folder))
            throw new ArgumentException("dbPath must include a directory.", nameof(dbPath));

        Directory.CreateDirectory(folder);

        SqliteConnectionStringBuilder builder = new()
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate
        };

        SqliteConnection conn = new(builder.ToString());
        conn.Open();

        return new SqliteDatabase(conn, persistedFilePath: null);
    }

    /// <summary>
    /// Opens an in-memory database seeded from the file database at dbPath (created if missing).
    /// Save() flushes the in-memory DB back to that file.
    /// </summary>
    public static SqliteDatabase OpenMemorySeededFromFile(string dbPath)
    {
        if (string.IsNullOrWhiteSpace(dbPath))
            throw new ArgumentException("dbPath is required.", nameof(dbPath));

        using SqliteDatabase fileDb = OpenFile(dbPath);

        SqliteConnectionStringBuilder memBuilder = new()
        {
            DataSource = ":memory:",
            Mode = SqliteOpenMode.Memory,
            Cache = SqliteCacheMode.Shared
        };

        SqliteConnection memConn = new(memBuilder.ToString());
        memConn.Open();

        fileDb._connection.BackupDatabase(memConn);

        return new SqliteDatabase(memConn, persistedFilePath: dbPath);
    }

    public void Save()
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(_persistedFilePath))
            return;

        using SqliteDatabase fileDb = OpenFile(_persistedFilePath);
        _connection.BackupDatabase(fileDb._connection);
    }

    public bool IsInMemory => !string.IsNullOrWhiteSpace(_persistedFilePath);

    public string? PersistedFilePath => _persistedFilePath;

    public int ExecuteNonQuery(
        string sql,
        IEnumerable<SqliteParameter>? parameters = null,
        SqliteTransaction? tx = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("sql is required.", nameof(sql));

        IReadOnlyList<SqliteParameter> sqliteParameters = MaterializeSqliteParameters(parameters);

        if (tx is not null)
        {
            using SqliteCommand cmd = tx.Connection!.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;

            foreach (SqliteParameter p in sqliteParameters)
                cmd.Parameters.Add(p);

            return cmd.ExecuteNonQuery();
        }

        return _engine.ExecuteNonQuery(sql, sqliteParameters);
    }

    public void Execute(
        string queryName,
        IEnumerable<DbParameter>? parameters = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(queryName))
            throw new ArgumentException("queryName is required.", nameof(queryName));

        string sql = _queryCatalog.GetSql(queryName);
        IReadOnlyList<SqliteParameter> sqliteParameters = MaterializeSqliteParameters(parameters);

        _engine.ExecuteNonQuery(sql, sqliteParameters, queryName);
    }

    public int ExecuteNamedNonQuery(
        string queryName,
        IEnumerable<SqliteParameter>? parameters = null,
        SqliteTransaction? tx = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(queryName))
            throw new ArgumentException("queryName is required.", nameof(queryName));

        string sql = _queryCatalog.GetSql(queryName);

        return ExecuteNonQuery(sql, parameters, tx);
    }

    public IReadOnlyList<T> Query<T>(
        string sql,
        Func<SqliteDataReader, T> map,
        IEnumerable<SqliteParameter>? parameters = null,
        SqliteTransaction? tx = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("sql is required.", nameof(sql));
        if (map is null)
            throw new ArgumentNullException(nameof(map));

        IReadOnlyList<SqliteParameter> sqliteParameters = MaterializeSqliteParameters(parameters);

        if (tx is not null)
        {
            List<T> results = new();

            using SqliteCommand cmd = tx.Connection!.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;

            foreach (SqliteParameter p in sqliteParameters)
                cmd.Parameters.Add(p);

            using SqliteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                results.Add(map(reader));

            return results;
        }

        return _engine.Query(sql, map, sqliteParameters);
    }

    public IReadOnlyList<T> QueryNamed<T>(
        string queryName,
        Func<SqliteDataReader, T> map,
        IEnumerable<SqliteParameter>? parameters = null,
        SqliteTransaction? tx = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(queryName))
            throw new ArgumentException("queryName is required.", nameof(queryName));
        if (map is null)
            throw new ArgumentNullException(nameof(map));

        string sql = _queryCatalog.GetSql(queryName);
        IReadOnlyList<SqliteParameter> sqliteParameters = MaterializeSqliteParameters(parameters);

        if (tx is not null)
            return Query(sql, map, sqliteParameters, tx);

        return _engine.Query(sql, map, sqliteParameters, queryName);
    }

    public DataTable QueryDataTable(
        string queryName,
        IEnumerable<DbParameter>? parameters = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(queryName))
            throw new ArgumentException("queryName is required.", nameof(queryName));

        string sql = _queryCatalog.GetSql(queryName);
        IReadOnlyList<SqliteParameter> sqliteParameters = MaterializeSqliteParameters(parameters);

        return _engine.QueryDataTable(sql, sqliteParameters, queryName);
    }

    public void ExecuteInTransaction(Action<SqliteTransaction> action)
    {
        ThrowIfDisposed();

        if (action is null)
            throw new ArgumentNullException(nameof(action));

        using SqliteTransaction tx = _connection.BeginTransaction();
        try
        {
            action(tx);
            tx.Commit();
        }
        catch
        {
            try
            {
                tx.Rollback();
            }
            catch
            {
            }

            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _engine.Dispose();
        _connection.Dispose();
    }

    private static IReadOnlyList<SqliteParameter> MaterializeSqliteParameters(IEnumerable<DbParameter>? parameters)
    {
        if (parameters is null)
            return [];

        List<SqliteParameter> list = new();

        foreach (DbParameter parameter in parameters)
        {
            if (parameter is SqliteParameter sqliteParameter)
            {
                list.Add(sqliteParameter);
                continue;
            }

            throw new InvalidOperationException(
                $"Expected parameter type {nameof(SqliteParameter)} but received {parameter.GetType().FullName}.");
        }

        return list;
    }

    private static IReadOnlyList<SqliteParameter> MaterializeSqliteParameters(IEnumerable<SqliteParameter>? parameters)
    {
        if (parameters is null)
            return [];

        return parameters.ToList();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SqliteDatabase));
    }
}
