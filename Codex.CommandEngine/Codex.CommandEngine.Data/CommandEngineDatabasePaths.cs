namespace Codex.CommandEngine.Data;

public static class CommandEngineDatabasePaths
{
    public static string DefaultDatabasePath
    {
        get
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Codex.CommandEngine",
                "Database");

            return Path.Combine(folder, "command-engine.db");
        }
    }

    public static string ResolveDevDatabasePath()
    {
        string baseDirectory = AppContext.BaseDirectory;

        string[] candidates =
        [
            Path.Combine(baseDirectory, "Database", "CommandEngine.dev.db"),
            Path.Combine(baseDirectory, "CommandEngine.dev.db"),
            Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "..", "Database", "CommandEngine.dev.db")),
            Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "Database", "CommandEngine.dev.db")),
            Path.Combine(Directory.GetCurrentDirectory(), "Database", "CommandEngine.dev.db")
        ];

        foreach (string candidate in candidates)
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return candidates[0];
    }
}
