using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsAddinPhase1StructureTests
{
    [Fact]
    public void BudgetsAddinProject_ReferencesCommandEngineAndSqlite()
    {
        string text = ReadFile(
           
            "Modules",
            "BudgetsAddin",
            "BudgetsAddin.csproj");

        Assert.Contains("Codex.CommandEngine.Core.csproj", text);
        Assert.Contains("Microsoft.Data.Sqlite", text);
        Assert.Contains("CopyLocalLockFileAssemblies", text);
    }

    [Fact]
    public void BudgetRepository_LoadsBudgetYearsAndMonthsFromBudgetMonth()
    {
        string text = ReadFile(
            
            "Modules",
            "BudgetsAddin",
            "SqliteBudgetRepository.cs");

        Assert.Contains("LoadBudgets", text);
        Assert.Contains("FROM [BudgetMonth]", text);
        Assert.Contains("[Year] AS [BudgetYear]", text);
        Assert.Contains("[Month] AS [BudgetMonthNumber]", text);
        Assert.Contains("BuildBudgetTree", text);
        Assert.DoesNotContain("FROM [Budget]", text);
    }

    [Fact]
    public void BudgetSmokeRunner_ReturnsCommandExecutionResult()
    {
        string text = ReadFile(
          
            "Modules",
            "BudgetsAddin",
            "BudgetLoadRuntimeSmokeRunner.cs");

        Assert.Contains("CommandExecutionResult", text);
        Assert.Contains("CommandName = \"Budgets.LoadTree\"", text);
        Assert.Contains("CommandExecutionStatus.Succeeded", text);
        Assert.Contains("OutputJson", text);
    }

    private static string ReadFile(params string[] parts)
    {
        string repositoryRoot = FindRepositoryRoot();
        string path = Path.Combine([repositoryRoot, .. parts]);

        Assert.True(File.Exists(path), $"File was not found: {path}");

        return File.ReadAllText(path);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
