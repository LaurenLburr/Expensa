using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class AddTransactionScreenStructureTests
{
    [Fact]
    public void AddinScreenModel_DeclaresAddTransactionCapability()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI", "Addins", "AddinScreenModel.cs");

        Assert.Contains(
            "public bool AllowAddTransaction { get; init; }",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void AddinScreenForm_AddsTransactionThroughExistingRepositories()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI", "Addins", "AddinScreenForm.cs");

        Assert.Contains("Add Transaction...", text, StringComparison.Ordinal);
        Assert.Contains("CreateBudgetRowMenu()", text, StringComparison.Ordinal);
        Assert.Contains("addTransaction.Click += AddTransaction_Click;", text, StringComparison.Ordinal);
        Assert.Contains("AddTransactionFromSelectedBudgetRowAsync()", text, StringComparison.Ordinal);
        Assert.Contains("_payees.GetByName(payeeName)", text, StringComparison.Ordinal);
        Assert.Contains("using AddTransactionForm dialog", text, StringComparison.Ordinal);
        Assert.Contains("_transactions.Add(", text, StringComparison.Ordinal);
        Assert.Contains("_saveDatabase?.Invoke();", text, StringComparison.Ordinal);
        Assert.Contains("await _reloadAsync();", text, StringComparison.Ordinal);
    }

    [Fact]
    public void MainForm_PassesRepositoriesAndReloadCallbackToAddinScreen()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI", "MainForm.cs");

        Assert.Contains("_accounts,", text, StringComparison.Ordinal);
        Assert.Contains("_payees,", text, StringComparison.Ordinal);
        Assert.Contains("_transactions,", text, StringComparison.Ordinal);
        Assert.Contains("saveDatabase: _dbSession.Save", text, StringComparison.Ordinal);
        Assert.Contains("reloadAsync:", text, StringComparison.Ordinal);
    }
}
