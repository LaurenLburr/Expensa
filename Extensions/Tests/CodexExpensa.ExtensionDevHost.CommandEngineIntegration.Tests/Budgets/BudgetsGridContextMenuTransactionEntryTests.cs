using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsGridContextMenuTransactionEntryTests
{
    [Fact]
    public void VerificationForm_AddsRowSpecificTransactionContextMenu()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains("ContextMenuStrip _budgetGridContextMenu", text, StringComparison.Ordinal);
        Assert.Contains("Enter Transaction...", text, StringComparison.Ordinal);
        Assert.Contains("Enter Payment...", text, StringComparison.Ordinal);
        Assert.Contains("_budgetRowsGrid.RowContextMenuRequested", text, StringComparison.Ordinal);
        Assert.Contains("ShowBudgetGridContextMenu(row)", text, StringComparison.Ordinal);
        Assert.Contains("_enterTransactionMenuItem.Visible = !isTransactionRow;", text, StringComparison.Ordinal);
        Assert.Contains("_enterPaymentMenuItem.Visible = isTransactionRow;", text, StringComparison.Ordinal);
        Assert.Contains("_enterTransactionMenuItem.Enabled = !isTransactionRow;", text, StringComparison.Ordinal);
        Assert.DoesNotContain("_enterTransactionMenuItem.Enabled = !isTransactionRow && hasPayee;", text, StringComparison.Ordinal);
        Assert.Contains("_budgetGridContextMenu.Show(", text, StringComparison.Ordinal);
    }

    [Fact]
    public void VerificationForm_UsesPayeeColumnsForTransactionEntry()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains("GetCellString(currentRow, \"Payee\")", text, StringComparison.Ordinal);
        Assert.Contains("GetCellString(currentRow, \"PayeeId\")", text, StringComparison.Ordinal);
        Assert.Contains("Fix the test data or SqlCatalog query", text, StringComparison.Ordinal);
        Assert.Contains("BudgetTransactionWpfGridRow? currentRow", text, StringComparison.Ordinal);
        Assert.Contains("row.GetValue(columnName)", text, StringComparison.Ordinal);
        Assert.DoesNotContain("EnsurePayeeId(", text, StringComparison.Ordinal);
        Assert.DoesNotContain("FindPayeeId(", text, StringComparison.Ordinal);
        Assert.DoesNotContain("INSERT INTO [Payee]", text, StringComparison.Ordinal);
    }

    [Fact]
    public void VerificationForm_PersistsTransactionEntryToRuntimeDatabase()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains("INSERT INTO [Txn]", text, StringComparison.Ordinal);
        Assert.Contains("UPDATE [Txn]", text, StringComparison.Ordinal);
        Assert.Contains("[Status] = @Status", text, StringComparison.Ordinal);
        Assert.Contains("_databaseSession?.Save();", text, StringComparison.Ordinal);
        Assert.Contains("ReloadMemoryDatabase();", text, StringComparison.Ordinal);
        Assert.Contains("LoadBudgetMonthRowsForSelectedBudget(TestTreeView.SelectedNode);", text, StringComparison.Ordinal);
    }

    [Fact]
    public void TransactionEntryDialog_ExposesPaymentDefaults()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetTransactionEntryDialog.cs");

        Assert.Contains("BudgetTransactionEntryDialog", text, StringComparison.Ordinal);
        Assert.Contains("acceptButtonText", text, StringComparison.Ordinal);
        Assert.Contains("$\"{title} - {payeeName}\"", text, StringComparison.Ordinal);
        Assert.Contains("defaultStatus", text, StringComparison.Ordinal);
        Assert.Contains("defaultAmount", text, StringComparison.Ordinal);
        Assert.Contains("Projected", text, StringComparison.Ordinal);
        Assert.Contains("Outstanding", text, StringComparison.Ordinal);
        Assert.Contains("Cleared", text, StringComparison.Ordinal);
    }
}
