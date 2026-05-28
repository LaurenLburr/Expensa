using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.Services;

public sealed class DatabaseConnectionSettingsStore
{
    private readonly string _settingsPath;

    public DatabaseConnectionSettingsStore()
    {
        string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Expensa", "Extensions");
        Directory.CreateDirectory(folder);
        _settingsPath = Path.Combine(folder, "DatabaseConnectionSettings.json");
    }

    public string SettingsPath => _settingsPath;
    public string SettingsFolder => Path.GetDirectoryName(_settingsPath) ?? AppContext.BaseDirectory;

    public DatabaseConnectionSettings Load()
    {
        if (!File.Exists(_settingsPath))
        {
            return CreateDefaultSettings();
        }

        string json = File.ReadAllText(_settingsPath);
        return string.IsNullOrWhiteSpace(json)
            ? CreateDefaultSettings()
            : JsonSerializer.Deserialize<DatabaseConnectionSettings>(json) ?? CreateDefaultSettings();
    }

    public void Save(DatabaseConnectionSettings settings)
    {
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_settingsPath, json);
    }

    private static DatabaseConnectionSettings CreateDefaultSettings()
    {
        string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Expensa", "Extensions");

        return new DatabaseConnectionSettings
        {
            Mode = DatabaseConnectionMode.SandboxCopy,
            ProductionDatabasePath = Path.Combine(folder, "ExpensaProd.db"),
            SandboxDatabasePath = Path.Combine(folder, "ExtensionDevHost_Sandbox.db")
        };
    }
}

public sealed class DatabaseConnectionSettings
{
    public DatabaseConnectionMode Mode { get; set; } = DatabaseConnectionMode.SandboxCopy;
    public string ProductionDatabasePath { get; set; } = string.Empty;
    public string SandboxDatabasePath { get; set; } = string.Empty;
}

public enum DatabaseConnectionMode
{
    SandboxCopy,
    Production,
    InMemory
}