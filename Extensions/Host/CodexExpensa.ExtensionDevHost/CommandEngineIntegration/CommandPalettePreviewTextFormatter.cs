using System.Text;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandPalettePreviewTextFormatter
{
    public static string Format(
        CommandPaletteItem? item)
    {
        if (item is null)
        {
            return "No command selected.";
        }

        StringBuilder builder = new();

        builder.AppendLine("Command Preview");
        builder.AppendLine("===============");
        builder.AppendLine();
        builder.AppendLine($"CommandName: {item.CommandName}");
        builder.AppendLine($"DisplayName: {item.DisplayName}");
        builder.AppendLine($"Category: {item.Category}");
        builder.AppendLine($"Enabled: {item.IsEnabled}");

        if (!string.IsNullOrWhiteSpace(item.Description))
        {
            builder.AppendLine();
            builder.AppendLine("Description");
            builder.AppendLine("-----------");
            builder.AppendLine(item.Description);
        }

        if (!string.IsNullOrWhiteSpace(item.ParameterTemplateJson))
        {
            builder.AppendLine();
            builder.AppendLine("Parameter Template");
            builder.AppendLine("------------------");
            builder.AppendLine(item.ParameterTemplateJson);
        }

        if (!string.IsNullOrWhiteSpace(item.Notes))
        {
            builder.AppendLine();
            builder.AppendLine("Notes");
            builder.AppendLine("-----");
            builder.AppendLine(item.Notes);
        }

        return builder.ToString();
    }
}
