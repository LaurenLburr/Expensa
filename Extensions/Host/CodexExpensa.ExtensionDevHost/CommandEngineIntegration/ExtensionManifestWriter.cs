using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class ExtensionManifestWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static void Write(
        string manifestPath,
        ExtensionManifest manifest)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestPath);
        ArgumentNullException.ThrowIfNull(manifest);

        string? folder =
            Path.GetDirectoryName(manifestPath);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string json =
            JsonSerializer.Serialize(manifest, JsonOptions);

        File.WriteAllText(manifestPath, json);
    }
}
