using System.Text;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class ExtensionManifestRegistryTextFormatter
{
    public static string Format(ExtensionManifestRegistryViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        StringBuilder builder = new();

        builder.AppendLine("Extension Manifest Registry");
        builder.AppendLine("===========================");
        builder.AppendLine();
        builder.AppendLine(viewModel.Summary);

        foreach (ExtensionManifestRecord record in viewModel.Records)
        {
            builder.AppendLine();
            builder.AppendLine($"ExtensionId: {record.ExtensionId}");
            builder.AppendLine($"DisplayName: {record.DisplayName}");
            builder.AppendLine($"Version: {record.Version}");
            builder.AppendLine($"Enabled: {record.Enabled}");
            builder.AppendLine($"Duplicate: {record.IsDuplicate}");
            builder.AppendLine($"DuplicateCount: {record.DuplicateCount}");
            builder.AppendLine($"AssemblyFile: {record.AssemblyFile}");
            builder.AppendLine($"ProviderType: {record.ProviderType}");
            builder.AppendLine($"ManifestFolder: {record.ManifestFolder}");
            builder.AppendLine($"ManifestPath: {record.ManifestPath}");
        }

        if (viewModel.Errors.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("Errors");
            builder.AppendLine("------");

            foreach (string error in viewModel.Errors)
            {
                builder.AppendLine();
                builder.AppendLine(error);
            }
        }

        return builder.ToString();
    }
}
