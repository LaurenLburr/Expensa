using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class CommandMetadataRegistry
{
    private readonly Dictionary<string, CommandMetadataRecord> _records =
        new(StringComparer.OrdinalIgnoreCase);

    public void AddOrUpdate(CommandMetadataRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        ArgumentException.ThrowIfNullOrWhiteSpace(record.CommandName);

        _records[record.CommandName] = record;
    }

    public void AddRange(IEnumerable<CommandMetadataRecord> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        foreach (CommandMetadataRecord record in records)
        {
            AddOrUpdate(record);
        }
    }

    public CommandMetadataRecord? Find(string commandName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

        return _records.TryGetValue(commandName, out CommandMetadataRecord? record)
            ? record
            : null;
    }

    public IReadOnlyList<CommandMetadataRecord> ListAll()
    {
        return _records.Values
            .OrderBy(static record => record.Category, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static record => record.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static record => record.CommandName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
