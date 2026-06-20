using System.Data;

namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class BudgetTransactionGrid : DataGridView
{
    private const string ToggleColumnName = "Toggle";

    private readonly HashSet<string> _collapsedBudgetRowIds =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly IReadOnlyList<string> _hiddenColumns;
    private DataTable? _sourceRows;

    public event EventHandler<DataGridViewRow>? TransactionRowDoubleClicked;

    public BudgetTransactionGrid(
        IReadOnlyList<string> hiddenColumns)
    {
        _hiddenColumns =
            hiddenColumns
                .Concat(
                [
                    "RowType",
                    "NodeId",
                    "ParentNodeId",
                    "HierarchyLevel",
                    "SortIndex",
                    "BudgetMonthRowId",
                    "PayeeId",
                    "TransactionId"
                ])
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

        Dock = DockStyle.Fill;
        ReadOnly = true;
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        AutoGenerateColumns = true;
        AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.DisplayedCells;
        RowHeadersVisible = false;
        SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        MultiSelect = false;
    }

    protected override void OnCellFormatting(
        DataGridViewCellFormattingEventArgs e)
    {
        base.OnCellFormatting(e);

        if (e.RowIndex < 0 ||
            e.RowIndex >= Rows.Count ||
            e.ColumnIndex < 0 ||
            !string.Equals(
                Columns[e.ColumnIndex].Name,
                ToggleColumnName,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        e.Value =
            GetToggleValue(Rows[e.RowIndex]);
        e.FormattingApplied = true;
    }

    protected override void OnCellContentClick(
        DataGridViewCellEventArgs e)
    {
        base.OnCellContentClick(e);

        if (e.RowIndex < 0 ||
            e.RowIndex >= Rows.Count ||
            e.ColumnIndex < 0 ||
            !string.Equals(
                Columns[e.ColumnIndex].Name,
                ToggleColumnName,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        ToggleBudgetRow(Rows[e.RowIndex]);
    }

    public void LoadRows(
        DataTable sourceRows)
    {
        _sourceRows =
            sourceRows ?? throw new ArgumentNullException(nameof(sourceRows));

        RenderRows();
    }

    public void ExpandAll()
    {
        if (_collapsedBudgetRowIds.Count == 0)
        {
            return;
        }

        _collapsedBudgetRowIds.Clear();
        RenderRows();
    }

    public void CollapseAll()
    {
        if (_sourceRows is null)
        {
            return;
        }

        _collapsedBudgetRowIds.Clear();

        foreach (DataRow row in _sourceRows.Rows)
        {
            if (IsTransactionRow(row))
            {
                continue;
            }

            string budgetMonthRowId =
                GetDataRowString(row, "BudgetMonthRowId");

            if (HasChildTransactions(budgetMonthRowId))
            {
                _collapsedBudgetRowIds.Add(budgetMonthRowId);
            }
        }

        RenderRows();
    }

    protected override void OnCellDoubleClick(
        DataGridViewCellEventArgs e)
    {
        base.OnCellDoubleClick(e);

        if (e.RowIndex < 0 ||
            e.RowIndex >= Rows.Count)
        {
            return;
        }

        DataGridViewRow row =
            Rows[e.RowIndex];

        if (IsTransactionRow(row))
        {
            TransactionRowDoubleClicked?.Invoke(
                this,
                row);
            return;
        }

        ToggleBudgetRow(row);
    }

    private void RenderRows()
    {
        if (_sourceRows is null)
        {
            return;
        }

        DataTable visibleRows =
            _sourceRows.Clone();

        foreach (DataRow row in _sourceRows.Rows)
        {
            string budgetMonthRowId =
                GetDataRowString(row, "BudgetMonthRowId");

            if (IsTransactionRow(row) &&
                _collapsedBudgetRowIds.Contains(budgetMonthRowId))
            {
                continue;
            }

            DataRow displayRow =
                visibleRows.NewRow();

            displayRow.ItemArray =
                row.ItemArray.ToArray();

            if (visibleRows.Columns.Contains("Item"))
            {
                displayRow["Item"] =
                    FormatDisplayItem(row);
            }

            visibleRows.Rows.Add(displayRow);
        }

        DataSource = visibleRows;
        EnsureToggleColumn();
        DisableSorting();
        HideColumns(_hiddenColumns);
    }

    private string FormatDisplayItem(
        DataRow row)
    {
        string item =
            GetDataRowString(row, "Item");

        if (IsTransactionRow(row))
        {
            return "    " + item.TrimStart();
        }

        string budgetMonthRowId =
            GetDataRowString(row, "BudgetMonthRowId");

        return item;
    }

    private void EnsureToggleColumn()
    {
        if (Columns.Contains(ToggleColumnName))
        {
            Columns[ToggleColumnName].DisplayIndex = 0;
            return;
        }

        DataGridViewButtonColumn toggleColumn =
            new()
            {
                Name = ToggleColumnName,
                HeaderText = string.Empty,
                Width = 28,
                MinimumWidth = 28,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                FlatStyle = FlatStyle.System,
                UseColumnTextForButtonValue = false
            };

        Columns.Insert(0, toggleColumn);
    }

    private void ToggleBudgetRow(
        DataGridViewRow row)
    {
        if (IsTransactionRow(row))
        {
            return;
        }

        string budgetMonthRowId =
            GetCellString(
                row,
                "BudgetMonthRowId");

        if (string.IsNullOrWhiteSpace(budgetMonthRowId) ||
            !HasChildTransactions(budgetMonthRowId))
        {
            return;
        }

        if (!_collapsedBudgetRowIds.Add(budgetMonthRowId))
        {
            _collapsedBudgetRowIds.Remove(budgetMonthRowId);
        }

        RenderRows();
    }

    private string GetToggleValue(
        DataGridViewRow row)
    {
        string budgetMonthRowId =
            GetCellString(row, "BudgetMonthRowId");

        if (IsTransactionRow(row) ||
            !HasChildTransactions(budgetMonthRowId))
        {
            return string.Empty;
        }

        return _collapsedBudgetRowIds.Contains(budgetMonthRowId)
            ? "+"
            : "-";
    }

    private bool HasChildTransactions(
        string budgetMonthRowId)
    {
        if (_sourceRows is null ||
            string.IsNullOrWhiteSpace(budgetMonthRowId))
        {
            return false;
        }

        foreach (DataRow row in _sourceRows.Rows)
        {
            if (IsTransactionRow(row) &&
                string.Equals(
                    GetDataRowString(row, "BudgetMonthRowId"),
                    budgetMonthRowId,
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private void DisableSorting()
    {
        foreach (DataGridViewColumn column in Columns)
        {
            column.SortMode =
                DataGridViewColumnSortMode.NotSortable;
        }
    }

    private void HideColumns(
        IEnumerable<string> columnNames)
    {
        foreach (string columnName in columnNames)
        {
            HideColumn(columnName);
        }
    }

    private void HideColumn(
        string columnName)
    {
        DataGridViewColumn? column =
            FindColumn(columnName);

        if (column is not null)
        {
            column.Visible = false;
        }
    }

    private DataGridViewColumn? FindColumn(
        string columnName)
    {
        if (Columns.Contains(columnName))
        {
            return Columns[columnName];
        }

        foreach (DataGridViewColumn column in Columns)
        {
            if (string.Equals(
                    column.Name,
                    columnName,
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    column.HeaderText,
                    columnName,
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    column.DataPropertyName,
                    columnName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return column;
            }
        }

        return null;
    }

    private static bool IsTransactionRow(
        DataRow row)
    {
        string rowType =
            GetDataRowString(row, "RowType");

        return string.Equals(
            rowType,
            "Transaction",
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsTransactionRow(
        DataGridViewRow row)
    {
        string rowType =
            GetCellString(row, "RowType");

        return string.Equals(
            rowType,
            "Transaction",
            StringComparison.OrdinalIgnoreCase);
    }

    private static string GetDataRowString(
        DataRow row,
        string columnName)
    {
        if (!row.Table.Columns.Contains(columnName))
        {
            return string.Empty;
        }

        return Convert.ToString(row[columnName])
            ?? string.Empty;
    }

    private static string GetCellString(
        DataGridViewRow row,
        string columnName)
    {
        object? value =
            row.DataGridView?.Columns.Contains(columnName) == true
                ? row.Cells[columnName].Value
                : null;

        return Convert.ToString(value)
            ?? string.Empty;
    }
}
