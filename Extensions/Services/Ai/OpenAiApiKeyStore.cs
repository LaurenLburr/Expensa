using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.Services.Ai;

public sealed class OpenAiApiKeyStore
{
    private readonly string _settingsPath;

    public OpenAiApiKeyStore()
    {
        string settingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions");

        Directory.CreateDirectory(settingsFolder);
        _settingsPath = Path.Combine(settingsFolder, "OpenAiSettings.json");
    }

    public string SettingsPath => _settingsPath;

    public string GetApiKey()
    {
        SettingsDocument doc = Load();
        return doc.ApiKey.Trim();
    }

    public string GetKeyName()
    {
        SettingsDocument doc = Load();
        return doc.KeyName.Trim();
    }

    public void SaveApiKey(string apiKey)
    {
        SettingsDocument current = Load();

        Save(new SettingsDocument
        {
            KeyName = current.KeyName.Trim(),
            ApiKey = apiKey.Trim()
        });
    }

    public void SaveApiKey(string keyName, string apiKey)
    {
        Save(new SettingsDocument
        {
            KeyName = keyName.Trim(),
            ApiKey = apiKey.Trim()
        });
    }

    private SettingsDocument Load()
    {
        if (!File.Exists(_settingsPath))
        {
            return new SettingsDocument();
        }

        string json = File.ReadAllText(_settingsPath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new SettingsDocument();
        }

        return JsonSerializer.Deserialize<SettingsDocument>(json) ?? new SettingsDocument();
    }

    private void Save(SettingsDocument document)
    {
        string json = JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_settingsPath, json);
    }

    private sealed class SettingsDocument
    {
        public string KeyName { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;
    }
}
