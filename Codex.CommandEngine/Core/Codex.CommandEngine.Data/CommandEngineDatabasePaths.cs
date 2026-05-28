namespace Codex.CommandEngine.Data;

public static class CommandEngineDatabasePaths
{
    public static string DefaultDatabasePath
    {
        get
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Codex",
                "CommandEngine",
                "db");

            Directory.CreateDirectory(folder);

            return Path.Combine(folder, "CodexCommandEngine.db");
        }
    }
}
