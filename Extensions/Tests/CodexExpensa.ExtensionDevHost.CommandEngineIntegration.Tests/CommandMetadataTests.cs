using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandMetadataTests
{
    [Fact]
    public void Registry_Find_ReturnsRegisteredMetadata()
    {
        CommandMetadataRegistry registry = new();

        registry.AddOrUpdate(new CommandMetadataRecord
        {
            CommandName = "test.command",
            DisplayName = "Test Command",
            Category = "Tests",
            ParameterTemplateJson = "{\"value\":1}"
        });

        CommandMetadataRecord? record =
            registry.Find("TEST.COMMAND");

        Assert.NotNull(record);
        Assert.Equal("Test Command", record.DisplayName);
        Assert.Equal("{\"value\":1}", record.ParameterTemplateJson);
    }

    [Fact]
    public void Formatter_WhenMetadataMissing_ReturnsHelpfulMessage()
    {
        string text =
            CommandMetadataTextFormatter.Format(null);

        Assert.Contains("No command metadata", text);
    }

    [Fact]
    public void Formatter_WhenMetadataExists_IncludesTemplate()
    {
        string text =
            CommandMetadataTextFormatter.Format(new CommandMetadataRecord
            {
                CommandName = "test.command",
                DisplayName = "Test",
                Category = "Tests",
                Description = "Description.",
                ParameterTemplateJson = "{\"value\":1}"
            });

        Assert.Contains("Command Metadata", text);
        Assert.Contains("test.command", text);
        Assert.Contains("{\"value\":1}", text);
    }
}
