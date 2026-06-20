using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsTestDataUiStructureTests
{
    [Fact]
    public void DatabasePanelTemplate_OwnsOptionalDatabaseActionLink()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Templates",
            "DatabasePanelTemplate.cs");

        Assert.Contains("ConfigureDatabaseActionLink(", text);
        Assert.Contains("panel1.Controls.Add(_databaseActionLink)", text);
        Assert.Contains("_databaseAction?.Invoke()", text);
    }

    [Fact]
    public void BudgetsDatabasePanel_ExposesRepeatableTestDataReset()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsDatabasePanelForm.cs");

        Assert.Contains("\"Reset Budget Test Data\"", text);
        Assert.Contains("ResetBudgetTestData", text);
        Assert.Contains("GetDevCurrentDatabasePath(AddinId)", text);
        Assert.Contains("\"BudgetsTestData.sql\"", text);
        Assert.Contains("_testDataSeedService.Reset(", text);
        Assert.Contains("ReplaceRuntimeDatabaseFromDev(AddinId)", text);
    }

    [Fact]
    public void TestDataSeedService_UsesSafeMemorySession()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTestDataSeedService.cs");

        Assert.Contains("AddinMemoryDatabaseSession.LoadFromFile", text);
        Assert.Contains("session.SaveToFile(fullDatabasePath)", text);
        Assert.DoesNotContain("new SqliteConnection", text);
        Assert.DoesNotContain("SqliteConnectionStringBuilder", text);
    }

    [Fact]
    public void TestDataSql_ReusesExistingBudgetMonths()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "..",
            "..",
            "Modules",
            "BudgetsAddin",
            "DevDatabase",
            "BudgetsTestData.sql");

        Assert.Contains("ON CONFLICT(Year, Month) DO NOTHING", text);
        Assert.Contains(
            "SELECT BudgetMonthId FROM BudgetMonth WHERE Year = 2026 AND Month = 5",
            text);
        Assert.Contains(
            "SELECT BudgetMonthId FROM BudgetMonth WHERE Year = 2026 AND Month = 6",
            text);
        Assert.DoesNotContain(
            "DELETE FROM BudgetMonth\nWHERE BudgetMonthId IN",
            text);
    }
}
