using CodexExpensa.Core.Abstractions;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.Data.Sqlite.Db;

/// <summary>
/// SQLite database session used by the app.
/// Supports:
/// - File DB (direct)
/// - In-memory DB seeded from a file DB, with Save() flushing memory back to disk.
/// </summary>
public sealed class SqliteDatabase : IDatabaseSession, IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly string? _persistedFilePath;
    private bool _disposed;

    private SqliteDatabase(SqliteConnection connection, string? persistedFilePath)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _persistedFilePath = persistedFilePath;
    }

    /// <summary>
    /// Opens a file-backed SQLite database and keeps a single open connection for the lifetime of this instance.
    /// </summary>
    public static SqliteDatabase OpenFile(string dbPath)
    {
        if (string.IsNullOrWhiteSpace(dbPath))
            throw new ArgumentException("dbPath is required.", nameof(dbPath));

        var folder = Path.GetDirectoryName(dbPath);
        if (string.IsNullOrWhiteSpace(folder))
            throw new ArgumentException("dbPath must include a directory.", nameof(dbPath));

        Directory.CreateDirectory(folder);

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate
        };

        var conn = new SqliteConnection(builder.ToString());
        conn.Open();

        // NOTE: In file mode, Save() is a no-op.
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
        using var fileDb = OpenFile(dbPath);

        // Create the in-memory DB and keep it open for the entire app session.
        var memBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = ":memory:",
            Mode = SqliteOpenMode.Memory,
            Cache = SqliteCacheMode.Shared
        };

        var memConn = new SqliteConnection(memBuilder.ToString());
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
        using var fileDb = OpenFile(_persistedFilePath);
        _connection.BackupDatabase(fileDb._connection);
    }

    public bool IsInMemory => !string.IsNullOrWhiteSpace(_persistedFilePath);

    public string? PersistedFilePath => _persistedFilePath;

    public int ExecuteNonQuery(string sql, IEnumerable<SqliteParameter>? parameters = null, SqliteTransaction? tx = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("sql is required.", nameof(sql));

        if (tx is not null)
        {
            using var cmd = tx.Connection!.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;

            if (parameters is not null)
            {
                foreach (var p in parameters)
                    cmd.Parameters.Add(p);
            }

            return cmd.ExecuteNonQuery();
        }

        using var cmd2 = _connection.CreateCommand();
        cmd2.CommandText = sql;

        if (parameters is not null)
        {
            foreach (var p in parameters)
                cmd2.Parameters.Add(p);
        }

        return cmd2.ExecuteNonQuery();
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

        var results = new List<T>();

        if (tx is not null)
        {
            using var cmd = tx.Connection!.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;

            if (parameters is not null)
            {
                foreach (var p in parameters)
                    cmd.Parameters.Add(p);
            }

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                results.Add(map(reader));

            return results;
        }

        using var cmd2 = _connection.CreateCommand();
        cmd2.CommandText = sql;

        if (parameters is not null)
        {
            foreach (var p in parameters)
                cmd2.Parameters.Add(p);
        }

        using var reader2 = cmd2.ExecuteReader();
        while (reader2.Read())
            results.Add(map(reader2));

        return results;
    }

    public void ExecuteInTransaction(Action<SqliteTransaction> action)
    {
        ThrowIfDisposed();

        if (action is null)
            throw new ArgumentNullException(nameof(action));

        using var tx = _connection.BeginTransaction();
        try
        {
            action(tx);
            tx.Commit();
        }
        catch
        {
            try { tx.Rollback(); } catch { /* ignore */ }
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _connection.Dispose();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SqliteDatabase));
    }
}