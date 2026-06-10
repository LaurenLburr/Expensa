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
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDatabasePath);

        if (!File.Exists(sourceDatabasePath))
        {
            throw new FileNotFoundException(
                $"Add-in database file was not found: {sourceDatabasePath}",
                sourceDatabasePath);
        }

        SqliteConnection memoryConnection = new("Data Source=:memory:");

        try
        {
            memoryConnection.Open();

            using SqliteConnection sourceConnection =
                new($"Data Source={sourceDatabasePath};Mode=ReadOnly");

            sourceConnection.Open();
            sourceConnection.BackupDatabase(memoryConnection);

            return new AddinMemoryDatabaseSession(
                sourceDatabasePath,
                memoryConnection);
        }
        catch
        {
            memoryConnection.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Connection.Dispose();
        _disposed = true;
    }
}
