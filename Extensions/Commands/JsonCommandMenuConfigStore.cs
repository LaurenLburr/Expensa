using System.IO;
using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class JsonCommandMenuConfigStore : ICommandMenuConfigStore
{
    public JsonCommandMenuConfigStore(string filePath)
    {
        System.ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        FilePath = filePath;
    }

    public string FilePath { get; }

    public CommandMenuConfigDocument Load()
    {
        if (!File.Exists(FilePath))
        {
            return new CommandMenuConfigDocument();
        }

        string json = File.ReadAllText(FilePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new CommandMenuConfigDocument();
        }

        return JsonSerializer.Deserialize<CommandMenuConfigDocument>(json) ?? new CommandMenuConfigDocument();
    }

    public void Save(CommandMenuConfigDocument document)
    {
        System.ArgumentNullException.ThrowIfNull(document);

        string? folder = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string json = JsonSerializer.Serialize(document, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(FilePath, json);
    }
}
