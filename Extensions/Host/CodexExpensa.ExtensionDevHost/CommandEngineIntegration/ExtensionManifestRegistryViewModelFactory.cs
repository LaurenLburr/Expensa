namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class ExtensionManifestRegistryViewModelFactory
{
    public static ExtensionManifestRegistryViewModel Create(
        ExtensionManifestRegistrySnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        IReadOnlyList<ExtensionManifestRecord> records =
            snapshot.Records
                .OrderBy(static record => record.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(static record => record.ExtensionId, StringComparer.OrdinalIgnoreCase)
                .ThenBy(static record => record.ManifestPath, StringComparer.OrdinalIgnoreCase)
                .ToList();

        return new ExtensionManifestRegistryViewModel
        {
            TotalCount = records.Count,
            EnabledCount = snapshot.EnabledCount,
            DisabledCount = snapshot.DisabledCount,
            DuplicateCount = snapshot.DuplicateCount,
            ErrorCount = snapshot.Errors.Count,
            Summary = BuildSummary(snapshot),
            Records = records,
            Errors = snapshot.Errors
        };
    }

    private static string BuildSummary(
        ExtensionManifestRegistrySnapshot snapshot)
    {
        if (snapshot.Records.Count == 0 && snapshot.Errors.Count == 0)
        {
            return "No extension manifests have been discovered.";
        }

        return
            $"Discovered {snapshot.Records.Count} manifest(s): " +
            $"{snapshot.EnabledCount} enabled, " +
            $"{snapshot.DisabledCount} disabled, " +
            $"{snapshot.DuplicateCount} duplicate row(s), " +
            $"{snapshot.Errors.Count} error(s).";
    }
}
