using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class SafeDataGridViewBindingStructureTests
{
    [Fact]
    public void SafeDataGridViewBinding_SuppressesGridFormattingDialogs()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "SafeDataGridViewBinding.cs");

        Assert.Contains("DataGridViewDataErrorEventArgs", text);
        Assert.Contains("e.ThrowException = false", text);
    }

    [Fact]
    public void SafeDataGridViewBinding_ForcesImageLikeColumnsToText()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "SafeDataGridViewBinding.cs");

        Assert.Contains("ShouldForceString", text);
        Assert.Contains("column.ColumnName, \"Image\"", text);
        Assert.Contains("typeof(byte[])", text);
        Assert.Contains("System.Drawing.Image", text);
    }

    [Fact]
    public void DatabasePanelTemplate_AttachesSafeGridHandlingAndSanitizesDataSources()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Templates",
            "DatabasePanelTemplate.cs");

        Assert.Contains("SafeDataGridViewBinding.Attach(gridDataView)", text);
        Assert.Contains("SafeDataGridViewBinding.Sanitize(dataSource)", text);
    }

    [Fact]
    public void TreeTestTemplate_AttachesSafeGridHandlingAndSanitizesDataSources()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Templates",
            "TreeTestTemplate.cs");

        Assert.Contains("SafeDataGridViewBinding.Attach(gridData)", text);
        Assert.Contains("SafeDataGridViewBinding.Sanitize(dataSource)", text);
        Assert.Contains("SafeDataGridViewBinding.Sanitize(table)", text);
    }

    [Theory]
    [InlineData("Websites", "WebsitesDatabasePanelForm.cs")]
    [InlineData("Budgets", "BudgetsDatabasePanelForm.cs")]
    [InlineData("Payees", "PayeesDatabasePanelForm.cs")]
    public void DatabasePanelForms_UseTemplateGridBinding(
        string folder,
        string fileName)
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            folder,
            fileName);

        Assert.Contains("SetGridDataSource(data)", text);
        Assert.DoesNotContain("SafeDataGridViewBinding.Attach(gridDataView)", text);
        Assert.DoesNotContain("SafeDataGridViewBinding.Sanitize(data)", text);
    }

    [Fact]
    public void PayeesTreeLoadVerificationForm_UsesTemplateGridBinding()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesTreeLoadVerificationForm.cs");

        Assert.Contains("SetGridData(transactions)", text);
        Assert.DoesNotContain("SafeDataGridViewBinding.Attach(ResultGrid)", text);
        Assert.DoesNotContain("SafeDataGridViewBinding.Sanitize(transactions)", text);
    }
}
