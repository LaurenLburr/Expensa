using System;
using System.IO;
using System.Windows.Forms;
using CodexExpensa.App.WinForms.UI;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Banks;
using CodexExpensa.Data.Sqlite.Accounts;
using CodexExpensa.Data.Sqlite.Banks;
using CodexExpensa.Data.Sqlite.Db;
using CodexExpensa.Data.Sqlite.Db.Schema;

namespace CodexExpensa.App.WinForms.Composition;

public sealed class AppBootstrapper
{
    public MainForm Initialize()
    {
        var dbPath = GetDatabasePath();

        // Concrete DB session (also implements IDatabaseSession / IDatabaseSession-like abstraction)
        var db = SqliteDatabase.OpenMemorySeededFromFile(dbPath);

        // Apply migrations at startup
        var migrationRunner = new MigrationRunner();
        migrationRunner.ApplyPendingMigrations(db);

        // Status provider for the DB Status UI (requires runner)
        IMigrationStatusProvider migrationStatusProvider = new SqliteMigrationStatusProvider(migrationRunner);

        // Repositories
        IAccountRepository accounts = new SqliteAccountRepository(db);
        IBankRepository banks = new SqliteBankRepository(db);

        var mainForm = new MainForm(
            dbSession: db,
            accounts: accounts,
            banks: banks,
            createDbStatusForm: () => new DbStatusForm(db, migrationStatusProvider, dbPath));

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