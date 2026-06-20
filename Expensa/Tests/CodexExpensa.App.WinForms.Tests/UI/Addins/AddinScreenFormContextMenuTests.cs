using System.Reflection;
using CodexExpensa.App.WinForms.UI.Addins;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Payees;
using CodexExpensa.Core.Domain.Transactions;
using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class AddinScreenFormContextMenuTests
{
    [Fact]
    public void BudgetRowContextMenu_WhenAddTransactionCapabilityAndRepositoriesExist_IsEnabled()
    {
        using AddinScreenForm form =
            new(
                CreateBudgetScreen(allowAddTransaction: true),
                assemblyPath: string.Empty,
                assemblyLastWriteUtc: null,
                accounts: new StubAccountRepository(),
                payees: new StubPayeeRepository(),
                transactions: new StubTransactionRepository());

        ToolStripMenuItem addTransaction =
            GetBudgetContextMenuAddTransactionItem(form);

        Assert.Equal("Add Transaction...", addTransaction.Text);
        Assert.True(addTransaction.Enabled);
    }

    [Fact]
    public void BudgetRowContextMenu_WhenRepositoriesAreMissing_RemainsEnabled()
    {
        using AddinScreenForm form =
            new(
                CreateBudgetScreen(allowAddTransaction: true),
                assemblyPath: string.Empty,
                assemblyLastWriteUtc: null);

        ToolStripMenuItem addTransaction =
            GetBudgetContextMenuAddTransactionItem(form);

        Assert.Equal("Add Transaction...", addTransaction.Text);
        Assert.True(addTransaction.Enabled);
    }

    private static AddinScreenModel CreateBudgetScreen(
        bool allowAddTransaction)
    {
        return new AddinScreenModel
        {
            Title = "June 2026",
            Columns =
            [
                "RowType",
                "Item",
                "Status",
                "Amount"
            ],
            Rows =
            [
                new Dictionary<string, object?>
                {
                    ["RowType"] = "Budget",
                    ["Item"] = "Power Company",
                    ["Status"] = null,
                    ["Amount"] = 125m
                }
            ],
            AllowAddTransaction = allowAddTransaction
        };
    }

    private static ToolStripMenuItem GetBudgetContextMenuAddTransactionItem(
        AddinScreenForm form)
    {
        FieldInfo field =
            typeof(AddinScreenForm).GetField(
                "_budgetRowMenu",
                BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException(
                "The budget context menu field was not found.");

        ContextMenuStrip menu =
            (ContextMenuStrip?)field.GetValue(form)
            ?? throw new InvalidOperationException(
                "The budget context menu was not created.");

        return Assert.IsType<ToolStripMenuItem>(menu.Items[0]);
    }

    private sealed class StubAccountRepository : IAccountRepository
    {
        public IReadOnlyList<Account> GetAll() =>
        [
            new Account
            {
                AccountId = "acct-1",
                AccountNickname = "Checking",
                SortIndex = 0,
                BankId = "bank-1",
                AccountNumber = "0001",
                BankName = "Bank",
                RoutingNumber = "123",
                AccountType = AccountType.Checking,
                IsActive = true
            }
        ];

        public Account? GetById(string accountId) =>
            GetAll().FirstOrDefault(account => account.AccountId == accountId);

        public void Add(Account account)
        {
        }

        public void Update(Account account)
        {
        }

        public void Delete(string accountId)
        {
        }
    }

    private sealed class StubPayeeRepository : IPayeeRepository
    {
        public IReadOnlyList<Payee> GetAll() =>
        [
            new Payee
            {
                PayeeId = "payee-1",
                PayeeName = "Power Company"
            }
        ];

        public Payee? GetById(string payeeId) =>
            GetAll().FirstOrDefault(payee => payee.PayeeId == payeeId);

        public Payee? GetByName(string payeeName) =>
            GetAll().FirstOrDefault(payee =>
                string.Equals(
                    payee.PayeeName,
                    payeeName,
                    StringComparison.OrdinalIgnoreCase));

        public void Add(Payee payee)
        {
        }

        public void Update(Payee payee)
        {
        }

        public void Delete(string payeeId)
        {
        }
    }

    private sealed class StubTransactionRepository : ITransactionRepository
    {
        public IReadOnlyList<Transaction> GetByAccountId(string accountId) =>
            [];

        public void Add(Transaction txn)
        {
        }

        public void Update(Transaction txn)
        {
        }

        public void Delete(int transactionId)
        {
        }
    }
}
