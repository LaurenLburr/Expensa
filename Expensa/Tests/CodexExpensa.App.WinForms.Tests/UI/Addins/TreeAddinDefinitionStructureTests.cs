using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class TreeAddinDefinitionStructureTests
{
    [Fact]
    public void DefaultTreeAddins_IncludesWebsitesAndBudgets()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinDefinition.cs");

        Assert.Contains("WebsitesAddin", text);
        Assert.Contains("BudgetsAddin", text);
        Assert.Contains("WebsitesAddin.WebsiteLoadRuntimeSmokeRunner", text);
        Assert.Contains("BudgetsAddin.BudgetLoadRuntimeSmokeRunner", text);
        Assert.Contains("WebsitesAddin.WebsiteLoadRequest", text);
        Assert.Contains("BudgetsAddin.BudgetLoadRequest", text);
    }

    [Fact]
    public void DefaultTreeAddins_DefinesModuleFolderAndAssemblyNames()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinDefinition.cs");

        Assert.Contains("ModuleFolderName = \"WebsitesAddin\"", text);
        Assert.Contains("ModuleFolderName = \"BudgetsAddin\"", text);
        Assert.Contains("AssemblyFileName = \"WebsitesAddin.dll\"", text);
        Assert.Contains("AssemblyFileName = \"BudgetsAddin.dll\"", text);
    }
}
