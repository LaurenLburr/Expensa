namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;

public static class ExpensaAddinAssemblyLocator
{
    public static string FindAssemblyPath(
        ExpensaAddinRuntimeDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        foreach (string candidatePath in GetCandidatePaths(descriptor))
        {
            string fullPath = Path.GetFullPath(candidatePath);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        string searched =
            string.Join(
                Environment.NewLine,
                GetCandidatePaths(descriptor).Select(Path.GetFullPath));

        throw new FileNotFoundException(
            $"Could not find {descriptor.AssemblyFileName}.{Environment.NewLine}{Environment.NewLine}Searched:{Environment.NewLine}{searched}");
    }

    private static IReadOnlyList<string> GetCandidatePaths(
        ExpensaAddinRuntimeDescriptor descriptor)
    {
        List<string> candidatePaths =
        [
            Path.Combine(AppContext.BaseDirectory, "Modules", descriptor.AddinName, descriptor.AssemblyFileName),
            Path.Combine(Directory.GetCurrentDirectory(), "Modules", descriptor.AddinName, descriptor.AssemblyFileName)
        ];

        foreach (string root in EnumerateAncestorFolders(AppContext.BaseDirectory))
        {
            candidatePaths.Add(Path.Combine(root, "Modules", descriptor.AddinName, descriptor.AssemblyFileName));
            candidatePaths.Add(Path.Combine(root, "Modules", descriptor.AddinName, "bin", "Debug", "net8.0-windows", descriptor.AssemblyFileName));
            candidatePaths.Add(Path.Combine(root, "Modules", descriptor.AddinName, "bin", "Release", "net8.0-windows", descriptor.AssemblyFileName));

            candidatePaths.Add(Path.Combine(root, "Extensions", "Modules", descriptor.AddinName, "bin", "Debug", "net8.0-windows", descriptor.AssemblyFileName));
            candidatePaths.Add(Path.Combine(root, "Extensions", "Modules", descriptor.AddinName, "bin", "Release", "net8.0-windows", descriptor.AssemblyFileName));

            candidatePaths.Add(Path.Combine(root, "Expensa", "CodexExpensa.App.WinForms", "bin", "Debug", "net8.0-windows", "Modules", descriptor.AddinName, descriptor.AssemblyFileName));
            candidatePaths.Add(Path.Combine(root, "Expensa", "CodexExpensa.App.WinForms", "bin", "Release", "net8.0-windows", "Modules", descriptor.AddinName, descriptor.AssemblyFileName));
        }

        return candidatePaths
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IEnumerable<string> EnumerateAncestorFolders(
        string startPath)
    {
        DirectoryInfo? directory = new(startPath);

        while (directory is not null)
        {
            yield return directory.FullName;
            directory = directory.Parent;
        }
    }
}
