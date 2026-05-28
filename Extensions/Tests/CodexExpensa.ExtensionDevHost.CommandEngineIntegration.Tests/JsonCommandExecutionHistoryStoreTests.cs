using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class JsonCommandExecutionHistoryStoreTests
{
    [Fact]
    public void AddThenListAll_PersistsRecord()
    {
        string path =
            Path.Combine(
                Path.GetTempPath(),
                "CommandExecutionHistoryTests",
                Guid.NewGuid().ToString("N"),
                "history.json");

        JsonCommandExecutionHistoryStore store = new(path);

        store.Add(new CommandExecutionHistoryRecord
        {
            StartedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            CompletedUtc = DateTimeOffset.Parse("2026-01-01T00:00:01Z"),
            CommandName = "test.command",
            Status = "Succeeded",
            Message = "Done"
        });

        JsonCommandExecutionHistoryStore reloaded = new(path);

        IReadOnlyList<CommandExecutionHistoryRecord> records =
            reloaded.ListAll();

        Assert.Single(records);
        Assert.Equal("test.command", records[0].CommandName);
        Assert.Equal("Succeeded", records[0].Status);
    }

    [Fact]
    public void Add_WhenAboveMaximum_TrimsOldestRecords()
    {
        string path =
            Path.Combine(
                Path.GetTempPath(),
                "CommandExecutionHistoryTests",
                Guid.NewGuid().ToString("N"),
                "history.json");

        JsonCommandExecutionHistoryStore store = new(path, maximumRecordCount: 1);

        store.Add(new CommandExecutionHistoryRecord
        {
            StartedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            CompletedUtc = DateTimeOffset.Parse("2026-01-01T00:00:01Z"),
            CommandName = "older"
        });

        store.Add(new CommandExecutionHistoryRecord
        {
            StartedUtc = DateTimeOffset.Parse("2026-01-02T00:00:00Z"),
            CompletedUtc = DateTimeOffset.Parse("2026-01-02T00:00:01Z"),
            CommandName = "newer"
        });

        IReadOnlyList<CommandExecutionHistoryRecord> records =
            store.ListAll();

        Assert.Single(records);
        Assert.Equal("newer", records[0].CommandName);
    }

    [Fact]
    public void Clear_RemovesPersistedRecords()
    {
        string path =
            Path.Combine(
                Path.GetTempPath(),
                "CommandExecutionHistoryTests",
                Guid.NewGuid().ToString("N"),
                "history.json");

        JsonCommandExecutionHistoryStore store = new(path);

        store.Add(new CommandExecutionHistoryRecord
        {
            StartedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            CompletedUtc = DateTimeOffset.Parse("2026-01-01T00:00:01Z"),
            CommandName = "test.command"
        });

        store.Clear();

        Assert.Empty(store.ListAll());
    }
}
