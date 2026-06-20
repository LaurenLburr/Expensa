using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class CurrentDeployedAddinsIntegrationTests
{
    [Fact]
    public void DefaultTreeAddins_IncludesWebsitesBudgetsAndPayees()
    {
        string text =
            TestRepositoryPath.ReadExpensaFile(
                "UI",
                "Addins",
                "TreeAddinDefinition.cs");

        Assert.Contains("WebsitesAddin", text);
        Assert.Contains("BudgetsAddin", text);
        Assert.Contains("PayeesAddin", text);
        Assert.Contains(
            "PayeesAddin.PayeeLoadRuntimeSmokeRunner",
            text);
        Assert.Contains(
            "PayeesAddin.PayeeLoadRequest",
            text);
    }

    [Fact]
    public void BudgetFactory_MapsCurrentPreviousAndTemplates()
    {
        string text =
            TestRepositoryPath.ReadExpensaFile(
                "UI",
                "Addins",
                "TreeAddinTreeNodeFactory.cs");

        Assert.Contains("BudgetCurrentMonth", text);
        Assert.Contains("BudgetPreviousMonth", text);
        Assert.Contains("BudgetTemplates", text);
        Assert.Contains("Budget.Current.", text);
        Assert.Contains("Budget.Month.", text);
        Assert.Contains("Budget.Template", text);
    }

    [Fact]
    public void PayeeNodes_OpenExistingPayeesForm()
    {
        string factoryText =
            TestRepositoryPath.ReadExpensaFile(
                "UI",
                "Addins",
                "TreeAddinTreeNodeFactory.cs");

        string mainFormText =
            TestRepositoryPath.ReadExpensaFile(
                "UI",
                "MainForm.cs");

        Assert.Contains(
            "CreatePayeesRootNode",
            factoryText);

        Assert.Contains(
            "PayeeTreeNodePayload",
            factoryText);

        Assert.Contains(
            "e.Node.Tag is PayeeTreeNodePayload",
            mainFormText);

        Assert.Contains(
            "new PayeesForm(_payees)",
            mainFormText);
    }

    [Fact]
    public void RootToolTip_ShowsExactLoadedDll()
    {
        string resultText =
            TestRepositoryPath.ReadExpensaFile(
                "UI",
                "Addins",
                "TreeAddinRuntimeResult.cs");

        string factoryText =
            TestRepositoryPath.ReadExpensaFile(
                "UI",
                "Addins",
                "TreeAddinTreeNodeFactory.cs");

        string mainFormText =
            TestRepositoryPath.ReadExpensaFile(
                "UI",
                "MainForm.cs");

        Assert.Contains("AssemblyPath", resultText);
        Assert.Contains(
            "AssemblyLastWriteTimeUtc",
            resultText);
        Assert.Contains(
            "Loaded from:",
            factoryText);
        Assert.Contains(
            "treeNav.ShowNodeToolTips = true",
            mainFormText);
    }

}
