using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class AddinScreenProviderIntegrationTests
{
    [Fact]
    public void Definitions_DeclareScreenProvidersForAllCurrentAddins()
    {
        string text =
            TestRepositoryPath.ReadExpensaFile(
                "UI",
                "Addins",
                "TreeAddinDefinition.cs");

        Assert.Contains("WebsiteScreenRuntimeRunner", text);
        Assert.Contains("BudgetScreenRuntimeRunner", text);
        Assert.Contains("PayeeScreenRuntimeRunner", text);
        Assert.Contains("ScreenRequestTypeName", text);
    }

    [Fact]
    public void MainForm_RoutesExtensionNodesToScreenInvoker()
    {
        string text =
            TestRepositoryPath.ReadExpensaFile(
                "UI",
                "MainForm.cs");

        Assert.Contains("ShowAddinScreenAsync(", text);
        Assert.Contains("TreeAddinKind.Websites", text);
        Assert.Contains("TreeAddinKind.Budgets", text);
        Assert.Contains("TreeAddinKind.Payees", text);

        Assert.DoesNotContain(
            "ShowBudgetMonth(cy, cm)",
            text);

        Assert.DoesNotContain(
            "ShowPayees();",
            text);
    }

    [Fact]
    public void SharedScreenForm_RendersProviderRows()
    {
        string text =
            TestRepositoryPath.ReadExpensaFile(
                "UI",
                "Addins",
                "AddinScreenForm.cs");

        Assert.Contains("CreateDataTable(screen)", text);
        Assert.Contains("DataGridView", text);
        Assert.Contains("Provider DLL modified", text);
    }
}
