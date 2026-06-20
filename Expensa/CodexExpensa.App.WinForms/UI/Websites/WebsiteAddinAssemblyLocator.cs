namespace CodexExpensa.App.WinForms.UI.Websites;

public static class WebsiteAddinAssemblyLocator
{
    private const string AssemblyName = "WebsitesAddin.dll";

    public static string FindWebsitesAddinAssemblyPath()
    {
        foreach (string candidatePath in GetCandidatePaths())
        {
            string fullPath = Path.GetFullPath(candidatePath);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        string candidates =
            string.Join(
                Environment.NewLine,
                GetCandidatePaths().Select(Path.GetFullPath));

        throw new FileNotFoundException(
            $"Could not locate {AssemblyName}. Build or deploy the Websites add-in under Expensa\\Extensions\\WebsitesAddin.{Environment.NewLine}{Environment.NewLine}Searched:{Environment.NewLine}{candidates}");
    }

    private static IReadOnlyList<string> GetCandidatePaths()
    {
        List<string> candidatePaths = [];

        foreach (string root in EnumerateAncestorFolders(AppContext.BaseDirectory))
        {
            candidatePaths.Add(
                Path.Combine(
                    root,
                    "Expensa",
                    "Extensions",
                    "WebsitesAddin",
                    AssemblyName));
        }

        candidatePaths.Add(
            Path.Combine(
                AppContext.BaseDirectory,
                "Modules",
                "WebsitesAddin",
                AssemblyName));

        candidatePaths.Add(
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "Modules",
                "WebsitesAddin",
                AssemblyName));

        foreach (string root in EnumerateAncestorFolders(AppContext.BaseDirectory))
        {

            candidatePaths.Add(
                Path.Combine(
                    root,
                    "Modules",
                    "WebsitesAddin",
                    AssemblyName));

            candidatePaths.Add(
                Path.Combine(
                    root,
                    "Modules",
                    "WebsitesAddin",
                    "bin",
                    "Debug",
                    "net8.0-windows",
                    AssemblyName));

            candidatePaths.Add(
                Path.Combine(
                    root,
                    "Modules",
                    "WebsitesAddin",
                    "bin",
                    "Debug",
                    "net8.0",
                    AssemblyName));

            candidatePaths.Add(
                Path.Combine(
                    root,
                    "Modules",
                    "WebsitesAddin",
                    "bin",
                    "Release",
                    "net8.0-windows",
                    AssemblyName));

            candidatePaths.Add(
                Path.Combine(
                    root,
                    "Modules",
                    "WebsitesAddin",
                    "bin",
                    "Release",
                    "net8.0",
                    AssemblyName));
        }

        return candidatePaths
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IEnumerable<string> EnumerateAncestorFolders(
        string startPath)
    {
        DirectoryInfo? directory =
            new(startPath);

        while (directory is not null)
        {
            yield return directory.FullName;

            directory = directory.Parent;
        }
    }
}
