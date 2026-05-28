using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using System.Windows.Forms;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionHistoryReplayTests
{
    [Fact]
    public void HistoryRecord_CanStoreReplayParameters()
    {
        CommandExecutionHistoryRecord record = new()
        {
            StartedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            CompletedUtc = DateTimeOffset.Parse("2026-01-01T00:00:01Z"),
            CommandName = "test.command",
            ParameterJson = """
            {
              "value": 42
            }
            """
        };

        IReadOnlyDictionary<string, object?> parameters =
            CommandParameterJsonParser.Parse(record.ParameterJson);

        Assert.Equal(42, parameters["value"]);
    }

    [Fact]
    public void ListViewBuilder_Populate_StoresHistoryRecordInTag()
    {
        using ListView listView = new();

        CommandExecutionHistoryRecord record = new()
        {
            StartedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            CompletedUtc = DateTimeOffset.Parse("2026-01-01T00:00:01Z"),
            CommandName = "test.command",
            Status = "Succeeded"
        };

        CommandExecutionHistoryListViewBuilder.ConfigureColumns(listView);
        CommandExecutionHistoryListViewBuilder.Populate(listView, [record]);

        Assert.Single(listView.Items);
        Assert.Same(record, listView.Items[0].Tag);
    }
}
