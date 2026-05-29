using Codex.CommandEngine.Core;
using WebsitesAddin;
using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsiteLoadRuntimeSmokeRunnerTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsSucceededOutput()
    {
        WebsiteLoadRuntimeSmokeRunner runner = new();

        CommandExecutionResult result =
            await runner.ExecuteAsync(
                new WebsiteLoadRequest
                {
                    SearchText = "bank",
                    MaximumRows = 25
                });

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
        Assert.Contains("Banking", result.OutputJson);
    }

    [Fact]
    public async Task ExecuteAndSaveAsync_WritesOutputFile()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                "WebsiteLoadRuntimeSmokeRunnerTests",
                Guid.NewGuid().ToString("N"));

        string outputPath =
            Path.Combine(folder, "websites-load-smoke.json");

        WebsiteLoadRuntimeSmokeRunner runner = new();

        CommandExecutionResult result =
            await runner.ExecuteAndSaveAsync(
                new WebsiteLoadRequest
                {
                    MaximumRows = 25
                },
                outputPath);

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
        Assert.True(File.Exists(outputPath));
        Assert.Contains("Demo Bank", File.ReadAllText(outputPath));
    }
}
