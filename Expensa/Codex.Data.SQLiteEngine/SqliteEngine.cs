using Microsoft.Data.Sqlite;
using System.Data;

namespace Codex.Data.SQLiteEngine;

public sealed class SqliteEngine : ISqliteEngine, IDisposable
{
    private readonly string? _connectionString;
    private readonly SqliteEngineOptions _options;
    private readonly object _syncRoot = new();
    private readonly bool _ownsProvidedConnection;

    private SqliteConnection? _sharedConnection;
    private bool _disposed;

    public event EventHandler<SqliteCommandEventArgs>? CommandExecuting;

    public event EventHandler<SqliteCommandEventArgs>? CommandExecuted;

    public event EventHandler<SqliteCommandEventArgs>? CommandFailed;

    public event EventHandler<SqliteTransactionEventArgs>? TransactionBegan;

    public event EventHandler<SqliteTransactionEventArgs>? TransactionCommitted;

    public event EventHandler<SqliteTransactionEventArgs>? TransactionRolledBack;

    public event EventHandler<SqliteTransactionEventArgs>? TransactionFailed;

    public SqliteEngine(string connectionString)
        : this(connectionString, new SqliteEngineOptions())
    {
    }

    public SqliteEngine(string connectionString, SqliteEngineOptions options)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("connectionString is required.", nameof(connectionString));

        _connectionString = connectionString;
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public SqliteEngine(SqliteConnection connection, bool ownsConnection = false)
    {
        _sharedConnection = connection ?? throw new ArgumentNullException(nameof(connection));
        _ownsProvidedConnection = ownsConnection;
        _options = new SqliteEngineOptions
        {
            UseSingleSharedConnection = true
        };

        if (_sharedConnection.State != ConnectionState.Open)
            _sharedConnection.Open();
    }

    public int ExecuteNonQuery(string sql, IReadOnlyList<SqliteParameter>? parameters = null, string? queryName = null)
    {
        ThrowIfDisposed();
        ValidateSql(sql);

        DateTime startedUtc = DateTime.UtcNow;
        IReadOnlyList<SqliteParameterSnapshot> snapshot = CreateSnapshot(parameters);

        RaiseCommandExecuting(queryName, sql, snapshot, startedUtc, false, null);

        try
        {
            SqliteConnection connection = GetConnectionForCommand(out bool ownsConnection);

            try
            {
                using SqliteCommand command = CreateCommand(connection, null, sql, parameters);
                int rowsAffected = command.ExecuteNonQuery();
                RaiseCommandExecuted(queryName, sql, snapshot, startedUtc, rowsAffected, null, false, null);
                return rowsAffected;
            }
            finally
            {
                if (ownsConnection)
                    connection.Dispose();
            }
        }
        catch (Exception ex)
        {
            RaiseCommandFailed(queryName, sql, snapshot, startedUtc, ex, false, null);
            throw;
        }
    }

    public T? ExecuteScalar<T>(string sql, IReadOnlyList<SqliteParameter>? parameters = null, string? queryName = null)
    {
        ThrowIfDisposed();
        ValidateSql(sql);

        DateTime startedUtc = DateTime.UtcNow;
        IReadOnlyList<SqliteParameterSnapshot> snapshot = CreateSnapshot(parameters);

        RaiseCommandExecuting(queryName, sql, snapshot, startedUtc, false, null);

        try
        {
            SqliteConnection connection = GetConnectionForCommand(out bool ownsConnection);

            try
            {
                using SqliteCommand command = CreateCommand(connection, null, sql, parameters);
                object? rawResult = command.ExecuteScalar();
                T? result = ConvertScalar<T>(rawResult);

                RaiseCommandExecuted(queryName, sql, snapshot, startedUtc, null, rawResult, false, null);
                return result;
            }
            finally
            {
                if (ownsConnection)
                    connection.Dispose();
            }
        }
        catch (Exception ex)
        {
            RaiseCommandFailed(queryName, sql, snapshot, startedUtc, ex, false, null);
            throw;
        }
    }

