namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifestRegistryService : IExtensionManifestRegistryService
{
    private readonly IExtensionManifestLoader _loader;

    public ExtensionManifestRegistryService()
        : this(new ExtensionManifestLoader())
    {
    }

    public ExtensionManifestRegistryService(IExtensionManifestLoader loader)
    {
        ArgumentNullException.ThrowIfNull(loader);
        _loader = loader;
    }

    public ExtensionManifestRegistrySnapshot Discover(string folderPath, bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        if (!Directory.Exists(folderPath))
        {
            return new ExtensionManifestRegistrySnapshot
            {
                Errors = [$"Folder was not found: {folderPath}"]
            };
        }

        SearchOption option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        List<ExtensionManifestRecord> records = [];
        List<string> errors = [];

        foreach (string manifestPath in Directory.GetFiles(folderPath, "extension.json", option))
        {
            ExtensionManifestLoadResult loadResult = _loader.Load(manifestPath);

            if (!loadResult.Success || loadResult.Manifest is null)
            {
                foreach (string error in loadResult.Errors)
                {
                    errors.Add($"{manifestPath}: {error}");
                }

                continue;
            }

            records.Add(ToRecord(loadResult.Manifest, manifestPath));
        }

        return new ExtensionManifestRegistrySnapshot
        {
            Records = MarkDuplicates(records),
            Errors = errors
        };
    }

    private static IReadOnlyList<ExtensionManifestRecord> MarkDuplicates(IReadOnlyList<ExtensionManifestRecord> records)
    {
        Dictionary<string, int> counts =
            records
                .GroupBy(static record => record.ExtensionId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);

        return records
            .Select(record =>
            {
                int count = counts.TryGetValue(record.ExtensionId, out int value) ? value : 1;

                return new ExtensionManifestRecord
                {
                    ExtensionId = record.ExtensionId,
                    DisplayName = record.DisplayName,
                    Version = record.Version,
                    AssemblyFile = record.AssemblyFile,
                    ProviderType = record.ProviderType,
                    MinimumHostVersion = record.MinimumHostVersion,
                    Enabled = record.Enabled,
                    Description = record.Description,
                    ManifestFolder = record.ManifestFolder,
                    ManifestPath = record.ManifestPath,
                    DuplicateCount = count,
                    IsDuplicate = count > 1
                };
            })
            .ToList();
    }

    private static ExtensionManifestRecord ToRecord(ExtensionManifest manifest, string manifestPath)
    {
        return new ExtensionManifestRecord
        {
            ExtensionId = manifest.ExtensionId,
            DisplayName = manifest.DisplayName,
            Version = manifest.Version,
            AssemblyFile = manifest.AssemblyFile,
            ProviderType = manifest.ProviderType,
            MinimumHostVersion = manifest.MinimumHostVersion,
            Enabled = manifest.Enabled,
            Description = manifest.Description,
            ManifestFolder = Path.GetDirectoryName(manifestPath) ?? string.Empty,
            ManifestPath = manifestPath
        };
    }
}
