using System;
using System.IO;
using System.Windows.Forms;
using CodexExpensa.App.WinForms.UI;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Banks;
using CodexExpensa.Core.Domain.Payees;
using CodexExpensa.Core.Domain.Transactions;
using CodexExpensa.Core.Events;
using CodexExpensa.Data.Sqlite.Accounts;
using CodexExpensa.Data.Sqlite.Banks;
using CodexExpensa.Data.Sqlite.Db;
using CodexExpensa.Data.Sqlite.Db.Schema;
using CodexExpensa.Data.Sqlite.Payees;
using CodexExpensa.Data.Sqlite.Transactions;
using EngineConfig = Codex.Data.SQLiteEngine.Configuration;

namespace CodexExpensa.App.WinForms.Composition;

public static class AppBootstrapper
{
    public static MainForm Initialize()
    {
        var dbPath = GetDatabasePath();

        var engineOptions = new EngineConfig.SqliteEngineOptions
        {
            SqlCatalog = new EngineConfig.SqlCatalogOptions
            {
                TableName = "SqlQuery",
                NameColumn = "QueryName",

                // IMPORTANT:
                // If your SqlQuery table stores SQL text in a column named "Sql",
                // leave this as "Sql".
                // If your table uses "SqlText", change this to "SqlText".
                SqlColumn = "SqlText"
            }

            // Logging is optional for now.
            // Add Logging = ... later if/when you want engine table logging enabled.
        };

        // In-memory DB seeded from file (Save flushes to disk)
        var db = SqliteDatabase.OpenMemorySeededFromFile(dbPath, engineOptions);

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

            throw;
        }

        IMigrationStatusProvider migrationStatusProvider = new SqliteMigrationStatusProvider(migrationRunner);

        ITableChangePublisher tableChangePublisher = new TableChangePublisher();

        tableChangePublisher.TableChanged += (_, args) =>
        {
            System.Diagnostics.Trace.WriteLine(
                $"[TableChanged] Table={args.TableName}; Operation={args.Operation}; Key={args.KeyValue}; Source={args.Source}; Summary={args.Summary}");
        };

        IAccountRepository accounts = new SqliteAccountRepository(db, tableChangePublisher);
        IBankRepository banks = new SqliteBankRepository(db);
        ITransactionRepository transactions = new SqliteTransactionRepository(db);
        IPayeeRepository payees = new SqlitePayeeRepository(db);

        return new MainForm(
            dbSession: db,
            accounts: accounts,
            banks: banks,
            transactions: transactions,
            payees: payees,
            createDbStatusForm: () => new DbStatusForm(db, migrationStatusProvider, dbPath),
            tableChangePublisher: tableChangePublisher
        );
    }

    private static string GetDatabasePath()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodexExpensa",
            "db");

        Directory.CreateDirectory(folder);

        return Path.Combine(folder, "codexexpensa.db");
    }
}