using System.IO;

namespace CodexExpensa.App.WinForms.Infrastructure;

public static class DbPathProvider
{
    public static string GetDefaultDbPath()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // %LOCALAPPDATA%\CodexExpensa\db
        var folder = Path.Combine(root, "CodexExpensa", "db");

        // Ensure folder exists
        Directory.CreateDirectory(folder);

        // %LOCALAPPDATA%\CodexExpensa\db\codexexpensa.db
        return Path.Combine(folder, "codexexpensa.db");
    }
}