namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class AddinFolderStructureInitializer
{
    private readonly AddinRuntimeDatabasePathService _pathService;

    public AddinFolderStructureInitializer()
        : this(new AddinRuntimeDatabasePathService())
    {
    }

    public AddinFolderStructureInitializer(
        AddinRuntimeDatabasePathService pathService)
    {
        ArgumentNullException.ThrowIfNull(pathService);

        _pathService = pathService;
    }

    public void EnsureCreated(
        IEnumerable<string> registeredAddinIds)
    {
        ArgumentNullException.ThrowIfNull(registeredAddinIds);

        foreach (string registeredAddinId in registeredAddinIds)
        {
            EnsureCreated(registeredAddinId);
        }
    }

    public void EnsureCreated(
        string registeredAddinId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registeredAddinId);

        string moduleAddinId =
            NormalizeModuleAddinId(registeredAddinId);

        CreateParentDirectory(
            _pathService.GetDevCurrentDatabasePath(moduleAddinId));

        CreateParentDirectory(
            _pathService
                .GetRuntimeDatabaseLocation(moduleAddinId)
                .DatabasePath);

        CreateParentDirectory(
            _pathService.GetActiveRuntimePathFile(moduleAddinId));
    }

    public static string NormalizeModuleAddinId(
        string registeredAddinId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registeredAddinId);

        string value =
            new string(
                registeredAddinId
                    .Trim()
                    .Where(static character =>
                        char.IsLetterOrDigit(character))
                    .ToArray());

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "The registered add-in ID does not contain a usable module name.",
                nameof(registeredAddinId));
        }

        if (value.EndsWith(
            "Addin",
            StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        return
            char.ToUpperInvariant(value[0]) +
            value[1..] +
            "Addin";
    }

    private static void CreateParentDirectory(
        string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        string? directory =
            Path.GetDirectoryName(filePath);

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException(
                $"Could not determine a directory for '{filePath}'.");
        }

        Directory.CreateDirectory(directory);
    }
}
