using System.Data;
using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class DataRowExtensionsTests
{
    [Fact]
    public void GetStringOrDefault_WhenColumnMissing_ReturnsDefault()
    {
        DataTable table = new();
        table.Columns.Add("Name", typeof(string));

        DataRow row = table.NewRow();
        row["Name"] = "Example";
        table.Rows.Add(row);

        Assert.Equal("fallback", row.GetStringOrDefault("Missing", "fallback"));
    }

    [Fact]
    public void GetRequiredString_WhenColumnExists_ReturnsString()
    {
        DataTable table = new();
        table.Columns.Add("Name", typeof(string));

        DataRow row = table.NewRow();
        row["Name"] = "Example";
        table.Rows.Add(row);

        Assert.Equal("Example", row.GetRequiredString("Name"));
    }

    [Fact]
    public void GetBooleanOrDefault_WhenIntegerOne_ReturnsTrue()
    {
        DataTable table = new();
        table.Columns.Add("IsEnabled", typeof(int));

        DataRow row = table.NewRow();
        row["IsEnabled"] = 1;
        table.Rows.Add(row);

        Assert.True(row.GetBooleanOrDefault("IsEnabled"));
    }
}
