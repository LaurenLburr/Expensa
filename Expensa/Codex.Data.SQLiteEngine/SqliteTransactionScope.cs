using Microsoft.Data.Sqlite;
using System.Data;

namespace Codex.Data.SQLiteEngine;

internal sealed class SqliteTransactionScope : ISqliteTransactionScope
{
    private readonly SqliteEngine _engine;
    private readonly SqliteConnection _connection;
    private readonly SqliteTransaction _transaction;
    private readonly DateTime _startedUtc;
    private readonly bool _ownsConnection;

    private bool _completed;
    private bool _disposed;

    public Guid TransactionId { get; }

    public SqliteTransactionScope(
        SqliteEngine engine,
        SqliteConnection connection,
        SqliteTransaction transaction,
        Guid transactionId,
        DateTime startedUtc,
        bool ownsConnection)
    {
        _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        TransactionId = transactionId;
        _startedUtc = startedUtc;
        _ownsConnection = ownsConnection;
    }

    public int ExecuteNonQuery(string sql, IReadOnlyList<SqliteParameter>? parameters = null, string? queryName = null)
    {
        ThrowIfDisposed();
        return _engine.ExecuteNonQueryInTransaction(_connection, _transaction, TransactionId, sql, parameters, queryName);
    }

    public T? ExecuteScalar<T>(string sql, IReadOnlyList<SqliteParameter>? parameters = null, string? queryName = null)
    {
        ThrowIfDisposed();
        return _engine.ExecuteScalarInTransaction<T>(_connection, _transaction, TransactionId, sql, parameters, queryName);
    }

    public IReadOnlyList<T> Query<T>(
        string sql,
        Func<SqliteDataReader, T> map,
        IReadOnlyList<SqliteParameter>? parameters = null,
        string? queryName = null)
    {
        ThrowIfDisposed();
        return _engine.QueryInTransaction(_connection, _transaction, TransactionId, sql, map, parameters, queryName);
    }

    public DataTable QueryDataTable(string sql, IReadOnlyList<SqliteParameter>? parameters = null, string? queryName = null)
    {
        ThrowIfDisposed();
        return _engine.QueryDataTableInTransaction(_connection, _transaction, TransactionId, sql, parameters, queryName);
    }

    public void Commit()
    {
        ThrowIfDisposed();

        if (_completed)
            throw new InvalidOperationException("Transaction has already been completed.");

        try
        {
            _transaction.Commit();
            _completed = true;
            _engine.RaiseTransactionCommitted(TransactionId, _startedUtc);
        }
        catch (Exception ex)
        {
            _engine.RaiseTransactionFailed(TransactionId, _startedUtc, ex);
            throw;
        }
    }

    public void Rollback()
    {
        ThrowIfDisposed();

        if (_completed)
            throw new InvalidOperationException("Transaction has already been completed.");

        try
        {
            _transaction.Rollback();
            _completed = true;
            _engine.RaiseTransactionRolledBack(TransactionId, _startedUtc);
        }
        catch (Exception ex)
        {
            _engine.RaiseTransactionFailed(TransactionId, _startedUtc, ex);
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        try
        {
            if (!_completed)
            {
                _transaction.Rollback();
                _engine.RaiseTransactionRolledBack(TransactionId, _startedUtc);
                _completed = true;
            }
        }
        finally
        {
            _transaction.Dispose();

            if (_ownsConnection)
                _connection.Dispose();

            _disposed = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SqliteTransactionScope));
    }
}
