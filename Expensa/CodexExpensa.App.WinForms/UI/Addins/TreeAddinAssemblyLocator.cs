namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class TreeAddinAssemblyLocator
{
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
            $"Could not find {definition.AssemblyFileName}. Deploy {definition.AddinName} to Expensa first.{Environment.NewLine}{searched}");
    }

    private static IReadOnlyList<string> GetCandidatePaths(TreeAddinDefinition definition)
    {
        string baseDirectory =
            AppContext.BaseDirectory;

        return
        [
            Path.Combine(baseDirectory, "Modules", definition.ModuleFolderName, definition.AssemblyFileName),
            Path.Combine(Directory.GetCurrentDirectory(), "Modules", definition.ModuleFolderName, definition.AssemblyFileName)
        ];
    }
}
