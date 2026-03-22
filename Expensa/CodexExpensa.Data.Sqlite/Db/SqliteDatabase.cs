using System.Data;
using System.Data.Common;
using Codex.Data.SQLiteEngine;
using Codex.Data.SQLiteEngine.Abstractions;
using Codex.Data.SQLiteEngine.Implementations;
using CodexExpensa.Core.Abstractions;
using Microsoft.Data.Sqlite;
using EngineConfig = Codex.Data.SQLiteEngine.Configuration;

namespace CodexExpensa.Data.Sqlite.Db;

public sealed class SqliteDatabase : IDatabaseSession, IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SqliteEngine _engine;
    private readonly string? _persistedFilePath;
    private bool _disposed;

    private SqliteDatabase(
        SqliteConnection connection,
        string? persistedFilePath,
        EngineConfig.SqliteEngineOptions? options = null)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _persistedFilePath = persistedFilePath;

        ISqlCatalog? sqlCatalog = null;
        ICommandLogger? commandLogger = null;

        if (options?.SqlCatalog is not null)
            sqlCatalog = new TableSqlCatalog(_connection, options.SqlCatalog);

        if (options?.Logging is not null)
            commandLogger = new TableCommandLogger(_connection, options.Logging);

        _engine = new SqliteEngine(
            _connection,
            sqlCatalog: sqlCatalog,
            commandLogger: commandLogger,
            options: options);
    }

    public static SqliteDatabase OpenFile(
        string dbPath,
        EngineConfig.SqliteEngineOptions? options = null)
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

        return new SqliteDatabase(conn, persistedFilePath: null, options: options);
    }

    public static SqliteDatabase OpenMemorySeededFromFile(
        string dbPath,
        EngineConfig.SqliteEngineOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(dbPath))
            throw new ArgumentException("dbPath is required.", nameof(dbPath));

        using SqliteDatabase fileDb = OpenFile(dbPath, options);

        SqliteConnectionStringBuilder memBuilder = new()
        {
            DataSource = ":memory:",
            Mode = SqliteOpenMode.Memory,
            Cache = SqliteCacheMode.Shared
        };

        SqliteConnection memConn = new(memBuilder.ToString());
        memConn.Open();

        fileDb._connection.BackupDatabase(memConn);

        return new SqliteDatabase(memConn, persistedFilePath: dbPath, options: options);
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

    public int ExecuteNonQuery(string sql, IEnumerable<SqliteParameter>? parameters = null)
    {
        ThrowIfDisposed();
        return _engine.ExecuteNonQuery(sql, parameters);
    }

    public int ExecuteNonQuery(
        string sql,
        IEnumerable<SqliteParameter>? parameters,
        ISqliteTransactionScope tx)
    {
        ThrowIfDisposed();
        return tx.ExecuteNonQuery(sql, parameters is null ? null : parameters.ToList());
    }

    public void Execute(string queryName, IEnumerable<DbParameter>? parameters = null)
    {
        ThrowIfDisposed();
        IReadOnlyList<SqliteParameter> sqliteParameters = MaterializeSqliteParameters(parameters);
        _engine.ExecuteNamedNonQuery(queryName, sqliteParameters);
    }

    public int ExecuteNamedNonQuery(string queryName, IEnumerable<SqliteParameter>? parameters = null)
    {
        ThrowIfDisposed();
        return _engine.ExecuteNamedNonQuery(queryName, parameters);
    }

    public int ExecuteNamedNonQuery(
        string queryName,
        IEnumerable<SqliteParameter>? parameters,
        ISqliteTransactionScope tx)
    {
        ThrowIfDisposed();

        string sql = _engine.ResolveSql(queryName, tx);
        return tx.ExecuteNonQuery(sql, parameters is null ? null : parameters.ToList(), queryName);
    }

    public IReadOnlyList<T> Query<T>(
        string sql,
        Func<SqliteDataReader, T> map,
        IEnumerable<SqliteParameter>? parameters = null)
    {
        ThrowIfDisposed();
        return _engine.Query(sql, map, parameters);
    }

    public IReadOnlyList<T> Query<T>(
        string sql,
        Func<SqliteDataReader, T> map,
        IEnumerable<SqliteParameter>? parameters,
        ISqliteTransactionScope tx)
    {
        ThrowIfDisposed();
        return tx.Query(sql, map, parameters is null ? null : parameters.ToList());
    }

    public IReadOnlyList<T> QueryNamed<T>(
        string queryName,
        Func<SqliteDataReader, T> map,
        IEnumerable<SqliteParameter>? parameters = null)
    {
        ThrowIfDisposed();
        return _engine.QueryNamed(queryName, map, parameters);
    }

    public IReadOnlyList<T> QueryNamed<T>(
        string queryName,
        Func<SqliteDataReader, T> map,
        IEnumerable<SqliteParameter>? parameters,
        ISqliteTransactionScope tx)
    {
        ThrowIfDisposed();

        string sql = _engine.ResolveSql(queryName, tx);
        return tx.Query(sql, map, parameters is null ? null : parameters.ToList(), queryName);
    }

    public DataTable QueryDataTable(string queryName, IEnumerable<DbParameter>? parameters = null)
    {
        ThrowIfDisposed();
        IReadOnlyList<SqliteParameter> sqliteParameters = MaterializeSqliteParameters(parameters);
        return _engine.QueryNamedDataTable(queryName, sqliteParameters);
    }

    public void ExecuteInTransaction(Action<ISqliteTransactionScope> action)
    {
        ThrowIfDisposed();
        _engine.ExecuteInTransaction(action);
    }

    public T ExecuteInTransaction<T>(Func<ISqliteTransactionScope, T> func)
    {
        ThrowIfDisposed();
        return _engine.ExecuteInTransaction(func);
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

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SqliteDatabase));
    }
}
