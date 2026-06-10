namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class AddinRuntimeDatabasePathService
{
    public AddinRuntimeDatabaseLocation GetRuntimeDatabaseLocation(
        string addinId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(addinId);

        string folder =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Expensa",
                "Extensions",
                "Runtime",
                addinId);

        string databaseName =
            GetCurrentDatabaseFileName(addinId);

        return new AddinRuntimeDatabaseLocation
        {
            AddinId = addinId,
            DatabaseName = databaseName,
            DatabasePath = Path.Combine(folder, databaseName)
        };
    }

    public string GetActiveRuntimePathFile(
        string addinId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(addinId);

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "Runtime",
            addinId,
            "active-runtime-db.txt");
    }

    public string GetDevCurrentDatabasePath(
        string addinId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(addinId);

        return Path.Combine(
            FindExtensionsRoot(),
            "Modules",
            addinId,
            "DevDatabase",
            GetCurrentDatabaseFileName(addinId));
    }

    public string GetProdDatabasePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodexExpensa",
            "db",
            "codexexpensa.db");
    }

    public static string GetCurrentDatabaseFileName(
        string addinId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(addinId);

        string baseName =
            addinId.Trim();

        if (baseName.EndsWith("Addin", StringComparison.OrdinalIgnoreCase))
        {
            baseName = baseName[..^"Addin".Length];
        }

        if (baseName.EndsWith("AddIn", StringComparison.OrdinalIgnoreCase))
        {
            baseName = baseName[..^"AddIn".Length];
        }

        baseName =
            new string(
                baseName
                    .Where(static character => char.IsLetterOrDigit(character))
                    .Select(static character => char.ToLowerInvariant(character))
                    .ToArray());

        if (string.IsNullOrWhiteSpace(baseName))
        {
            baseName = "addin";
        }

        return $"{baseName}.current.db";
    }

    private static string FindExtensionsRoot()
    {
        DirectoryInfo? directory =
            new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (IsExtensionsRoot(directory.FullName))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        DirectoryInfo? currentDirectory =
            new(Directory.GetCurrentDirectory());

        while (currentDirectory is not null)
        {
            if (IsExtensionsRoot(currentDirectory.FullName))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Extensions");
    }

    private static bool IsExtensionsRoot(
        string folder)
    {
        return Directory.Exists(Path.Combine(folder, "Host")) &&
            Directory.Exists(Path.Combine(folder, "Modules"));
    }
}
