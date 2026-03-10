using System.Data;
using System.Data.Common;
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
    private readonly SqliteQueryCatalog _queryCatalog;
    private readonly string? _persistedFilePath;
    private bool _disposed;

    private SqliteDatabase(SqliteConnection connection, string? persistedFilePath)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _persistedFilePath = persistedFilePath;
        _queryCatalog = new SqliteQueryCatalog(_connection);
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

        // In file mode, Save() is a no-op.
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

        // Ensure the file DB exists and is a valid SQLite database.
        using SqliteDatabase fileDb = OpenFile(dbPath);

        // Create the in-memory DB and keep it open for the entire app session.
        SqliteConnectionStringBuilder memBuilder = new()
        {
            DataSource = ":memory:",
            Mode = SqliteOpenMode.Memory,
            Cache = SqliteCacheMode.Shared
        };

        SqliteConnection memConn = new(memBuilder.ToString());
        memConn.Open();

        // Copy disk -> memory
        fileDb._connection.BackupDatabase(memConn);

        return new SqliteDatabase(memConn, persistedFilePath: dbPath);
    }

    /// <summary>
    /// Flushes the in-memory database back to the persisted file (if any).
    /// File mode: no-op.
    /// </summary>
    public void Save()
    {
        ThrowIfDisposed();

        // File mode: nothing special to do.
        if (string.IsNullOrWhiteSpace(_persistedFilePath))
            return;

        // Flush memory -> disk
        using SqliteDatabase fileDb = OpenFile(_persistedFilePath);
        _connection.BackupDatabase(fileDb._connection);
    }

    /// <summary>
    /// True when the database is running in memory with a persisted backing file.
    /// </summary>
    public bool IsInMemory => !string.IsNullOrWhiteSpace(_persistedFilePath);

    /// <summary>
    /// Full path to the persisted database file if one exists.
    /// </summary>
    public string? PersistedFilePath => _persistedFilePath;

    /// <summary>
    /// Executes raw SQL and returns the affected row count.
    /// Use this for bootstrap/schema operations that must not depend on the SqlQuery catalog.
    /// </summary>
    public int ExecuteNonQuery(
        string sql,
        IEnumerable<SqliteParameter>? parameters = null,
        SqliteTransaction? tx = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("sql is required.", nameof(sql));

        if (tx is not null)
        {
            using SqliteCommand cmd = tx.Connection!.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;

            if (parameters is not null)
            {
                foreach (SqliteParameter p in parameters)
                    cmd.Parameters.Add(p);
            }

            return cmd.ExecuteNonQuery();
        }

        using SqliteCommand cmd2 = _connection.CreateCommand();
        cmd2.CommandText = sql;

        if (parameters is not null)
        {
            foreach (SqliteParameter p in parameters)
                cmd2.Parameters.Add(p);
        }

        return cmd2.ExecuteNonQuery();
    }

    /// <summary>
    /// Executes a named query from the SqlQuery catalog and returns nothing.
    /// This method implements the UI-facing IDatabaseSession contract.
    /// </summary>
    public void Execute(
        string queryName,
        IEnumerable<DbParameter>? parameters = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(queryName))
            throw new ArgumentException("queryName is required.", nameof(queryName));

        string sql = _queryCatalog.GetSql(queryName);

        using SqliteCommand cmd = _connection.CreateCommand();
        cmd.CommandText = sql;

        if (parameters is not null)
        {
            foreach (DbParameter p in parameters)
                cmd.Parameters.Add(p);
        }

        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Executes a named query from the SqlQuery catalog and returns the affected row count.
    /// Use this in the SQLite data layer when transaction support is needed.
    /// </summary>
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

    /// <summary>
    /// Executes raw SQL and maps the result set.
    /// Use this for bootstrap/schema operations that must not depend on the SqlQuery catalog.
    /// </summary>
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

        List<T> results = new();

        if (tx is not null)
        {
            using SqliteCommand cmd = tx.Connection!.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;

            if (parameters is not null)
            {
                foreach (SqliteParameter p in parameters)
                    cmd.Parameters.Add(p);
            }

            using SqliteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                results.Add(map(reader));

            return results;
        }

        using SqliteCommand cmd2 = _connection.CreateCommand();
        cmd2.CommandText = sql;

        if (parameters is not null)
        {
            foreach (SqliteParameter p in parameters)
                cmd2.Parameters.Add(p);
        }

        using SqliteDataReader reader2 = cmd2.ExecuteReader();
        while (reader2.Read())
            results.Add(map(reader2));

        return results;
    }

    /// <summary>
    /// Executes a named query from the SqlQuery catalog and maps the result set.
    /// Use this for normal application queries stored in SqlQuery.
    /// </summary>
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

        return Query(sql, map, parameters, tx);
    }

    /// <summary>
    /// Executes a named query from the SqlQuery catalog and returns a DataTable.
    /// This method implements the UI-facing IDatabaseSession contract.
    /// </summary>
    public DataTable QueryDataTable(
        string queryName,
        IEnumerable<DbParameter>? parameters = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(queryName))
            throw new ArgumentException("queryName is required.", nameof(queryName));

        string sql = _queryCatalog.GetSql(queryName);

        DataTable table = new();

        using SqliteCommand cmd = _connection.CreateCommand();
        cmd.CommandText = sql;

        if (parameters is not null)
        {
            foreach (DbParameter p in parameters)
                cmd.Parameters.Add(p);
        }

        using SqliteDataReader reader = cmd.ExecuteReader();
        table.Load(reader);

        return table;
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
                // ignore rollback failures
            }

            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _connection.Dispose();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SqliteDatabase));
    }
}