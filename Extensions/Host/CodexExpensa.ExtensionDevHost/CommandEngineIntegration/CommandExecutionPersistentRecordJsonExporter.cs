using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionPersistentRecordJsonExporter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static void Export(
        string filePath,
        IReadOnlyList<CommandExecutionPersistentRecord> records)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(records);

        string? folder =
            Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllText(
            filePath,
            JsonSerializer.Serialize(records, JsonOptions));
    }
}
