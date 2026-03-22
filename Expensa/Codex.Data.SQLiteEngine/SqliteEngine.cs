using System.Data;
using Codex.Data.SQLiteEngine.Abstractions;
using EngineConfig = Codex.Data.SQLiteEngine.Configuration;
using Microsoft.Data.Sqlite;

namespace Codex.Data.SQLiteEngine;

public sealed class SqliteEngine : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly bool _ownsConnection;
    private readonly ISqlCatalog? _sqlCatalog;
    private readonly ICommandLogger? _commandLogger;
    private readonly EngineConfig.SqliteEngineOptions? _options;
    private bool _disposed;

    public SqliteEngine(
        SqliteConnection connection,
        bool ownsConnection = false,
        ISqlCatalog? sqlCatalog = null,
        ICommandLogger? commandLogger = null,
        EngineConfig.SqliteEngineOptions? options = null)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _ownsConnection = ownsConnection;
        _sqlCatalog = sqlCatalog;
        _commandLogger = commandLogger;
        _options = options;
    }

    public int ExecuteNonQuery(string sql, IEnumerable<SqliteParameter>? parameters = null, string? queryName = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("sql is required.", nameof(sql));

        using SqliteCommand cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        AddParameters(cmd, parameters);

        try
        {
            int rows = cmd.ExecuteNonQuery();
            Log("ExecuteNonQuery", BuildDetails(queryName, sql, rows));
            return rows;
        }
        catch (Exception ex)
        {
            Log("ExecuteNonQueryFailed", BuildDetails(queryName, sql, error: ex.Message));
            throw;
        }
    }

    public int ExecuteNamedNonQuery(string queryName, IEnumerable<SqliteParameter>? parameters = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(queryName))
            throw new ArgumentException("queryName is required.", nameof(queryName));

        string sql = ResolveSql(queryName, tx: null);
        return ExecuteNonQuery(sql, parameters, queryName);
    }

    public IReadOnlyList<T> Query<T>(string sql, Func<SqliteDataReader, T> map, IEnumerable<SqliteParameter>? parameters = null, string? queryName = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("sql is required.", nameof(sql));
        if (map is null)
            throw new ArgumentNullException(nameof(map));

        using SqliteCommand cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        AddParameters(cmd, parameters);

        try
        {
            List<T> results = new();
            using SqliteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                results.Add(map(reader));

            Log("Query", BuildDetails(queryName, sql, results.Count));
            return results;
        }
        catch (Exception ex)
        {
            Log("QueryFailed", BuildDetails(queryName, sql, error: ex.Message));
            throw;
        }
    }

    public IReadOnlyList<T> QueryNamed<T>(string queryName, Func<SqliteDataReader, T> map, IEnumerable<SqliteParameter>? parameters = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(queryName))
            throw new ArgumentException("queryName is required.", nameof(queryName));

        string sql = ResolveSql(queryName, tx: null);
        return Query(sql, map, parameters, queryName);
    }

    public DataTable QueryDataTable(string sql, IEnumerable<SqliteParameter>? parameters = null, string? queryName = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("sql is required.", nameof(sql));

        using SqliteCommand cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        AddParameters(cmd, parameters);

        try
        {
            using SqliteDataReader reader = cmd.ExecuteReader();
            DataTable table = new();
            table.Load(reader);

            Log("QueryDataTable", BuildDetails(queryName, sql, table.Rows.Count));
            return table;
        }
        catch (Exception ex)
        {
            Log("QueryDataTableFailed", BuildDetails(queryName, sql, error: ex.Message));
            throw;
        }
    }

    public DataTable QueryNamedDataTable(string queryName, IEnumerable<SqliteParameter>? parameters = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(queryName))
            throw new ArgumentException("queryName is required.", nameof(queryName));

        string sql = ResolveSql(queryName, tx: null);
        return QueryDataTable(sql, parameters, queryName);
    }

    public void ExecuteInTransaction(Action<ISqliteTransactionScope> action)
    {
        ThrowIfDisposed();

        if (action is null)
            throw new ArgumentNullException(nameof(action));

        using ISqliteTransactionScope tx = BeginTransaction();
        try
        {
            action(tx);
            tx.Commit();
        }
        catch
        {
            try { tx.Rollback(); } catch { }
            throw;
        }
    }

    public T ExecuteInTransaction<T>(Func<ISqliteTransactionScope, T> func)
    {
        ThrowIfDisposed();

        if (func is null)
            throw new ArgumentNullException(nameof(func));

        using ISqliteTransactionScope tx = BeginTransaction();
        try
        {
            T result = func(tx);
            tx.Commit();
            return result;
        }
        catch
        {
            try { tx.Rollback(); } catch { }
            throw;
        }
    }

    public ISqliteTransactionScope BeginTransaction()
    {
        ThrowIfDisposed();
        return new SqliteTransactionScope(_connection.BeginTransaction(), Log);
    }

    public string ResolveSql(string queryName, ISqliteTransactionScope? tx)
    {
        if (_sqlCatalog is null)
            throw new InvalidOperationException("No SQL catalog is configured for named-query execution.");

        EngineConfig.TransactionParticipationMode mode =
            _options?.SqlCatalog?.TransactionParticipation
            ?? EngineConfig.TransactionParticipationMode.UseTransactionWhenAvailable;

        return mode switch
        {
            EngineConfig.TransactionParticipationMode.UseConnectionOnly => _sqlCatalog.GetSql(queryName),
            EngineConfig.TransactionParticipationMode.UseTransactionWhenAvailable => tx is null
                ? _sqlCatalog.GetSql(queryName)
                : _sqlCatalog.GetSql(queryName, tx),
            EngineConfig.TransactionParticipationMode.RequireTransaction => tx is null
                ? throw new InvalidOperationException("A transaction is required for SQL catalog access.")
                : _sqlCatalog.GetSql(queryName, tx),
            _ => _sqlCatalog.GetSql(queryName)
        };
    }

    private static void AddParameters(SqliteCommand command, IEnumerable<SqliteParameter>? parameters)
    {
        if (parameters is null)
            return;

        foreach (SqliteParameter p in parameters)
            command.Parameters.Add(p);
    }

    private string BuildDetails(string? queryName, string sql, int? rowCount = null, string? error = null)
    {
        string namePart = string.IsNullOrWhiteSpace(queryName) ? "(ad hoc)" : queryName;
        string details = $"Name={namePart}; SQL={sql}";
        if (rowCount is not null)
            details += $"; Rows={rowCount.Value}";
        if (!string.IsNullOrWhiteSpace(error))
            details += $"; Error={error}";
        return details;
    }

    internal void Log(string action, string? details, ISqliteTransactionScope? tx = null)
    {
        if (_commandLogger is null)
            return;

        EngineConfig.TransactionParticipationMode mode =
            _options?.Logging?.TransactionParticipation
            ?? EngineConfig.TransactionParticipationMode.UseConnectionOnly;

        switch (mode)
        {
            case EngineConfig.TransactionParticipationMode.UseConnectionOnly:
                _commandLogger.Log(action, details);
                break;

            case EngineConfig.TransactionParticipationMode.UseTransactionWhenAvailable:
                if (tx is null)
                    _commandLogger.Log(action, details);
                else
                    _commandLogger.Log(action, details, tx);
                break;

            case EngineConfig.TransactionParticipationMode.RequireTransaction:
                if (tx is null)
                    throw new InvalidOperationException("A transaction is required for command logging.");
                _commandLogger.Log(action, details, tx);
                break;

            default:
                _commandLogger.Log(action, details);
                break;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_ownsConnection)
            _connection.Dispose();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SqliteEngine));
    }
}
