using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteDatabaseDisplayNameFormatterTests
{
    [Theory]
    [InlineData("websitesaddin.db", 12, "websitesa...")]
    [InlineData("short.db", 12, "short.db")]
    [InlineData("", 12, "")]
    public void Ellipsize_ReturnsExpectedText(
        string value,
        int maximumLength,
        string expected)
    {
        string result =
            HostWebsiteDatabaseDisplayNameFormatter.Ellipsize(
                value,
                maximumLength);

        Assert.Equal(expected, result);
    }
}
