using Codex.CommandEngine.Abstractions;
using WebsitesAddin;
using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsiteLoadCommandHandlerTests
{
    [Fact]
    public void Definition_UsesCommandName()
    {
        WebsiteLoadCommandHandler handler = new();

        Assert.Equal("websites.load", handler.Definition.CommandName);
        Assert.Equal("Load Websites", handler.Definition.DisplayName);
    }

    [Fact]
    public async Task ValidateAsync_WhenCommandMatches_ReturnsSuccess()
    {
        WebsiteLoadCommandHandler handler = new();

        CommandExecutionResult result =
            await handler.ValidateAsync(
                new CommandRequest
                {
                    CommandName = "websites.load"
                });

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsSerializedResult()
    {
        WebsiteLoadCommandHandler handler = new();

        CommandExecutionResult result =
            await handler.ExecuteAsync(
                new CommandRequest
                {
                    CommandName = "websites.load",
                    Parameters =
                    {
                        ["searchText"] = "bank"
                    }
                });

        Assert.True(result.Succeeded);
        Assert.Contains("Banking", result.Message);
    }
}
