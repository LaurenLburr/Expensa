using System.Text;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public static class ExtensionTreeLoadSummaryFormatter
{
    public static string Format(
        ExtensionTreeLoadSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        StringBuilder builder = new();

        builder.AppendLine(summary.ToDisplayText());
        builder.AppendLine();

        foreach (ExtensionTreeLoadResult result in summary.Results)
        {
            builder.AppendLine(
                $"{result.SortOrder}: {result.DisplayName} [{result.AddinId}] - {(result.Succeeded ? "Succeeded" : "Failed")}");

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                builder.AppendLine($"  {result.Message}");
            }
        }

        return builder.ToString();
    }
}