    public IReadOnlyList<T> Query<T>(
        string sql,
        Func<SqliteDataReader, T> map,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null)
    {
        ThrowIfDisposed();
        ValidateSql(sql);

        if (map is null)
            throw new ArgumentNullException(nameof(map));

        DateTime startedUtc = DateTime.UtcNow;
        IReadOnlyList<SqliteParameterSnapshot> snapshot = CreateSnapshot(parameters);

        RaiseCommandExecuting(queryName, sql, snapshot, startedUtc, false, null);

        try
        {
            SqliteConnection connection = GetConnectionForCommand(out bool ownsConnection);

            try
            {
                using SqliteCommand command = CreateCommand(connection, null, sql, parameters);
                using SqliteDataReader reader = command.ExecuteReader();

                List<T> results = new();

                while (reader.Read())
                    results.Add(map(reader));

                RaiseCommandExecuted(queryName, sql, snapshot, startedUtc, results.Count, null, false, null);
                return results;
            }
            finally
            {
                if (ownsConnection)
                    connection.Dispose();
            }
        }
        catch (Exception ex)
        {
            RaiseCommandFailed(queryName, sql, snapshot, startedUtc, ex, false, null);
            throw;
        }
    }

    public DataTable QueryDataTable(
        string sql,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null)
    {
        ThrowIfDisposed();
        ValidateSql(sql);

        DateTime startedUtc = DateTime.UtcNow;
        IReadOnlyList<SqliteParameterSnapshot> snapshot = CreateSnapshot(parameters);

        RaiseCommandExecuting(queryName, sql, snapshot, startedUtc, false, null);

        try
        {
            SqliteConnection connection = GetConnectionForCommand(out bool ownsConnection);

            try
            {
                using SqliteCommand command = CreateCommand(connection, null, sql, parameters);
                using SqliteDataReader reader = command.ExecuteReader();

                DataTable table = new();
                table.Load(reader);

                RaiseCommandExecuted(queryName, sql, snapshot, startedUtc, table.Rows.Count, null, false, null);
                return table;
            }
            finally
            {
                if (ownsConnection)
                    connection.Dispose();
            }
        }
        catch (Exception ex)
        {
            RaiseCommandFailed(queryName, sql, snapshot, startedUtc, ex, false, null);
            throw;
        }
    }

