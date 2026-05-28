using Codex.CommandEngine.Core;
using WebsitesAddin;
using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsitesAddinCommandProviderTests
{
    [Fact]
    public void GetRegistrations_ReturnsSmokeCommandRegistration()
    {
        WebsitesAddinCommandProvider provider = new();

        IReadOnlyList<RuntimeCommandRegistration> registrations =
            provider.GetRegistrations();

        Assert.Single(registrations);
        Assert.Equal(WebsitesAddinSmokeCommandHandler.RegisteredCommandName, registrations[0].Handler.CommandName);
        Assert.Equal("Websites Add-in Smoke Test", registrations[0].DisplayName);
        Assert.Equal("Websites", registrations[0].Category);
    }

    [Fact]
    public async Task SmokeCommand_ExecutesSuccessfully()
    {
        WebsitesAddinSmokeCommandHandler handler = new();

        CommandExecutionResult result =
            await handler.ExecuteAsync(
                new CommandExecutionRequest
                {
                    CommandName = WebsitesAddinSmokeCommandHandler.RegisteredCommandName,
                    CorrelationId = "websites-001"
                },
                new CommandExecutionContext());

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
        Assert.Equal("websites-001", result.CorrelationId);
        Assert.Contains("Websites add-in smoke test completed", result.Message);
    }
}
