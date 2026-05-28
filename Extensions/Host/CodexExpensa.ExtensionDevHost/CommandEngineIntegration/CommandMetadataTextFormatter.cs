using Codex.CommandEngine.Core;
using System.Text;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandMetadataTextFormatter
{
    public static string Format(CommandMetadataRecord? metadata)
    {
        if (metadata is null)
        {
            return "No command metadata was found for the selected command.";
        }

        StringBuilder builder = new();

        builder.AppendLine("Command Metadata");
        builder.AppendLine("================");
        builder.AppendLine();
        builder.AppendLine($"CommandName: {metadata.CommandName}");
        builder.AppendLine($"DisplayName: {metadata.DisplayName}");
        builder.AppendLine($"Category: {metadata.Category}");

        if (!string.IsNullOrWhiteSpace(metadata.Description))
        {
            builder.AppendLine();
            builder.AppendLine("Description");
            builder.AppendLine("-----------");
            builder.AppendLine(metadata.Description);
        }

        if (!string.IsNullOrWhiteSpace(metadata.ParameterTemplateJson))
        {
            builder.AppendLine();
            builder.AppendLine("Parameter Template");
            builder.AppendLine("------------------");
            builder.AppendLine(metadata.ParameterTemplateJson);
        }

        if (!string.IsNullOrWhiteSpace(metadata.Notes))
        {
            builder.AppendLine();
            builder.AppendLine("Notes");
            builder.AppendLine("-----");
            builder.AppendLine(metadata.Notes);
        }

        return builder.ToString();
    }
}
