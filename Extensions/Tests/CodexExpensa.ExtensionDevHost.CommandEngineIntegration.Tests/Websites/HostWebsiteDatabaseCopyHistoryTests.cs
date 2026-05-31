using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteDatabaseCopyHistoryTests
{
    [Fact]
    public void Add_InsertsNewestRecordFirst()
    {
        HostWebsiteDatabaseCopyHistory history = new();

        history.Add(
            new HostWebsiteDatabaseCopyRecord
            {
                SourceLabel = "first",
                DatabasePath = @"C:\Temp\first.db"
            });

        history.Add(
            new HostWebsiteDatabaseCopyRecord
            {
                SourceLabel = "second",
                DatabasePath = @"C:\Temp\second.db"
            });

        Assert.Equal("second", history.Records[0].SourceLabel);
        Assert.Equal("first", history.Records[1].SourceLabel);
    }
}
