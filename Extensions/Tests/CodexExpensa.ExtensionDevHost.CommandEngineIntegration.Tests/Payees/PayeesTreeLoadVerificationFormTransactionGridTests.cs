using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Payees;

public sealed class PayeesTreeLoadVerificationFormTransactionGridTests
{
    [Fact]
    public void PayeesTreeLoadVerificationForm_UsesTemplateGridForSelectedPayeeTransactions()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesTreeLoadVerificationForm.cs");

        Assert.Contains("TreeTestTemplate", text, StringComparison.Ordinal);
        Assert.Contains("TestTreeView.AfterSelect", text, StringComparison.Ordinal);
        Assert.Contains("LoadTransactionsForSelectedPayee", text, StringComparison.Ordinal);
        Assert.Contains("LoadTransactionsForPayee", text, StringComparison.Ordinal);
        Assert.Contains("SetGridData", text, StringComparison.Ordinal);
        Assert.Contains("FROM [Txn]", text, StringComparison.Ordinal);
        Assert.Contains("WHERE t.[PayeeId] = @PayeeId", text, StringComparison.Ordinal);
    }

    [Fact]
    public void PayeesTreeLoadVerificationForm_LoadsAddinRuntimeDatabaseIntoMemory()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesTreeLoadVerificationForm.cs");

        Assert.Contains("AddinId = \"PayeesAddin\"", text, StringComparison.Ordinal);
        Assert.Contains("GetRuntimeDatabaseLocation(AddinId)", text, StringComparison.Ordinal);
        Assert.Contains("add-in runtime database", text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("AddinMemoryDatabaseSession", text, StringComparison.Ordinal);
        Assert.Contains("ReloadMemoryDatabase", text, StringComparison.Ordinal);
        Assert.Contains("loaded into memory", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("GetDefaultExpensaDatabasePath", text, StringComparison.Ordinal);
        Assert.DoesNotContain("LocalApplicationData", text, StringComparison.Ordinal);
    }

    [Fact]
    public void SandboxTransactionSeedScript_InsertsIntoTxnOnly()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "SeedSandboxPayeeTransactions.sql");

        Assert.Contains("INSERT INTO [Txn]", text, StringComparison.Ordinal);
        Assert.Contains("Settings(TargetPayeeId)", text, StringComparison.Ordinal);
        Assert.Contains("Sandbox generated transaction", text, StringComparison.Ordinal);
        Assert.DoesNotContain("ATTACH DATABASE", text, StringComparison.OrdinalIgnoreCase);
    }
}
