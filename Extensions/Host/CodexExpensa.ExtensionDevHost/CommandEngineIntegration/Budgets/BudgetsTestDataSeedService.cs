using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class BudgetsTestDataSeedService
{
    public BudgetsTestDataSeedResult Reset(
        string devDatabasePath,
        string scriptPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(devDatabasePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(scriptPath);

        string fullDatabasePath =
            Path.GetFullPath(devDatabasePath);

        string fullScriptPath =
            Path.GetFullPath(scriptPath);

        if (!File.Exists(fullDatabasePath))
        {
            throw new FileNotFoundException(
                $"The Budgets Dev database was not found:{Environment.NewLine}{fullDatabasePath}",
                fullDatabasePath);
        }

        if (!File.Exists(fullScriptPath))
        {
            throw new FileNotFoundException(
                $"The Budgets test-data SQL file was not found:{Environment.NewLine}{fullScriptPath}",
                fullScriptPath);
        }

        string sql =
            File.ReadAllText(fullScriptPath);

        if (string.IsNullOrWhiteSpace(sql))
        {
            throw new InvalidDataException(
                $"The Budgets test-data SQL file is empty:{Environment.NewLine}{fullScriptPath}");
        }

        using AddinMemoryDatabaseSession session =
            AddinMemoryDatabaseSession.LoadFromFile(fullDatabasePath);

        using (SqliteCommand command = session.Connection.CreateCommand())
        {
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        BudgetsTestDataSeedResult result =
            ReadResult(
                session.Connection,
                fullDatabasePath,
                fullScriptPath);

        session.SaveToFile(fullDatabasePath);

        return result;
    }

    private static BudgetsTestDataSeedResult ReadResult(
        SqliteConnection connection,
        string databasePath,
        string scriptPath)
    {
        int budgetMonthCount =
            ExecuteCount(
                connection,
                """
                SELECT COUNT(*)
                FROM [BudgetMonth]
                WHERE ([Year] = 2026 AND [Month] = 5)
                   OR ([Year] = 2026 AND [Month] = 6);
                """);

        int budgetMonthRowCount =
            ExecuteCount(
                connection,
                """
                SELECT COUNT(*)
                FROM [BudgetMonthRow]
                WHERE [BudgetMonthRowId] LIKE 'test-budget-%';
                """);

        int transactionCount =
            ExecuteCount(
                connection,
                """
                SELECT COUNT(*)
                FROM [Txn]
                WHERE [Note] LIKE 'test-budget-seed:%';
                """);

        List<string> statuses = [];

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                SELECT DISTINCT [Status]
                FROM [Txn]
                WHERE [Note] LIKE 'test-budget-seed:%'
                ORDER BY [Status];
                """;

            using SqliteDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                string? status =
                    Convert.ToString(
                        reader.GetValue(0),
                        CultureInfo.InvariantCulture);

                if (!string.IsNullOrWhiteSpace(status))
                {
                    statuses.Add(status);
                }
            }
        }

        return new BudgetsTestDataSeedResult
        {
            DevDatabasePath = databasePath,
            ScriptPath = scriptPath,
            BudgetMonthCount = budgetMonthCount,
            BudgetMonthRowCount = budgetMonthRowCount,
            TransactionCount = transactionCount,
            Statuses = statuses
        };
    }

    private static int ExecuteCount(
        SqliteConnection connection,
        string sql)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText = sql;

        return Convert.ToInt32(
            command.ExecuteScalar(),
            CultureInfo.InvariantCulture);
    }
}
