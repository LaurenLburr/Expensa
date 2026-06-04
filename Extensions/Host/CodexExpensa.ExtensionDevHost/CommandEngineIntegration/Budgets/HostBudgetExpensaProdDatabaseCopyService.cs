namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetExpensaProdDatabaseCopyService
{
    private const string CurrentDatabaseFileName = "budgets.current.db";

    public HostBudgetExpensaProdDatabaseCopyResult CopyToDevAndRuntime()
    {
        string prodDatabasePath = GetDefaultExpensaProdDatabasePath();

        if (!File.Exists(prodDatabasePath))
        {
            throw new FileNotFoundException(
                $"Expensa production database was not found: {prodDatabasePath}",
                prodDatabasePath);
        }

        string devDatabasePath = GetBudgetsDevCurrentDatabasePath();
        string runtimeDatabasePath = GetBudgetsRuntimeCurrentDatabasePath();
        string activeRuntimePathFile = GetActiveRuntimePathFile();

        Directory.CreateDirectory(Path.GetDirectoryName(devDatabasePath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(runtimeDatabasePath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(activeRuntimePathFile)!);

        List<string> warnings = [];

        TryReplaceCurrentDatabase(prodDatabasePath, devDatabasePath, "development", warnings);
        TryReplaceCurrentDatabase(prodDatabasePath, runtimeDatabasePath, "runtime", warnings);

        File.WriteAllText(activeRuntimePathFile, runtimeDatabasePath);

        return new HostBudgetExpensaProdDatabaseCopyResult
        {
            ProdDatabasePath = prodDatabasePath,
            DevDatabasePath = devDatabasePath,
            RuntimeDatabasePath = runtimeDatabasePath,
            ActiveRuntimePathFile = activeRuntimePathFile,
            Warnings = warnings
        };
    }

    private static void TryReplaceCurrentDatabase(
        string sourceDatabasePath,
        string targetDatabasePath,
        string targetDescription,
        List<string> warnings)
    {
        try
        {
            ReplaceCurrentDatabase(sourceDatabasePath, targetDatabasePath);
        }
        catch (IOException exception)
        {
            warnings.Add(
                $"Could not replace the {targetDescription} database because it is currently in use: {targetDatabasePath}{Environment.NewLine}{exception.Message}");
        }
        catch (UnauthorizedAccessException exception)
        {
            warnings.Add(
                $"Could not replace the {targetDescription} database because access was denied: {targetDatabasePath}{Environment.NewLine}{exception.Message}");
        }
    }

    private static void ReplaceCurrentDatabase(string sourceDatabasePath, string targetDatabasePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(targetDatabasePath)!);

        if (File.Exists(targetDatabasePath))
        {
            string archivePath = CreateArchivePath(targetDatabasePath);
            File.Move(targetDatabasePath, archivePath);
        }

        File.Copy(sourceDatabasePath, targetDatabasePath, overwrite: false);
    }

    private static string CreateArchivePath(string databasePath)
    {
        string folder = Path.GetDirectoryName(databasePath)!;
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(databasePath);
        string extension = Path.GetExtension(databasePath);
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        return Path.Combine(folder, $"{fileNameWithoutExtension}.archive.{timestamp}{extension}");
    }

    private static string GetDefaultExpensaProdDatabasePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodexExpensa",
            "db",
            "codexexpensa.db");
    }

    private static string GetBudgetsDevCurrentDatabasePath()
    {
        return Path.Combine(
            FindExtensionsRoot(),
            "Modules",
            "BudgetsAddin",
            "DevDatabase",
            CurrentDatabaseFileName);
    }

    private static string GetBudgetsRuntimeCurrentDatabasePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "Runtime",
            "BudgetsAddin",
            CurrentDatabaseFileName);
    }

    private static string GetActiveRuntimePathFile()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "Runtime",
            "BudgetsAddin",
            "active-runtime-db.txt");
    }

    private static string FindExtensionsRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (string.Equals(directory.Name, "Extensions", StringComparison.OrdinalIgnoreCase))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Extensions");
    }
}
