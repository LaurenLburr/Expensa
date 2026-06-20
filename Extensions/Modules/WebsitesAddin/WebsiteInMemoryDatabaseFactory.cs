using Microsoft.Data.Sqlite;

namespace WebsitesAddin;

public static class WebsiteInMemoryDatabaseFactory
{
    public static SqliteConnection OpenMemoryCopy(string sourceDatabasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDatabasePath);

        if (!File.Exists(sourceDatabasePath))
        {
            throw new FileNotFoundException(
                $"Website source database was not found: {sourceDatabasePath}",
                sourceDatabasePath);
        }

        using SqliteConnection sourceConnection =
            new($"Data Source={sourceDatabasePath};Mode=ReadOnly;Pooling=False");

        SqliteConnection memoryConnection =
            new("Data Source=:memory:");

        try
        {
            sourceConnection.Open();
            memoryConnection.Open();

            sourceConnection.BackupDatabase(memoryConnection);

            return memoryConnection;
        }
        catch
        {
            memoryConnection.Dispose();
            throw;
        }
    }
}
