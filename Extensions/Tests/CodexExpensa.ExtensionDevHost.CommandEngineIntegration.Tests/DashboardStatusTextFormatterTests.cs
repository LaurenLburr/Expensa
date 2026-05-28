using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class DashboardStatusTextFormatterTests
{
    [Fact]
    public void Format_IncludesStatusFields()
    {
        DashboardStatusViewModel status = new()
        {
            Mode = "History",
            RuntimeStatus = "Started",
            RowCount = 3,
            Recursive = true,
            ShowingDuplicates = false
        };

        string text = DashboardStatusTextFormatter.Format(status);

        Assert.Contains("History", text);
        Assert.Contains("Started", text);
        Assert.Contains("Rows: 3", text);
        Assert.Contains("Recursive: True", text);
        Assert.Contains("Duplicates: False", text);
    }
}