    public ISqliteTransactionScope BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable)
    {
        ThrowIfDisposed();

        DateTime startedUtc = DateTime.UtcNow;

        try
        {
            SqliteConnection connection;
            bool ownsConnection;

            if (_options.UseSingleSharedConnection || _sharedConnection is not null)
            {
                connection = GetOrCreateSharedConnection();
                ownsConnection = false;
            }
            else
            {
                connection = CreateOpenConnection();
                ownsConnection = true;
            }

            SqliteTransaction transaction = connection.BeginTransaction(isolationLevel);
            Guid transactionId = Guid.NewGuid();

            RaiseTransactionBegan(transactionId, startedUtc);

            return new SqliteTransactionScope(this, connection, transaction, transactionId, startedUtc, ownsConnection);
        }
        catch (Exception ex)
        {
            RaiseTransactionFailed(Guid.NewGuid(), startedUtc, ex);
            throw;
        }
    }

    internal int ExecuteNonQueryInTransaction(
        SqliteConnection connection,
        SqliteTransaction transaction,
        Guid transactionId,
        string sql,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null)
    {
        ThrowIfDisposed();
        ValidateSql(sql);

        DateTime startedUtc = DateTime.UtcNow;
        IReadOnlyList<SqliteParameterSnapshot> snapshot = CreateSnapshot(parameters);

        RaiseCommandExecuting(queryName, sql, snapshot, startedUtc, true, transactionId);

        try
        {
            using SqliteCommand command = CreateCommand(connection, transaction, sql, parameters);
            int rowsAffected = command.ExecuteNonQuery();
            RaiseCommandExecuted(queryName, sql, snapshot, startedUtc, rowsAffected, null, true, transactionId);
            return rowsAffected;
        }
        catch (Exception ex)
        {
            RaiseCommandFailed(queryName, sql, snapshot, startedUtc, ex, true, transactionId);
            throw;
        }
    }

    internal T? ExecuteScalarInTransaction<T>(
        SqliteConnection connection,
        SqliteTransaction transaction,
        Guid transactionId,
        string sql,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null)
    {
        ThrowIfDisposed();
        ValidateSql(sql);

        DateTime startedUtc = DateTime.UtcNow;
        IReadOnlyList<SqliteParameterSnapshot> snapshot = CreateSnapshot(parameters);

        RaiseCommandExecuting(queryName, sql, snapshot, startedUtc, true, transactionId);

        try
        {
            using SqliteCommand command = CreateCommand(connection, transaction, sql, parameters);
            object? rawResult = command.ExecuteScalar();
            T? result = ConvertScalar<T>(rawResult);
            RaiseCommandExecuted(queryName, sql, snapshot, startedUtc, null, rawResult, true, transactionId);
            return result;
        }
        catch (Exception ex)
        {
            RaiseCommandFailed(queryName, sql, snapshot, startedUtc, ex, true, transactionId);
            throw;
        }
    }

    internal IReadOnlyList<T> QueryInTransaction<T>(
        SqliteConnection connection,
        SqliteTransaction transaction,
        Guid transactionId,
        string sql,
        Func<SqliteDataReader, T> map,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null)
    {
        ThrowIfDisposed();
        ValidateSql(sql);

        if (map is null)
            throw new ArgumentNullException(nameof(map));

        DateTime startedUtc = DateTime.UtcNow;
        IReadOnlyList<SqliteParameterSnapshot> snapshot = CreateSnapshot(parameters);

        RaiseCommandExecuting(queryName, sql, snapshot, startedUtc, true, transactionId);

        try
        {
            using SqliteCommand command = CreateCommand(connection, transaction, sql, parameters);
            using SqliteDataReader reader = command.ExecuteReader();

            List<T> results = new();

            while (reader.Read())
                results.Add(map(reader));

            RaiseCommandExecuted(queryName, sql, snapshot, startedUtc, results.Count, null, true, transactionId);
            return results;
        }
        catch (Exception ex)
        {
            RaiseCommandFailed(queryName, sql, snapshot, startedUtc, ex, true, transactionId);
            throw;
        }
    }

    internal DataTable QueryDataTableInTransaction(
        SqliteConnection connection,
        SqliteTransaction transaction,
        Guid transactionId,
        string sql,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null)
    {
        ThrowIfDisposed();
        ValidateSql(sql);

        DateTime startedUtc = DateTime.UtcNow;
        IReadOnlyList<SqliteParameterSnapshot> snapshot = CreateSnapshot(parameters);

        RaiseCommandExecuting(queryName, sql, snapshot, startedUtc, true, transactionId);

        try
        {
            using SqliteCommand command = CreateCommand(connection, transaction, sql, parameters);
            using SqliteDataReader reader = command.ExecuteReader();

            DataTable table = new();
            table.Load(reader);

            RaiseCommandExecuted(queryName, sql, snapshot, startedUtc, table.Rows.Count, null, true, transactionId);
            return table;
        }
        catch (Exception ex)
        {
            RaiseCommandFailed(queryName, sql, snapshot, startedUtc, ex, true, transactionId);
            throw;
        }
    }

    internal void RaiseTransactionCommitted(Guid transactionId, DateTime startedUtc)
    {
        TransactionCommitted?.Invoke(this, new SqliteTransactionEventArgs
        {
            TransactionId = transactionId,
            StartedUtc = startedUtc,
            Duration = DateTime.UtcNow - startedUtc
        });
    }

    internal void RaiseTransactionRolledBack(Guid transactionId, DateTime startedUtc)
    {
        TransactionRolledBack?.Invoke(this, new SqliteTransactionEventArgs
        {
            TransactionId = transactionId,
            StartedUtc = startedUtc,
            Duration = DateTime.UtcNow - startedUtc
        });
    }

    internal void RaiseTransactionFailed(Guid transactionId, DateTime startedUtc, Exception exception)
    {
        TransactionFailed?.Invoke(this, new SqliteTransactionEventArgs
        {
            TransactionId = transactionId,
            StartedUtc = startedUtc,
            Duration = DateTime.UtcNow - startedUtc,
            Exception = exception
        });
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        lock (_syncRoot)
        {
            if (_disposed)
                return;

            if (_ownsProvidedConnection || _connectionString is not null)
            {
                _sharedConnection?.Dispose();
            }

            _sharedConnection = null;
            _disposed = true;
        }
    }

    private SqliteConnection GetConnectionForCommand(out bool ownsConnection)
    {
        if (_sharedConnection is not null || _options.UseSingleSharedConnection)
        {
            ownsConnection = false;
            return GetOrCreateSharedConnection();
        }

        ownsConnection = true;
        return CreateOpenConnection();
    }

    private SqliteConnection GetOrCreateSharedConnection()
    {
        lock (_syncRoot)
        {
            ThrowIfDisposed();

            if (_sharedConnection is null)
            {
                _sharedConnection = CreateOpenConnection();
            }
            else if (_sharedConnection.State != ConnectionState.Open)
            {
                _sharedConnection.Open();
            }

            return _sharedConnection;
        }
    }

    private SqliteConnection CreateOpenConnection()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("A connection string is not available for this engine instance.");

        SqliteConnection connection = new(_connectionString);
        connection.Open();
        return connection;
    }

    private static SqliteCommand CreateCommand(
        SqliteConnection connection,
        SqliteTransaction? transaction,
        string sql,
        IReadOnlyList<SqliteParameter>? parameters)
    {
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;

        if (parameters is not null)
        {
            foreach (SqliteParameter parameter in parameters)
                command.Parameters.Add(parameter);
        }

        return command;
    }

    private static void ValidateSql(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("sql is required.", nameof(sql));
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SqliteEngine));
    }

    private void RaiseCommandExecuting(
        string? queryName,
        string sql,
        IReadOnlyList<SqliteParameterSnapshot> parameters,
        DateTime startedUtc,
        bool isInTransaction,
        Guid? transactionId)
    {
        CommandExecuting?.Invoke(this, new SqliteCommandEventArgs
        {
            QueryName = queryName,
            Sql = sql,
            Parameters = parameters,
            StartedUtc = startedUtc,
            Duration = TimeSpan.Zero,
            IsInTransaction = isInTransaction,
            TransactionId = transactionId
        });
    }

    private void RaiseCommandExecuted(
        string? queryName,
        string sql,
        IReadOnlyList<SqliteParameterSnapshot> parameters,
        DateTime startedUtc,
        int? rowsAffected,
        object? scalarResult,
        bool isInTransaction,
        Guid? transactionId)
    {
        CommandExecuted?.Invoke(this, new SqliteCommandEventArgs
        {
            QueryName = queryName,
            Sql = sql,
            Parameters = parameters,
            StartedUtc = startedUtc,
            Duration = DateTime.UtcNow - startedUtc,
            RowsAffected = rowsAffected,
            ScalarResult = scalarResult,
            IsInTransaction = isInTransaction,
            TransactionId = transactionId
        });
    }

    private void RaiseCommandFailed(
        string? queryName,
        string sql,
        IReadOnlyList<SqliteParameterSnapshot> parameters,
        DateTime startedUtc,
        Exception exception,
        bool isInTransaction,
        Guid? transactionId)
    {
        CommandFailed?.Invoke(this, new SqliteCommandEventArgs
        {
            QueryName = queryName,
            Sql = sql,
            Parameters = parameters,
            StartedUtc = startedUtc,
            Duration = DateTime.UtcNow - startedUtc,
            Exception = exception,
            IsInTransaction = isInTransaction,
            TransactionId = transactionId
        });
    }

    private void RaiseTransactionBegan(Guid transactionId, DateTime startedUtc)
    {
        TransactionBegan?.Invoke(this, new SqliteTransactionEventArgs
        {
            TransactionId = transactionId,
            StartedUtc = startedUtc,
            Duration = TimeSpan.Zero
        });
    }

    private static IReadOnlyList<SqliteParameterSnapshot> CreateSnapshot(IReadOnlyList<SqliteParameter>? parameters)
    {
        if (parameters is null || parameters.Count == 0)
            return [];

        List<SqliteParameterSnapshot> snapshots = new(parameters.Count);

        foreach (SqliteParameter parameter in parameters)
        {
            snapshots.Add(new SqliteParameterSnapshot
            {
                Name = parameter.ParameterName,
                Value = parameter.Value,
                DbType = parameter.SqliteType.ToString()
            });
        }

        return snapshots;
    }

    private static T? ConvertScalar<T>(object? rawResult)
    {
        if (rawResult is null || rawResult is DBNull)
            return default;

        if (rawResult is T direct)
            return direct;

        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        object converted = Convert.ChangeType(rawResult, targetType);
        return (T)converted;
    }
}
