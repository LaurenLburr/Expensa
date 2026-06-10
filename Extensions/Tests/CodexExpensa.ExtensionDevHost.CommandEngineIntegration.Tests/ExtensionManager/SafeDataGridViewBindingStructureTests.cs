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

    [Theory]
    [InlineData("Websites", "WebsitesDatabasePanelForm.cs")]
    [InlineData("Budgets", "BudgetsDatabasePanelForm.cs")]
    [InlineData("Payees", "PayeesDatabasePanelForm.cs")]
    public void DatabasePanelForms_AttachSafeGridHandlingAndSanitizeDataTables(
        string folder,
        string fileName)
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            folder,
            fileName);

        Assert.Contains("SafeDataGridViewBinding.Attach(gridDataView)", text);
        Assert.Contains("SafeDataGridViewBinding.Sanitize(data)", text);
    }

    [Fact]
    public void PayeesTreeLoadVerificationForm_AttachesSafeGridHandlingAndSanitizesTransactionTable()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesTreeLoadVerificationForm.cs");

        Assert.Contains("SafeDataGridViewBinding.Attach(ResultGrid)", text);
        Assert.Contains("SafeDataGridViewBinding.Sanitize(transactions)", text);
    }
}
