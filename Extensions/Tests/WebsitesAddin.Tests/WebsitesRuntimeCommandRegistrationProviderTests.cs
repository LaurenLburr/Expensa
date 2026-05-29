using WebsitesAddin;
using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsitesRuntimeCommandRegistrationProviderTests
{
    [Fact]
    public void GetRegistrations_ReturnsWebsitesLoadRegistration()
    {
        WebsitesRuntimeCommandRegistrationProvider provider = new();

        var registrations =
            provider.GetRegistrations();

        Assert.Single(registrations);
        Assert.Equal("websites.load", registrations[0].Handler.CommandName);
        Assert.Equal("Load Websites", registrations[0].DisplayName);
        Assert.Equal("Websites", registrations[0].Category);
    }
}
