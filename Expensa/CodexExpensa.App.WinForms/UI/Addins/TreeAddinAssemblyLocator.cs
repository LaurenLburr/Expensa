using System.Reflection;

namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class TreeAddinAssemblyLocator
{
    public Assembly FindAssembly(TreeAddinDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        string assemblyName =
            definition.ProjectAssemblyName;

        Assembly? loadedAssembly =
            AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(assembly =>
                    string.Equals(
                        assembly.GetName().Name,
                        assemblyName,
                        StringComparison.OrdinalIgnoreCase));

        if (loadedAssembly is not null)
        {
            return loadedAssembly;
        }

        string assemblyPath =
            FindAssemblyPath(definition);

        return Assembly.LoadFrom(assemblyPath);
    }

    public string FindAssemblyPath(TreeAddinDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        foreach (string candidatePath in GetCandidatePaths(definition))
        {
            string fullPath =
                Path.GetFullPath(candidatePath);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        string searched =
            string.Join(
                Environment.NewLine,
                GetCandidatePaths(definition).Select(Path.GetFullPath));

        throw new FileNotFoundException(
            $"Could not find {definition.ProjectAssemblyName}. Build the {definition.AddinName} add-in and make sure its compiled files are available under the Extensions folder.{Environment.NewLine}{searched}");
    }

    private static IReadOnlyList<string> GetCandidatePaths(TreeAddinDefinition definition)
    {
        string baseDirectory =
            AppContext.BaseDirectory;

        List<string> candidatePaths = [];

        foreach (string root in EnumerateAncestorFolders(baseDirectory))
        {
            candidatePaths.Add(Path.Combine(
                root,
                "Expensa",
                "Extensions",
                definition.AddinName,
                definition.ProjectAssemblyName + ".dll"));

            candidatePaths.Add(Path.Combine(
                root,
                "Extensions",
                "Modules",
                definition.AddinName,
                "bin",
                "Debug",
                "net8.0-windows",
                definition.ProjectAssemblyName + ".dll"));

            candidatePaths.Add(Path.Combine(
                root,
                "Extensions",
                "Modules",
                definition.AddinName,
                "bin",
                "Release",
                "net8.0-windows",
                definition.ProjectAssemblyName + ".dll"));

            candidatePaths.Add(Path.Combine(
                root,
                "Modules",
                definition.AddinName,
                definition.ProjectAssemblyName + ".dll"));
        }

        candidatePaths.Add(Path.Combine(baseDirectory, definition.ProjectAssemblyName + ".dll"));
        candidatePaths.Add(Path.Combine(Directory.GetCurrentDirectory(), definition.ProjectAssemblyName + ".dll"));

        return candidatePaths
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IEnumerable<string> EnumerateAncestorFolders(string startPath)
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
