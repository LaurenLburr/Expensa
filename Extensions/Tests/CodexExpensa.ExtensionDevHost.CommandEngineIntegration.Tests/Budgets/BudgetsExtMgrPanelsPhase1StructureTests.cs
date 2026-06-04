using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsExtMgrPanelsPhase1StructureTests
{
    [Fact]
    public void BudgetsRuntimeInvoker_LoadsBudgetAddinWithDependencyResolver()
    {
        string text = ReadFile(
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "HostBudgetRuntimeModuleInvoker.cs");

        Assert.Contains("BudgetsAddin.dll", text);
        Assert.Contains("BudgetLoadRuntimeSmokeRunner", text);
        Assert.Contains("AssemblyDependencyResolver", text);
        Assert.Contains("MapCommandExecutionResult", text);
    }

    [Fact]
    public void BudgetsTreeVerificationForm_UsesBudgetContributionLoader()
    {
        string text = ReadFile(
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains("HostBudgetTreeContributionLoader", text);
        Assert.Contains("LoadContributionAsync", text);
        Assert.Contains("TreeView", text);
    }

    [Fact]
    public void BudgetsDatabasePanel_UsesWebsitesStyleDatabaseWorkflow()
    {
        string text = ReadFile(
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsDatabasePanelForm.cs");

        Assert.Contains("Add-in Database", text);
        Assert.Contains("Copy from Expensa Prod", text);
        Assert.Contains("Copy new from dev template", text);
        Assert.Contains("Copy new from Sandbox DB", text);
        Assert.Contains("Open copied DB folder", text);
        Assert.Contains("HostBudgetRuntimeDatabaseSelectionService", text);
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
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}
