namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

internal static class TestRepositoryPath
{
    public static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Expensa")) &&
                Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find repository root. Expected a parent directory containing Expensa and Extensions folders.");
    }

    public static string ReadExpensaFile(params string[] parts)
    {
        string repositoryRoot = FindRepositoryRoot();
        string path = Path.Combine([repositoryRoot, "Expensa", "CodexExpensa.App.WinForms", .. parts]);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Expected file was not found: {path}", path);
        }

        return File.ReadAllText(path);
    }
}
