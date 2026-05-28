namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class ExtensionManifestRegistryFilter
{
    public static IReadOnlyList<ExtensionManifestRecord> PreferSingleRecordPerExtension(
        IReadOnlyList<ExtensionManifestRecord> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        return records
            .GroupBy(static record => record.ExtensionId, StringComparer.OrdinalIgnoreCase)
            .Select(static group => group
                .OrderBy(static record => GetPreferenceRank(record.ManifestPath))
                .ThenBy(static record => record.ManifestPath, StringComparer.OrdinalIgnoreCase)
                .First())
            .OrderBy(static record => record.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static record => record.ExtensionId, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static int GetPreferenceRank(string manifestPath)
    {
        if (manifestPath.Contains("\\Debug\\", StringComparison.OrdinalIgnoreCase) ||
            manifestPath.Contains("/Debug/", StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        if (manifestPath.Contains("\\Release\\", StringComparison.OrdinalIgnoreCase) ||
            manifestPath.Contains("/Release/", StringComparison.OrdinalIgnoreCase))
        {
            return 1;
        }

        return 2;
    }
}
