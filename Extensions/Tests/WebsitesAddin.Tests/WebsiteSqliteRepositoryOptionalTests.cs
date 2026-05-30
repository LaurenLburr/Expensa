using WebsitesAddin;
using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsiteSqliteRepositoryOptionalTests
{
    [Fact]
    public void DevDatabasePathResolver_ReturnsDevDatabasePath()
    {
        string path =
            WebsiteDatabasePathResolver.ResolveDevDatabasePath();

        Assert.EndsWith(
            Path.Combine("DevDatabase", "websites.dev.db"),
            path,
            StringComparison.OrdinalIgnoreCase);
    }
}
