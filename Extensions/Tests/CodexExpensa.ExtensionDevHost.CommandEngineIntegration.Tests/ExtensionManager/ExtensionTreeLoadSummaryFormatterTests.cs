using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionTreeLoadSummaryFormatterTests
{
    [Fact]
    public void Format_IncludesResultDetails()
    {
        ExtensionTreeLoadSummary summary = new()
        {
            Results =
            [
                new ExtensionTreeLoadResult
                {
                    AddinId = "websites",
                    DisplayName = "Websites",
                    SortOrder = 100,
                    Succeeded = true,
                    Message = "Loaded."
                }
            ]
        };

        string text =
            ExtensionTreeLoadSummaryFormatter.Format(summary);

        Assert.Contains("Websites", text);
        Assert.Contains("succeeded", text, StringComparison.OrdinalIgnoreCase);
    }
}
