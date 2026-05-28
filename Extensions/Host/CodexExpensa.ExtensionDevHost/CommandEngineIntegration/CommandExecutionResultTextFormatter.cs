using Codex.CommandEngine.Core;
using System.Text;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionResultTextFormatter
{
    public static string Format(CommandExecutionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        StringBuilder builder = new();

        builder.AppendLine("Command Execution Result");
        builder.AppendLine("========================");
        builder.AppendLine();
        builder.AppendLine($"CommandName: {result.CommandName}");
        builder.AppendLine($"CorrelationId: {result.CorrelationId}");
        builder.AppendLine($"Status: {result.Status}");
        builder.AppendLine($"Message: {result.Message}");

        if (!string.IsNullOrWhiteSpace(result.OutputJson))
        {
            builder.AppendLine();
            builder.AppendLine("OutputJson");
            builder.AppendLine("----------");
            builder.AppendLine(result.OutputJson);
        }

        return builder.ToString();
    }
}
