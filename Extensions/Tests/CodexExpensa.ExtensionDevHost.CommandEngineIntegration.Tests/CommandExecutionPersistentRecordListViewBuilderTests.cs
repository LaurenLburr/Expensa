using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using System.Windows.Forms;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionPersistentRecordListViewBuilderTests
{
    [Fact]
    public void Populate_StoresPersistentRecordInTag()
    {
        using ListView listView = new();

        CommandExecutionPersistentRecord record = new()
        {
            ExecutionId = "abc",
            SourceKind = "History",
            CommandName = "test.command",
            Status = "Completed",
            CreatedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            CompletedUtc = DateTimeOffset.Parse("2026-01-01T00:00:01Z"),
            Message = "Done."
        };

        CommandExecutionPersistentRecordListViewBuilder.ConfigureColumns(listView);
        CommandExecutionPersistentRecordListViewBuilder.Populate(listView, [record]);

        Assert.Single(listView.Items);
        Assert.Same(record, listView.Items[0].Tag);
        Assert.Equal("test.command", listView.Items[0].SubItems[3].Text);
    }

    [Fact]
    public void ConfigureColumns_AddsExpectedColumns()
    {
        using ListView listView = new();

        CommandExecutionPersistentRecordListViewBuilder.ConfigureColumns(listView);

        Assert.Equal(6, listView.Columns.Count);
        Assert.Equal("Created UTC", listView.Columns[0].Text);
        Assert.Equal("Message", listView.Columns[5].Text);
    }
}
