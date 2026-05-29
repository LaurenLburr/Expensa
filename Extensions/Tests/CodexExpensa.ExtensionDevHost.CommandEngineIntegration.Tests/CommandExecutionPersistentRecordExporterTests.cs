using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionPersistentRecordExporterTests
{
    [Fact]
    public void MarkdownExporter_BuildMarkdown_IncludesCommand()
    {
        string markdown =
            CommandExecutionPersistentRecordMarkdownExporter.BuildMarkdown(
                [
                    new CommandExecutionPersistentRecord
                    {
                        ExecutionId = "abc",
                        SourceKind = "History",
                        CommandName = "test.command",
                        Status = "Completed",
                        Message = "Done.",
                        CreatedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
                        ParameterJson = "{}",
                        OutputJson = "{\"ok\":true}"
                    }
                ]);

        Assert.Contains("# Persisted Command Execution Report", markdown);
        Assert.Contains("test.command", markdown);
        Assert.Contains("```json", markdown);
    }

    [Fact]
    public void JsonExporter_Export_WritesFile()
    {
        string folder =
            Path.Combine(Path.GetTempPath(), "PersistentExecutionExporterTests", Guid.NewGuid().ToString("N"));

        string filePath =
            Path.Combine(folder, "export.json");

        CommandExecutionPersistentRecordJsonExporter.Export(
            filePath,
            [
                new CommandExecutionPersistentRecord
                {
                    ExecutionId = "abc",
                    SourceKind = "Queue",
                    CommandName = "test.command",
                    Status = "Completed",
                    CreatedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
                }
            ]);

        Assert.True(File.Exists(filePath));
        Assert.Contains("test.command", File.ReadAllText(filePath));
    }
}
