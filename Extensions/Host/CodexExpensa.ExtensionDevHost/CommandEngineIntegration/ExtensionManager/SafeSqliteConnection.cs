using Microsoft.Data.Sqlite;
using System.Data;
using System.IO;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

/// <summary>
/// Owns every physical SQLite connection created by CommandEngineIntegration.
/// Disk databases are opened only long enough to copy to or from memory.
/// </summary>
public static class SafeSqliteConnection
{
    public static SqliteConnection CreateEmptyMemory()
    {
        SqliteConnection connection =
            new(
                new SqliteConnectionStringBuilder
                {
                    DataSource = ":memory:",
                    Mode = SqliteOpenMode.Memory,
                    Cache = SqliteCacheMode.Private,
                    Pooling = false
                }.ToString());

        connection.Open();
        return connection;
    }

    public static SqliteConnection OpenFromFile(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        FileInfo databaseFile =
            new(Path.GetFullPath(databasePath));

        if (!databaseFile.Exists)
        {
            throw new FileNotFoundException(
                $"SQLite database file was not found: {databaseFile.FullName}",
                databaseFile.FullName);
        }

        if (!IsValidSqliteFile(databaseFile))
        {
            throw new InvalidDataException(
                $"The file is not a valid SQLite database: {databaseFile.FullName}");
        }

        SqliteConnection memoryConnection = CreateEmptyMemory();

        try
        {
            using (SqliteConnection fileConnection = CreateFileConnection(
                       databasePath,
                       SqliteOpenMode.ReadOnly))
            {
                fileConnection.Open();

                if (fileConnection.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException(
                        $"SQLite database could not be opened: {databasePath}");
                }

                fileConnection.BackupDatabase(memoryConnection);
                fileConnection.Close();
            }

            SqliteConnection.ClearAllPools();
            return memoryConnection;
        }
        catch
        {
            memoryConnection.Dispose();
            SqliteConnection.ClearAllPools();
            throw;
        }
    }

    public static SqliteConnection GetCopy(SqliteConnection sourceConnection)
    {
        ArgumentNullException.ThrowIfNull(sourceConnection);

        if (sourceConnection.State != ConnectionState.Open)
        {
            throw new InvalidOperationException(
                "The source SQLite connection must be open.");
        }

        SqliteConnection memoryConnection = CreateEmptyMemory();

        try
        {
            sourceConnection.BackupDatabase(memoryConnection);
            return memoryConnection;
        }
        catch
        {
            memoryConnection.Dispose();
            throw;
        }
    }

    public static void SaveToFile(
        SqliteConnection memoryConnection,
        string databasePath)
    {
        ArgumentNullException.ThrowIfNull(memoryConnection);
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        if (memoryConnection.State != ConnectionState.Open)
        {
            throw new InvalidOperationException(
                "The memory SQLite connection must be open.");
        }

        string fullPath = Path.GetFullPath(databasePath);

        if (Directory.Exists(fullPath))
        {
            throw new InvalidOperationException(
                $"The SQLite database path is a folder, not a file: {fullPath}");
        }

        string? folder = Path.GetDirectoryName(fullPath);

        if (string.IsNullOrWhiteSpace(folder))
        {
            throw new InvalidOperationException(
                $"Could not determine the database folder for: {fullPath}");
        }

        Directory.CreateDirectory(folder);

        string temporaryPath =
            Path.Combine(
                folder,
                $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.incoming");

        try
        {
            using (SqliteConnection fileConnection = CreateFileConnection(
                       temporaryPath,
                       SqliteOpenMode.ReadWriteCreate))
            {
                fileConnection.Open();
                memoryConnection.BackupDatabase(fileConnection);
                fileConnection.Close();
            }

            SqliteConnection.ClearAllPools();

            ReplaceDatabaseFile(temporaryPath, fullPath, memoryConnection);
        }
        finally
        {
            SqliteConnection.ClearAllPools();

            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private static void ReplaceDatabaseFile(
        string temporaryPath,
        string fullPath,
        SqliteConnection memoryConnection)
    {
        try
        {
            if (File.Exists(fullPath))
            {
                File.Move(temporaryPath, fullPath, overwrite: true);
            }
            else
            {
                File.Move(temporaryPath, fullPath);
            }
        }
        catch (UnauthorizedAccessException exception)
        {
            if (File.Exists(fullPath))
            {
                BackupMemoryDatabaseToExistingFile(memoryConnection, fullPath);
                return;
            }

            throw new UnauthorizedAccessException(
                $"Access was denied while replacing the SQLite database file: {fullPath}",
                exception);
        }
        catch (IOException exception)
        {
            if (File.Exists(fullPath))
            {
                BackupMemoryDatabaseToExistingFile(memoryConnection, fullPath);
                return;
            }

            throw new IOException(
                $"Could not replace the SQLite database file: {fullPath}",
                exception);
        }
    }

    private static void BackupMemoryDatabaseToExistingFile(
        SqliteConnection memoryConnection,
        string fullPath)
    {
        try
        {
            using SqliteConnection fileConnection = CreateFileConnection(
                fullPath,
                SqliteOpenMode.ReadWrite);

            fileConnection.Open();
            memoryConnection.BackupDatabase(fileConnection);
            fileConnection.Close();
        }
        catch (Exception exception) when (exception is SqliteException or IOException or UnauthorizedAccessException)
        {
            throw new IOException(
                $"Could not save the in-memory SQLite database back to the existing database file: {fullPath}",
                exception);
        }
    }

    public static bool IsValidSqliteFile(FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (!file.Exists || file.Length < 16)
        {
            return false;
        }

        Span<byte> actualHeader =
            stackalloc byte[16];

        using FileStream stream =
            File.Open(
                file.FullName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite);

        int totalBytesRead = 0;

        while (totalBytesRead < actualHeader.Length)
        {
            int bytesRead =
                stream.Read(
                    actualHeader[totalBytesRead..]);

            if (bytesRead == 0)
            {
                return false;
            }

            totalBytesRead += bytesRead;
        }

        ReadOnlySpan<byte> expectedHeader =
            "SQLite format 3\0"u8;

        return actualHeader.SequenceEqual(expectedHeader);
    }

    private static SqliteConnection CreateFileConnection(
        string databasePath,
        SqliteOpenMode mode)
    {
        SqliteConnectionStringBuilder builder =
            new()
            {
                DataSource = Path.GetFullPath(databasePath),
                Mode = mode,
                Cache = SqliteCacheMode.Private,
                Pooling = false
            };

        return new SqliteConnection(builder.ToString());
    }
}
