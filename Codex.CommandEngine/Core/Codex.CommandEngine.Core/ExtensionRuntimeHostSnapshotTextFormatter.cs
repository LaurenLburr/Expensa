using System.Text;

namespace Codex.CommandEngine.Core;

public static class ExtensionRuntimeHostSnapshotTextFormatter
{
    public static string Format(
        ExtensionRuntimeHostSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        StringBuilder builder = new();

        builder.AppendLine("Extension Runtime Host");
        builder.AppendLine("======================");
        builder.AppendLine();
        builder.AppendLine($"State: {snapshot.State}");
        builder.AppendLine($"Registered commands: {snapshot.RegisteredCommandCount}");
        builder.AppendLine($"Has errors: {snapshot.Diagnostics.HasErrors}");
        builder.AppendLine($"Has warnings: {snapshot.Diagnostics.HasWarnings}");

        if (snapshot.Commands.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("Commands");
            builder.AppendLine("--------");

            foreach (RuntimeCommandDescriptor command in snapshot.Commands)
            {
                builder.AppendLine();
                builder.AppendLine($"Command: {command.CommandName}");
                builder.AppendLine($"Display Name: {command.DisplayName}");
                builder.AppendLine($"Category: {command.Category}");
                builder.AppendLine($"Enabled: {command.IsEnabled}");
            }
        }

        if (snapshot.Diagnostics.Issues.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("Diagnostics");
            builder.AppendLine("-----------");

            foreach (RuntimeBootstrapIssue issue in snapshot.Diagnostics.Issues)
            {
                builder.AppendLine();
                builder.AppendLine($"Severity: {issue.Severity}");

                if (!string.IsNullOrWhiteSpace(issue.CommandName))
                {
                    builder.AppendLine($"Command: {issue.CommandName}");
                }

                builder.AppendLine($"Message: {issue.Message}");
            }
        }

        if (snapshot.Commands.Count == 0 && snapshot.Diagnostics.Issues.Count == 0)
        {
            builder.AppendLine();
            builder.AppendLine("No commands or diagnostics are available.");
        }

        return builder.ToString();
    }
}
