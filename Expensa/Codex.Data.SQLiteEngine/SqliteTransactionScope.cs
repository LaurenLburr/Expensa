using System.Data;
using Microsoft.Data.Sqlite;

namespace Codex.Data.SQLiteEngine;

public sealed class SqliteTransactionScope : ISqliteTransactionScope
{
    private readonly SqliteTransaction _transaction;
    private readonly Action<string, string?, ISqliteTransactionScope?>? _logger;
    private bool _completed;
    private bool _disposed;

    public SqliteTransactionScope(
        SqliteTransaction transaction,
        Action<string, string?, ISqliteTransactionScope?>? logger = null)
    {
        _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        _logger = logger;
        TransactionId = Guid.NewGuid();
        _logger?.Invoke("TransactionBegin", null, this);
    }

    public Guid TransactionId { get; }

    public int ExecuteNonQuery(
        string sql,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null)
    {
        ThrowIfCompleted();

        using SqliteCommand cmd = _transaction.Connection!.CreateCommand();
        cmd.Transaction = _transaction;
        cmd.CommandText = sql;

        if (parameters is not null)
        {
            foreach (SqliteParameter p in parameters)
                cmd.Parameters.Add(p);
        }

        try
        {
            int rows = cmd.ExecuteNonQuery();
            _logger?.Invoke(
                "TransactionExecuteNonQuery",
                $"TransactionId={TransactionId}; Name={queryName ?? "(ad hoc)"}; Rows={rows}; SQL={sql}",
                this);
            return rows;
        }
        catch (Exception ex)
        {
            _logger?.Invoke(
                "TransactionExecuteNonQueryFailed",
                $"TransactionId={TransactionId}; Name={queryName ?? "(ad hoc)"}; Error={ex.Message}; SQL={sql}",
                this);
            throw;
        }
    }

    public T ExecuteScalar<T>(
        string sql,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null)
    {
        ThrowIfCompleted();

        using SqliteCommand cmd = _transaction.Connection!.CreateCommand();
        cmd.Transaction = _transaction;
        cmd.CommandText = sql;

        if (parameters is not null)
        {
            foreach (SqliteParameter p in parameters)
                cmd.Parameters.Add(p);
        }

        try
        {
            object? result = cmd.ExecuteScalar();

            if (result is null || result is DBNull)
            {
                _logger?.Invoke(
                    "TransactionExecuteScalar",
                    $"TransactionId={TransactionId}; Name={queryName ?? "(ad hoc)"}; Result=NULL; SQL={sql}",
                    this);

                return default!;
            }

            T typedResult;

            if (result is T alreadyTyped)
            {
                typedResult = alreadyTyped;
            }
            else
            {
                typedResult = (T)Convert.ChangeType(result, typeof(T));
            }

            _logger?.Invoke(
                "TransactionExecuteScalar",
                $"TransactionId={TransactionId}; Name={queryName ?? "(ad hoc)"}; SQL={sql}",
                this);

            return typedResult;
        }
        catch (Exception ex)
        {
            _logger?.Invoke(
                "TransactionExecuteScalarFailed",
                $"TransactionId={TransactionId}; Name={queryName ?? "(ad hoc)"}; Error={ex.Message}; SQL={sql}",
                this);
            throw;
        }
    }

    public IReadOnlyList<T> Query<T>(
        string sql,
        Func<SqliteDataReader, T> map,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null)
    {
        ThrowIfCompleted();

        if (map is null)
            throw new ArgumentNullException(nameof(map));

        using SqliteCommand cmd = _transaction.Connection!.CreateCommand();
        cmd.Transaction = _transaction;
        cmd.CommandText = sql;

        if (parameters is not null)
        {
            foreach (SqliteParameter p in parameters)
                cmd.Parameters.Add(p);
        }

        try
        {
            List<T> results = new();

            using SqliteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                results.Add(map(reader));

            _logger?.Invoke(
                "TransactionQuery",
                $"TransactionId={TransactionId}; Name={queryName ?? "(ad hoc)"}; Rows={results.Count}; SQL={sql}",
                this);

            return results;
        }
        catch (Exception ex)
        {
            _logger?.Invoke(
                "TransactionQueryFailed",
                $"TransactionId={TransactionId}; Name={queryName ?? "(ad hoc)"}; Error={ex.Message}; SQL={sql}",
                this);
            throw;
        }
    }

    public DataTable QueryDataTable(
        string sql,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null)
    {
        ThrowIfCompleted();

        using SqliteCommand cmd = _transaction.Connection!.CreateCommand();
        cmd.Transaction = _transaction;
        cmd.CommandText = sql;

        if (parameters is not null)
        {
            foreach (SqliteParameter p in parameters)
                cmd.Parameters.Add(p);
        }

        try
        {
            using SqliteDataReader reader = cmd.ExecuteReader();

            DataTable table = new();
            table.Load(reader);

            _logger?.Invoke(
                "TransactionQueryDataTable",
                $"TransactionId={TransactionId}; Name={queryName ?? "(ad hoc)"}; Rows={table.Rows.Count}; SQL={sql}",
                this);

            return table;
        }
        catch (Exception ex)
        {
            _logger?.Invoke(
                "TransactionQueryDataTableFailed",
                $"TransactionId={TransactionId}; Name={queryName ?? "(ad hoc)"}; Error={ex.Message}; SQL={sql}",
                this);
            throw;
        }
    }

    public void Commit()
    {
        ThrowIfCompleted();
        _transaction.Commit();
        _completed = true;
        _logger?.Invoke("TransactionCommit", $"TransactionId={TransactionId}", this);
    }

    public void Rollback()
    {
        ThrowIfCompleted();
        _transaction.Rollback();
        _completed = true;
        _logger?.Invoke("TransactionRollback", $"TransactionId={TransactionId}", this);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (!_completed)
        {
            try
            {
                _transaction.Rollback();
                _logger?.Invoke("TransactionRollback", $"TransactionId={TransactionId}; DisposeRollback", this);
            }
            catch (Exception ex)
            {
                _logger?.Invoke("TransactionRollbackFailed", $"TransactionId={TransactionId}; {ex.Message}", this);
            }
        }

        _transaction.Dispose();
    }

    private void ThrowIfCompleted()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SqliteTransactionScope));

        if (_completed)
            throw new InvalidOperationException("Transaction scope has already completed.");
    }
}