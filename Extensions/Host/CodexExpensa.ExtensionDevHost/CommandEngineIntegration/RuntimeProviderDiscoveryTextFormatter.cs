using System.Text;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class RuntimeProviderDiscoveryTextFormatter
{
    public static string Format(
        RuntimeProviderDiscoveryResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        StringBuilder builder = new();

        builder.AppendLine("Runtime Provider Discovery");
        builder.AppendLine("==========================");
        builder.AppendLine();
        builder.AppendLine($"Providers found: {result.Providers.Count}");
        builder.AppendLine($"Issues: {result.Issues.Count}");

        if (result.Issues.Count == 0)
        {
            builder.AppendLine();
            builder.AppendLine("No provider discovery issues found.");
            return builder.ToString();
        }

        builder.AppendLine();
        builder.AppendLine("Issues");
        builder.AppendLine("------");

        foreach (RuntimeProviderDiscoveryIssue issue in result.Issues)
        {
            builder.AppendLine();
            builder.AppendLine($"Source: {issue.Source}");
            builder.AppendLine($"Message: {issue.Message}");
        }

        return builder.ToString();
    }
}
