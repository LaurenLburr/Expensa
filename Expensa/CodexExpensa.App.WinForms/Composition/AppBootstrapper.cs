using System;
using System.IO;
using System.Windows.Forms;
using CodexExpensa.App.WinForms.UI;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Banks;
using CodexExpensa.Core.Domain.Payees;
using CodexExpensa.Core.Domain.Transactions;
using CodexExpensa.Data.Sqlite.Accounts;
using CodexExpensa.Data.Sqlite.Banks;
using CodexExpensa.Data.Sqlite.Db;
using CodexExpensa.Data.Sqlite.Db.Schema;
using CodexExpensa.Data.Sqlite.Payees;
using CodexExpensa.Data.Sqlite.Transactions;

namespace CodexExpensa.App.WinForms.Composition;

public static class AppBootstrapper
{
    public static MainForm Initialize()
    {
        var dbPath = GetDatabasePath();

        // In-memory DB seeded from file (Save flushes to disk)
        var db = SqliteDatabase.OpenMemorySeededFromFile(dbPath);

        // ✅ MUST RUN MIGRATIONS BEFORE ANY REPO QUERIES
        var migrationRunner = new MigrationRunner();
        MigrationRunResult migResult;

        try
        {
            migResult = migrationRunner.ApplyPendingMigrations(db);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Database migration failed:\n\n{ex}",
                "Codex Expensa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            throw; // keep failing fast; DB is not safe to use
        }

        // Optional: if you want visibility during dev
        if (migResult.AppliedCount > 0)
        {
            // Avoid noisy popups later if you don't want them.
            // MessageBox.Show($"Applied {migResult.AppliedCount} migration(s).", "Codex Expensa");
        }

        IMigrationStatusProvider migrationStatusProvider = new SqliteMigrationStatusProvider(migrationRunner);

        // Repositories (now safe: tables exist)
        IAccountRepository accounts = new SqliteAccountRepository(db);
        IBankRepository banks = new SqliteBankRepository(db);
        ITransactionRepository transactions = new SqliteTransactionRepository(db);
        IPayeeRepository payees = new SqlitePayeeRepository(db);

        return new MainForm(
            dbSession: db,
            accounts: accounts,
            banks: banks,
            transactions: transactions,
            payees: payees,
            createDbStatusForm: () => new DbStatusForm(db, migrationStatusProvider, dbPath)
        );
    }

    private static string GetDatabasePath()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodexExpensa","db");

        Directory.CreateDirectory(folder);

        return Path.Combine(folder, "codexexpensa.db");
    }
}