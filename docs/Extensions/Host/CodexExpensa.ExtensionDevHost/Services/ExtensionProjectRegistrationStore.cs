using Microsoft.Data.Sqlite;
using CodexExpensa.ExtensionDevHost.Models;

namespace CodexExpensa.ExtensionDevHost.Services;

public sealed class ExtensionProjectRegistrationStore
{
    private readonly string _databasePath;

    public ExtensionProjectRegistrationStore()
    {
        string settingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions");

        Directory.CreateDirectory(settingsFolder);
        _databasePath = Path.Combine(settingsFolder, "ExtensionMgr.db");
        EnsureDatabase();
    }

    public string DatabasePath => _databasePath;

    public IReadOnlyList<ExtensionProjectRegistration> GetAll()
    {
        using SqliteConnection connection = CreateConnection();
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "SELECT [ProjectName], [RelativeBinPath], [AssemblyName], [IsEnabled], [SortOrder] " +
            "FROM [ExtensionProjectRegistration] " +
            "ORDER BY [SortOrder], [ProjectName];";

        using SqliteDataReader reader = command.ExecuteReader();

        List<ExtensionProjectRegistration> rows = new();

        while (reader.Read())
        {
            rows.Add(new ExtensionProjectRegistration
            {
                ProjectName = reader.GetString(0),
                RelativeBinPath = reader.IsDBNull(1) ? null : reader.GetString(1),
                AssemblyName = reader.GetString(2),
                IsEnabled = reader.GetInt32(3) == 1,
                SortOrder = reader.GetInt32(4)
            });
        }

        return rows;
    }

    public int GetNextSortOrder()
    {
        using SqliteConnection connection = CreateConnection();
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT COALESCE(MAX([SortOrder]), 0) + 10 FROM [ExtensionProjectRegistration];";

        object? scalar = command.ExecuteScalar();
        return scalar is long longValue ? checked((int)longValue) : 10;
    }

    public void Upsert(ExtensionProjectRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);

        using SqliteConnection connection = CreateConnection();
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "INSERT INTO [ExtensionProjectRegistration] " +
            "([ProjectName], [RelativeBinPath], [AssemblyName], [IsEnabled], [SortOrder]) " +
            "VALUES (@ProjectName, @RelativeBinPath, @AssemblyName, @IsEnabled, @SortOrder) " +
            "ON CONFLICT([ProjectName]) DO UPDATE SET " +
            "[RelativeBinPath] = excluded.[RelativeBinPath], " +
            "[AssemblyName] = excluded.[AssemblyName], " +
            "[IsEnabled] = excluded.[IsEnabled], " +
            "[SortOrder] = excluded.[SortOrder];";

        command.Parameters.AddWithValue("@ProjectName", registration.ProjectName);
        command.Parameters.AddWithValue("@RelativeBinPath", (object?)registration.RelativeBinPath ?? DBNull.Value);
        command.Parameters.AddWithValue("@AssemblyName", registration.AssemblyName);
        command.Parameters.AddWithValue("@IsEnabled", registration.IsEnabled ? 1 : 0);
        command.Parameters.AddWithValue("@SortOrder", registration.SortOrder);
        command.ExecuteNonQuery();
    }

    public void Delete(string projectName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectName);

        using SqliteConnection connection = CreateConnection();
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "DELETE FROM [ExtensionProjectRegistration] " +
            "WHERE [ProjectName] = @ProjectName;";

        command.Parameters.AddWithValue("@ProjectName", projectName.Trim());
        command.ExecuteNonQuery();
    }

    private void EnsureDatabase()
    {
        using SqliteConnection connection = CreateConnection();
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "CREATE TABLE IF NOT EXISTS [ExtensionProjectRegistration] (" +
            "[ProjectName] TEXT NOT NULL PRIMARY KEY, " +
            "[RelativeBinPath] TEXT NULL, " +
            "[AssemblyName] TEXT NOT NULL, " +
            "[IsEnabled] INTEGER NOT NULL, " +
            "[SortOrder] INTEGER NOT NULL" +
            ");";
        command.ExecuteNonQuery();
    }

    private SqliteConnection CreateConnection() => new($"Data Source={_databasePath}");
}
