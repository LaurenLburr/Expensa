using System.Text;

namespace Codex.CommandEngine.Core;

public static class RuntimeBootstrapDiagnosticsTextFormatter
{
    public static string Format(
        CommandEngineRuntimeBootstrapResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return Format(
            RuntimeBootstrapDiagnosticViewModelFactory.Create(result));
    }

    public static string Format(
        ExtensionCommandRuntimeBootstrapResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return Format(
            RuntimeBootstrapDiagnosticViewModelFactory.Create(result));
    }

    public static string Format(
        RuntimeBootstrapDiagnosticViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        StringBuilder builder = new();

        builder.AppendLine("CommandEngine Runtime Bootstrap");
        builder.AppendLine("===============================");
        builder.AppendLine();
        builder.AppendLine($"Registered commands: {viewModel.RegisteredCommandCount}");
        builder.AppendLine($"Issues: {viewModel.IssueCount}");
        builder.AppendLine($"Has errors: {viewModel.HasErrors}");
        builder.AppendLine($"Has warnings: {viewModel.HasWarnings}");

        if (viewModel.Lines.Count == 0)
        {
            builder.AppendLine();
            builder.AppendLine("No bootstrap issues found.");
            return builder.ToString();
        }

        builder.AppendLine();
        builder.AppendLine("Issues");
        builder.AppendLine("------");

        foreach (RuntimeBootstrapDiagnosticLine line in viewModel.Lines)
        {
            builder.AppendLine();
            builder.AppendLine($"Severity: {line.Severity}");

            if (!string.IsNullOrWhiteSpace(line.CommandName))
            {
                builder.AppendLine($"Command: {line.CommandName}");
            }

            builder.AppendLine($"Message: {line.Message}");
        }

        return builder.ToString();
    }
}
