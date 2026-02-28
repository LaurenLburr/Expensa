using System.Runtime.InteropServices;

namespace CodexExpensa.App.WinForms.Composition;

public static class AppPaths
{
    public static string AppDataRoot()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(root, "CodexExpensa");
    }

    public static string DatabaseFolder()
    {
        return Path.Combine(AppDataRoot(), "db");
    }

    public static string DatabaseFilePath()
    {
        return Path.Combine(DatabaseFolder(), "codexexpensa.db");
    }

    public static void EnsureFoldersExist()
    {
        Directory.CreateDirectory(DatabaseFolder());
    }
}