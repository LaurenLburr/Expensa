using Microsoft.Data.Sqlite;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class AddinMemoryDatabaseSession : IDisposable
{
    private bool _disposed;

    private AddinMemoryDatabaseSession(
        string sourceDatabasePath,
        SqliteConnection connection)
    {
        SourceDatabasePath = sourceDatabasePath;
        Connection = connection;
    }

    public string SourceDatabasePath { get; }

    public SqliteConnection Connection { get; }

    public static AddinMemoryDatabaseSession LoadFromFile(
        string sourceDatabasePath)
    {
        SqliteConnection connection =
            SafeSqliteConnection.OpenFromFile(sourceDatabasePath);

        return new AddinMemoryDatabaseSession(
            sourceDatabasePath,
            connection);
    }

    public static AddinMemoryDatabaseSession CreateEmpty(
        string targetDatabasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetDatabasePath);

        return new AddinMemoryDatabaseSession(
            targetDatabasePath,
            SafeSqliteConnection.CreateEmptyMemory());
    }

    public void Save()
    {
        SaveToFile(SourceDatabasePath);
    }

    public void SaveToFile(string databasePath)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        SafeSqliteConnection.SaveToFile(
            Connection,
            databasePath);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Connection.Close();
        Connection.Dispose();
        SqliteConnection.ClearAllPools();
        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}
