using System.Data;
using System.Reflection;
using CodexExpensa.App.WinForms.UI.Addins;
using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class BudgetTransactionGridTests
{
    [Fact]
    public void DoubleClickTransactionRow_RaisesTransactionRowDoubleClicked()
    {
        using BudgetTransactionGrid grid =
            new(["BudgetMonthRowId", "TransactionId"]);

        InitializeGrid(grid);
        grid.LoadRows(CreateRows());

        DataGridViewRow? doubleClickedRow = null;
        grid.TransactionRowDoubleClicked += (_, row) =>
            doubleClickedRow = row;

        DoubleClickRow(grid, rowIndex: 1);

        Assert.NotNull(doubleClickedRow);
        Assert.Equal("Transaction", doubleClickedRow.Cells["RowType"].Value);
        Assert.Equal("    Rent payment", doubleClickedRow.Cells["Item"].Value);
    }

    private static void DoubleClickRow(
        BudgetTransactionGrid grid,
        int rowIndex)
    {
        MethodInfo method =
            typeof(BudgetTransactionGrid).GetMethod(
                "OnCellDoubleClick",
                BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException(
                "BudgetTransactionGrid did not expose its double-click handler.");

        method.Invoke(
            grid,
            [new DataGridViewCellEventArgs(columnIndex: 0, rowIndex)]);
    }

    private static void InitializeGrid(
        BudgetTransactionGrid grid)
    {
        grid.BindingContext =
            new BindingContext();

        grid.CreateControl();
    }

    private static DataTable CreateRows()
    {
        DataTable table = new();
        table.Columns.Add("RowType", typeof(object));
        table.Columns.Add("Item", typeof(object));
        table.Columns.Add("Payee", typeof(object));
        table.Columns.Add("Amount", typeof(object));
        table.Columns.Add("NodeId", typeof(object));
        table.Columns.Add("BudgetMonthRowId", typeof(object));
        table.Columns.Add("TransactionId", typeof(object));

        AddRow(table, "Budget", "Rent", "Rent", 1200m, "rent-row", null);

        AddRow(table, "Transaction", "Rent payment", "Rent", 1200m, "rent-row", 1);

        AddRow(table, "Budget", "Power", "Power", 180m, "power-row", null);
        AddRow(table, "Transaction", "Power payment", "Power", 180m, "power-row", 2);

        return table;
    }

    private static void AddRow(
        DataTable table,
        string rowType,
        string item,
        string payee,
        decimal amount,
        string budgetMonthRowId,
        int? transactionId)
    {
        DataRow row =
            table.NewRow();

        row["RowType"] = rowType;
        row["Item"] = item;
        row["Payee"] = payee;
        row["Amount"] = amount;
        row["NodeId"] = rowType + ":" + budgetMonthRowId;
        row["BudgetMonthRowId"] = budgetMonthRowId;
        row["TransactionId"] = transactionId is null
            ? DBNull.Value
            : transactionId.Value;

        table.Rows.Add(row);
    }
}
