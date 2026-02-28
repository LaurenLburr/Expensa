using CodexExpensa.App.WinForms.UI;
using CodexExpensa.Data.Sqlite.Db;
using CodexExpensa.Data.Sqlite.Db.Schema;

namespace CodexExpensa.App.WinForms.Composition;

public sealed class AppBootstrapper
{
    public MainForm Initialize()
    {
        var dbPath = GetDatabasePath();

        // Your current MigrationRunner expects SqliteDatabase, so we keep it concrete here.
        var db = SqliteDatabase.OpenMemorySeededFromFile(dbPath);

        var migrationRunner = new MigrationRunner();
        migrationRunner.ApplyPendingMigrations(db);

        var mainForm = new MainForm(db, migrationRunner, dbPath);

        mainForm.FormClosing += (_, _) =>
        {
            try
            {
                db.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Database save failed:\n\n{ex}",
                    "Codex Expensa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                db.Dispose();
            }
        };

        return mainForm;
    }

    private static string GetDatabasePath()
    {
        var baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodexExpensa",
            "db");

        Directory.CreateDirectory(baseFolder);

        return Path.Combine(baseFolder, "codexexpensa.db");
    }
}