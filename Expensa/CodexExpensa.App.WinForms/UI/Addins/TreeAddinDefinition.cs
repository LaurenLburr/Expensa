using Microsoft.Data.Sqlite;

namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class TreeAddinDefinition
{
    public required string AddinName { get; init; }

    public required string ProjectAssemblyName { get; init; }

    public required string SmokeRunnerTypeName { get; init; }

    public required string RequestTypeName { get; init; }

    public required string ScreenRunnerTypeName { get; init; }

    public required string ScreenRequestTypeName { get; init; }

    public required TreeAddinKind Kind { get; init; }

    public int DisplaySort { get; init; }

    public static IReadOnlyList<TreeAddinDefinition> DefaultTreeAddins()
    {
        IReadOnlyDictionary<string, int> displaySortOverrides =
            LoadRegisteredDisplaySortOverrides();

        List<TreeAddinDefinition> addins =
        [
            new TreeAddinDefinition
            {
                AddinName = "WebsitesAddin",
                DisplaySort = GetDisplaySort(displaySortOverrides, "WebsitesAddin", 100),
                ProjectAssemblyName = "WebsitesAddin",
                SmokeRunnerTypeName = "WebsitesAddin.WebsiteLoadRuntimeSmokeRunner",
                RequestTypeName = "WebsitesAddin.WebsiteLoadRequest",
                ScreenRunnerTypeName = "WebsitesAddin.WebsiteScreenRuntimeRunner",
                ScreenRequestTypeName = "WebsitesAddin.WebsiteScreenRequest",
                Kind = TreeAddinKind.Websites
            },
            new TreeAddinDefinition
            {
                AddinName = "BudgetsAddin",
                DisplaySort = GetDisplaySort(displaySortOverrides, "BudgetsAddin", 200),
                ProjectAssemblyName = "BudgetsAddin",
                SmokeRunnerTypeName = "BudgetsAddin.BudgetLoadRuntimeSmokeRunner",
                RequestTypeName = "BudgetsAddin.BudgetLoadRequest",
                ScreenRunnerTypeName = "BudgetsAddin.BudgetScreenRuntimeRunner",
                ScreenRequestTypeName = "BudgetsAddin.BudgetScreenRequest",
                Kind = TreeAddinKind.Budgets
            },
            new TreeAddinDefinition
            {
                AddinName = "PayeesAddin",
                DisplaySort = GetDisplaySort(displaySortOverrides, "PayeesAddin", 300),
                ProjectAssemblyName = "PayeesAddin",
                SmokeRunnerTypeName = "PayeesAddin.PayeeLoadRuntimeSmokeRunner",
                RequestTypeName = "PayeesAddin.PayeeLoadRequest",
                ScreenRunnerTypeName = "PayeesAddin.PayeeScreenRuntimeRunner",
                ScreenRequestTypeName = "PayeesAddin.PayeeScreenRequest",
                Kind = TreeAddinKind.Payees
            }
        ];

        return addins
            .OrderBy(static addin => addin.DisplaySort)
            .ThenBy(static addin => addin.AddinName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static int GetDisplaySort(
        IReadOnlyDictionary<string, int> displaySortOverrides,
        string addinName,
        int fallback)
    {
        return displaySortOverrides.TryGetValue(addinName, out int displaySort)
            ? displaySort
            : fallback;
    }

    private static IReadOnlyDictionary<string, int> LoadRegisteredDisplaySortOverrides()
    {
        string databasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "ExtensionMgr.db");

        if (!File.Exists(databasePath))
        {
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        try
        {
            using SqliteConnection connection = new($"Data Source={databasePath}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                "SELECT [ProjectName], [SortOrder] " +
                "FROM [ExtensionProjectRegistration];";

            using SqliteDataReader reader = command.ExecuteReader();

            Dictionary<string, int> values =
                new(StringComparer.OrdinalIgnoreCase);

            while (reader.Read())
            {
                values[reader.GetString(0)] = reader.GetInt32(1);
            }

            return values;
        }
        catch (SqliteException)
        {
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
