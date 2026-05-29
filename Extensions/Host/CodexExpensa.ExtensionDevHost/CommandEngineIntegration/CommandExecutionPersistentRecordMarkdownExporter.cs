using System.Text;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionPersistentRecordMarkdownExporter
{
    public static void Export(
        string filePath,
        IReadOnlyList<CommandExecutionPersistentRecord> records)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(records);

        string? folder =
            Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllText(filePath, BuildMarkdown(records));
    }

    public static string BuildMarkdown(
        IReadOnlyList<CommandExecutionPersistentRecord> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        StringBuilder builder = new();

        builder.AppendLine("# Persisted Command Execution Report");
        builder.AppendLine();
        builder.AppendLine($"Generated UTC: {DateTimeOffset.UtcNow:u}");
        builder.AppendLine($"Record count: {records.Count}");
        builder.AppendLine();
        builder.AppendLine("| Created UTC | Source | Command | Status | Message |");
        builder.AppendLine("|---|---|---|---|---|");

        foreach (CommandExecutionPersistentRecord record in records)
        {
            builder.AppendLine(
                $"| {Escape(record.CreatedUtc.ToString("u"))} | {Escape(record.SourceKind)} | {Escape(record.CommandName)} | {Escape(record.Status)} | {Escape(record.Message)} |");
        }

        builder.AppendLine();

        foreach (CommandExecutionPersistentRecord record in records)
        {
            builder.AppendLine($"## {record.CommandName}");
            builder.AppendLine();
            builder.AppendLine($"- ExecutionId: `{record.ExecutionId}`");
            builder.AppendLine($"- SourceKind: `{record.SourceKind}`");
            builder.AppendLine($"- Status: `{record.Status}`");
            builder.AppendLine($"- CreatedUtc: `{record.CreatedUtc:u}`");
            builder.AppendLine($"- StartedUtc: `{record.StartedUtc:u}`");
            builder.AppendLine($"- CompletedUtc: `{record.CompletedUtc:u}`");
            builder.AppendLine($"- CorrelationId: `{record.CorrelationId}`");
            builder.AppendLine();
            builder.AppendLine("### Message");
            builder.AppendLine();
            builder.AppendLine(string.IsNullOrWhiteSpace(record.Message) ? "_No message._" : record.Message);
            builder.AppendLine();
            builder.AppendLine("### Parameters");
            builder.AppendLine();
            builder.AppendLine("```json");
            builder.AppendLine(string.IsNullOrWhiteSpace(record.ParameterJson) ? "{}" : record.ParameterJson);
            builder.AppendLine("```");
            builder.AppendLine();
            builder.AppendLine("### Output");
            builder.AppendLine();
            builder.AppendLine("```json");
            builder.AppendLine(string.IsNullOrWhiteSpace(record.OutputJson) ? "{}" : record.OutputJson);
            builder.AppendLine("```");
            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static string Escape(
        string value)
    {
        return value
            .Replace("|", "\\|", StringComparison.Ordinal)
            .Replace(Environment.NewLine, " ", StringComparison.Ordinal);
    }
}
