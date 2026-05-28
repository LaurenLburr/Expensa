using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandPalettePreviewTextFormatterTests
{
    [Fact]
    public void Format_WhenNull_ReturnsNoSelectionMessage()
    {
        string text =
            CommandPalettePreviewTextFormatter.Format(null);

        Assert.Contains("No command selected", text);
    }

    [Fact]
    public void Format_WhenItemProvided_IncludesTemplate()
    {
        string text =
            CommandPalettePreviewTextFormatter.Format(new CommandPaletteItem
            {
                CommandName = "test.command",
                DisplayName = "Test Command",
                Category = "Tests",
                IsEnabled = true,
                Description = "Does test things.",
                ParameterTemplateJson = "{\"value\":1}",
                Notes = "No notes."
            });

        Assert.Contains("test.command", text);
        Assert.Contains("Does test things.", text);
        Assert.Contains("{\"value\":1}", text);
    }
}
