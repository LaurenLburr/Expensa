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
    public void DefaultTreeAddins_DefinesProjectAssemblyNames()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinDefinition.cs");

        Assert.Contains("ProjectAssemblyName = \"WebsitesAddin\"", text);
        Assert.Contains("ProjectAssemblyName = \"BudgetsAddin\"", text);
        Assert.Contains("ProjectAssemblyName = \"PayeesAddin\"", text);
        Assert.DoesNotContain("ModuleFolderName =", text);
        Assert.DoesNotContain("AssemblyFileName =", text);
    }

    [Fact]
    public void DefaultTreeAddins_DefinesDisplaySortOrder()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinDefinition.cs");

        Assert.Contains("public int DisplaySort", text);
        Assert.Contains("LoadRegisteredDisplaySortOverrides", text);
        Assert.Contains("ExtensionMgr.db", text);
        Assert.Contains("[ExtensionProjectRegistration]", text);
        Assert.Contains("[SortOrder]", text);
        Assert.Contains("GetDisplaySort(displaySortOverrides, \"WebsitesAddin\", 100)", text);
        Assert.Contains("GetDisplaySort(displaySortOverrides, \"BudgetsAddin\", 200)", text);
        Assert.Contains("GetDisplaySort(displaySortOverrides, \"PayeesAddin\", 300)", text);
        Assert.Contains("OrderBy(static addin => addin.DisplaySort)", text);
    }
}
