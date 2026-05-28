using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class JsonExtensionRuntimeDashboardSettingsStore : IExtensionRuntimeDashboardSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private readonly string _settingsPath;

    public JsonExtensionRuntimeDashboardSettingsStore()
        : this(GetDefaultSettingsPath())
    {
    }

    public JsonExtensionRuntimeDashboardSettingsStore(
        string settingsPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(settingsPath);

        _settingsPath = settingsPath;
    }

    public ExtensionRuntimeDashboardSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return new ExtensionRuntimeDashboardSettings();
            }

            string json =
                File.ReadAllText(_settingsPath);

            return JsonSerializer.Deserialize<ExtensionRuntimeDashboardSettings>(json, JsonOptions)
                ?? new ExtensionRuntimeDashboardSettings();
        }
        catch
        {
            return new ExtensionRuntimeDashboardSettings();
        }
    }

    public void Save(
        ExtensionRuntimeDashboardSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        string? folder =
            Path.GetDirectoryName(_settingsPath);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string json =
            JsonSerializer.Serialize(settings, JsonOptions);

        File.WriteAllText(_settingsPath, json);
    }

    private static string GetDefaultSettingsPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "CommandEngineDashboardSettings.json");
    }
}
