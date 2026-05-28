namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class ExtensionOutputFolderResolver
{
    public static string FindModulesFolder()
    {
        DirectoryInfo? directory =
            new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            string directModulesFolder =
                Path.Combine(
                    directory.FullName,
                    "Modules");

            if (Directory.Exists(directModulesFolder))
            {
                return directModulesFolder;
            }

            string extensionsModulesFolder =
                Path.Combine(
                    directory.FullName,
                    "Extensions",
                    "Modules");

            if (Directory.Exists(extensionsModulesFolder))
            {
                return extensionsModulesFolder;
            }

            directory = directory.Parent;
        }

        return Path.Combine(
            AppContext.BaseDirectory,
            "Extensions",
            "Modules");
    }

    public static bool IsModulesFolder(
        string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            return false;
        }

        string folderName =
            Path.GetFileName(
                folderPath.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar));

        return string.Equals(
            folderName,
            "Modules",
            StringComparison.OrdinalIgnoreCase);
    }

    public static string GetExtensionDebugOutputFolder(
        string extensionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extensionName);

        return Path.Combine(
            FindModulesFolder(),
            extensionName,
            "bin",
            "Debug",
            "net8.0-windows");
    }

    public static string GetExtensionReleaseOutputFolder(
        string extensionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extensionName);

        return Path.Combine(
            FindModulesFolder(),
            extensionName,
            "bin",
            "Release",
            "net8.0-windows");
    }

    public static string GetFirstExistingExtensionOutputFolder(
        string extensionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extensionName);

        string debugFolder =
            GetExtensionDebugOutputFolder(extensionName);

        if (Directory.Exists(debugFolder))
        {
            return debugFolder;
        }

        string releaseFolder =
            GetExtensionReleaseOutputFolder(extensionName);

        if (Directory.Exists(releaseFolder))
        {
            return releaseFolder;
        }

        return debugFolder;
    }

    public static string GetWebsitesAddinDebugOutputFolder()
    {
        return GetExtensionDebugOutputFolder("WebsitesAddin");
    }

    public static string GetWebsitesAddinReleaseOutputFolder()
    {
        return GetExtensionReleaseOutputFolder("WebsitesAddin");
    }

    public static string GetFirstExistingWebsitesAddinOutputFolder()
    {
        return GetFirstExistingExtensionOutputFolder("WebsitesAddin");
    }
}
