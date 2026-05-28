using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class JsonCommandExecutionHistoryStore : ICommandExecutionHistoryFileStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private readonly int _maximumRecordCount;

    public JsonCommandExecutionHistoryStore()
        : this(GetDefaultHistoryPath(), maximumRecordCount: 500)
    {
    }

    public JsonCommandExecutionHistoryStore(string historyPath, int maximumRecordCount = 500)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(historyPath);

        if (maximumRecordCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumRecordCount), maximumRecordCount, "Maximum record count must be at least 1.");
        }

        HistoryPath = historyPath;
        _maximumRecordCount = maximumRecordCount;
    }

    public string HistoryPath { get; }

    public void Add(CommandExecutionHistoryRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        List<CommandExecutionHistoryRecord> records = LoadInternal();
        records.Add(record);

        records = records
            .OrderByDescending(static item => item.StartedUtc)
            .Take(_maximumRecordCount)
            .ToList();

        SaveInternal(records);
    }

    public IReadOnlyList<CommandExecutionHistoryRecord> ListAll()
    {
        return LoadInternal()
            .OrderByDescending(static record => record.StartedUtc)
            .ToList();
    }

    public void Clear()
    {
        SaveInternal([]);
    }

    private List<CommandExecutionHistoryRecord> LoadInternal()
    {
        try
        {
            if (!File.Exists(HistoryPath))
            {
                return [];
            }

            string json = File.ReadAllText(HistoryPath);

            return JsonSerializer.Deserialize<List<CommandExecutionHistoryRecord>>(json, JsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private void SaveInternal(IReadOnlyList<CommandExecutionHistoryRecord> records)
    {
        string? folder = Path.GetDirectoryName(HistoryPath);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string json = JsonSerializer.Serialize(records, JsonOptions);
        File.WriteAllText(HistoryPath, json);
    }

    public static string GetDefaultHistoryPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "CommandEngineExecutionHistory.json");
    }
}
