using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Payees;
using CodexExpensa.Core.Domain.Transactions;
using Krypton.Toolkit.Suite.Extended.TreeGridView;
using System.Data;
using System.Windows.Forms.Integration;

namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class AddinScreenForm : Form
{
    private readonly AddinScreenModel _screen;
    private readonly IAccountRepository? _accounts;
    private readonly IPayeeRepository? _payees;
    private readonly ITransactionRepository? _transactions;
    private readonly AddinScreenSelection? _selection;
    private readonly Action? _saveDatabase;
    private readonly Func<Task>? _reloadAsync;
    private readonly Control _grid;
    private readonly ContextMenuStrip _budgetRowMenu;
    private readonly bool _canAddTransaction;
    private readonly bool _useBudgetTransactionGrid;

    public AddinScreenForm(
        AddinScreenModel screen,
        string assemblyPath,
        DateTime? assemblyLastWriteUtc,
        string assemblyVersion = "",
        DateTime? deployedUtc = null,
        IAccountRepository? accounts = null,
        IPayeeRepository? payees = null,
        ITransactionRepository? transactions = null,
        AddinScreenSelection? selection = null,
        Action? saveDatabase = null,
        Func<Task>? reloadAsync = null)
    {
        _screen = screen ?? throw new ArgumentNullException(nameof(screen));
        _accounts = accounts;
        _payees = payees;
        _transactions = transactions;
        _selection = selection;
        _saveDatabase = saveDatabase;
        _reloadAsync = reloadAsync;

        Text = screen.Title;
        FormBorderStyle = FormBorderStyle.None;
        Dock = DockStyle.Fill;

        _canAddTransaction =
            screen.AllowAddTransaction &&
            accounts is not null &&
            payees is not null &&
            transactions is not null;

        _useBudgetTransactionGrid =
            IsBudgetTransactionScreen(screen);

        bool showActionPanel =
            _canAddTransaction;

        TableLayoutPanel layout =
            new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = showActionPanel ? 4 : 3,
                Padding = new Padding(12)
            };

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        if (showActionPanel)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Label title =
            new()
            {
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Text = string.IsNullOrWhiteSpace(screen.Title)
                    ? "Extension screen"
                    : screen.Title
            };

        Label subtitle =
            new()
            {
                AutoSize = true,
                MaximumSize = new Size(1000, 0),
                Text = BuildSubtitle(
                    screen.Subtitle,
                    assemblyPath,
                    assemblyLastWriteUtc,
                    assemblyVersion,
                    deployedUtc)
            };

        _grid = _useBudgetTransactionGrid
            ? CreateBudgetTransactionGridHost(screen)
            : screen.IsHierarchical
            ? CreateHierarchyGrid(screen)
            : CreateFlatGrid(screen);

        _budgetRowMenu = CreateBudgetRowMenu();

        if (_grid is DataGridView dataGridView)
        {
            dataGridView.CellMouseDown += Grid_CellMouseDown;
        }

        if (_grid is ElementHost { Child: BudgetTransactionWpfGridControl budgetGrid })
        {
            budgetGrid.RowContextMenuRequested += BudgetGrid_RowContextMenuRequested;
            budgetGrid.TransactionRowDoubleClicked += (_, row) =>
                OpenTransactionDialog(row);
        }

        layout.Controls.Add(title, 0, 0);
        layout.Controls.Add(subtitle, 0, 1);

        int gridRow = 2;

        if (showActionPanel)
        {
            layout.Controls.Add(CreateActionPanel(), 0, 2);
            gridRow = 3;
        }

        layout.Controls.Add(_grid, 0, gridRow);
        Controls.Add(layout);
    }

    public static AddinScreenForm CreateFailure(
        string title,
        Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        AddinScreenModel model =
            new()
            {
                Title = string.IsNullOrWhiteSpace(title)
                    ? "Extension screen failed"
                    : title,
                Subtitle = exception.ToString(),
                Columns = ["Error"],
                Rows =
                [
                    new Dictionary<string, object?>
                    {
                        ["Error"] = exception.Message
                    }
                ]
            };

        return new AddinScreenForm(model, string.Empty, null);
    }

    private Control CreateActionPanel()
    {
        FlowLayoutPanel panel =
            new()
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 6, 0, 6)
            };

        if (_canAddTransaction)
        {
            Button addTransaction =
                new()
                {
                    AutoSize = true,
                    Text = "Add Transaction..."
                };

            addTransaction.Click += AddTransaction_Click;
            panel.Controls.Add(addTransaction);
        }

        Label help =
            new()
            {
                AutoSize = true,
                Margin = new Padding(12, 7, 0, 0),
                Text = _canAddTransaction
                    ? "Select a Budget row, then add its transaction."
                    : string.Empty
            };

        if (!string.IsNullOrWhiteSpace(help.Text))
        {
            panel.Controls.Add(help);
        }

        return panel;
    }

    private ContextMenuStrip CreateBudgetRowMenu()
    {
        ContextMenuStrip menu = new();

        ToolStripMenuItem addTransaction =
            new("Add Transaction...");
        addTransaction.Click += AddTransaction_Click;

        menu.Items.Add(addTransaction);
        return menu;
    }

    private void Grid_CellMouseDown(
        object? sender,
        DataGridViewCellMouseEventArgs e)
    {
        if (sender is not DataGridView grid)
        {
            return;
        }

        if (e.Button != MouseButtons.Right ||
            e.RowIndex < 0)
        {
            return;
        }

        grid.ClearSelection();
        grid.CurrentCell =
            GetSelectableCell(grid.Rows[e.RowIndex], e.ColumnIndex);
        grid.Rows[e.RowIndex].Selected = true;

        string rowType =
            Convert.ToString(
                GetCellValue(grid.Rows[e.RowIndex], "RowType"))
            ?? string.Empty;

        if (string.Equals(
                rowType,
                "Budget",
                StringComparison.OrdinalIgnoreCase))
        {
            _budgetRowMenu.Show(
                grid,
                grid.PointToClient(Cursor.Position));
            return;
        }

        if (string.Equals(
                rowType,
                "Transaction",
                StringComparison.OrdinalIgnoreCase))
        {
            OpenTransactionDialog(grid.Rows[e.RowIndex]);
        }
    }

    private void BudgetGrid_RowContextMenuRequested(
        object? sender,
        BudgetTransactionWpfGridRow row)
    {
        if (row.IsTransaction)
        {
            OpenTransactionDialog(row);
            return;
        }

        _budgetRowMenu.Show(
            this,
            PointToClient(Cursor.Position));
    }

    private void OpenTransactionDialog(DataGridViewRow row)
    {
        // Placeholder for the upcoming transaction dialog wiring.
        _ = row;
    }

    private void OpenTransactionDialog(BudgetTransactionWpfGridRow row)
    {
        // Placeholder for the upcoming transaction dialog wiring.
        _ = row;
    }

    private static DataGridViewCell GetSelectableCell(
        DataGridViewRow row,
        int preferredColumnIndex)
    {
        if (preferredColumnIndex >= 0 &&
            preferredColumnIndex < row.Cells.Count &&
            row.Cells[preferredColumnIndex].Visible)
        {
            return row.Cells[preferredColumnIndex];
        }

        foreach (DataGridViewCell cell in row.Cells)
        {
            if (cell.Visible)
            {
                return cell;
            }
        }

        return row.Cells[0];
    }

    private async void AddTransaction_Click(
        object? sender,
        EventArgs e)
    {
        await AddTransactionFromSelectedBudgetRowAsync();
    }

    private async Task AddTransactionFromSelectedBudgetRowAsync()
    {
        try
        {
            if (_accounts is null ||
                _payees is null ||
                _transactions is null)
            {
                MessageBox.Show(
                    this,
                    "Add Transaction is not available for this screen.",
                    "Add Transaction",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            BudgetSelection? selectedBudget =
                GetSelectedBudgetRow();

            if (selectedBudget is null)
            {
                MessageBox.Show(
                    this,
                    "Select a Budget row first.",
                    "Add Transaction",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!string.Equals(
                    selectedBudget.RowType,
                    "Budget",
                    StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    this,
                    "Select a Budget parent row, not an existing transaction.",
                    "Add Transaction",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            string payeeName =
                selectedBudget.PayeeName;

            Payee? payee =
                string.IsNullOrWhiteSpace(payeeName)
                    ? null
                    : _payees.GetByName(payeeName);

            if (payee is null)
            {
                MessageBox.Show(
                    this,
                    $"No Payee record matches the Budget item '{payeeName}'.",
                    "Add Transaction",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            using AddTransactionForm dialog =
                new(
                    _accounts.GetAll(),
                    payee.PayeeName,
                    GetDefaultTransactionDate());

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            _transactions.Add(
                new Transaction
                {
                    TransactionId = 0,
                    AccountId = dialog.AccountId,
                    PayeeId = payee.PayeeId,
                    Status = dialog.Status,
                    Amount = dialog.Amount,
                    StartDate = dialog.StartDate,
                    Confirm = dialog.Confirm,
                    Note = dialog.Note
                });

            // Expensa runs against an in-memory database, while add-in screen
            // providers read the persisted file. Save before reloading so the
            // newly added transaction is visible immediately.
            _saveDatabase?.Invoke();

            if (_reloadAsync is not null)
            {
                await _reloadAsync();
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.ToString(),
                "Add Transaction failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private BudgetSelection? GetSelectedBudgetRow()
    {
        if (_grid is DataGridView { CurrentRow: not null } dataGridView)
        {
            return new BudgetSelection(
                Convert.ToString(
                    GetCellValue(dataGridView.CurrentRow, "RowType"))
                ?? string.Empty,
                GetBudgetPayeeName(dataGridView.CurrentRow));
        }

        if (_grid is ElementHost { Child: BudgetTransactionWpfGridControl budgetGrid } &&
            budgetGrid.SelectedRow is BudgetTransactionWpfGridRow selectedRow)
        {
            return new BudgetSelection(
                selectedRow.GetValue("RowType"),
                GetBudgetPayeeName(selectedRow));
        }

        return null;
    }

    private DateTime GetDefaultTransactionDate()
    {
        if (_selection?.Year is not int year ||
            _selection.Month is not int month ||
            month < 1 ||
            month > 12)
        {
            return DateTime.Today;
        }

        DateTime today = DateTime.Today;

        if (today.Year == year &&
            today.Month == month)
        {
            return today;
        }

        return new DateTime(year, month, 1);
    }

    private static string GetBudgetPayeeName(DataGridViewRow row)
    {
        string value =
            Convert.ToString(
                GetCellValue(row, "Item"))
            ?? string.Empty;

        return value
            .Replace("[+]", string.Empty, StringComparison.Ordinal)
            .Replace("[-]", string.Empty, StringComparison.Ordinal)
            .Trim();
    }

    private static string GetBudgetPayeeName(BudgetTransactionWpfGridRow row)
    {
        string payee =
            row.GetValue("Payee");

        if (!string.IsNullOrWhiteSpace(payee))
        {
            return payee.Trim();
        }

        return row.GetValue("Item")
            .Replace("[+]", string.Empty, StringComparison.Ordinal)
            .Replace("[-]", string.Empty, StringComparison.Ordinal)
            .Trim();
    }

    private sealed record BudgetSelection(
        string RowType,
        string PayeeName);

    private static object? GetCellValue(
        DataGridViewRow row,
        string columnName)
    {
        return row.DataGridView?.Columns.Contains(columnName) == true
            ? row.Cells[columnName].Value
            : null;
    }

    private static DataGridView CreateFlatGrid(
        AddinScreenModel screen)
    {
        return new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoGenerateColumns = true,
            AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.DisplayedCells,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            DataSource = CreateDataTable(screen)
        };
    }

    private static Control CreateBudgetTransactionGridHost(
        AddinScreenModel screen)
    {
        BudgetTransactionWpfGridControl grid = new();
        grid.LoadRows(CreateDataTable(screen));

        return new ElementHost
        {
            Dock = DockStyle.Fill,
            Child = grid
        };
    }

    private static DataGridView CreateHierarchyGrid(
        AddinScreenModel screen)
    {
        if (string.IsNullOrWhiteSpace(screen.IdColumnName) ||
            string.IsNullOrWhiteSpace(screen.ParentIdColumnName))
        {
            throw new InvalidOperationException(
                "A hierarchical extension screen must specify its ID and parent ID columns.");
        }

        DataTable table = CreateDataTable(screen);

        if (!table.Columns.Contains(screen.IdColumnName) ||
            !table.Columns.Contains(screen.ParentIdColumnName))
        {
            throw new InvalidOperationException(
                "The hierarchical extension screen did not return its declared identity columns.");
        }

        KryptonTreeGridView grid =
            new()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.DisplayedCells,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ShowLines = true,
                FontParentBold = true,
                UseParentRelationship = true,
                IsOneLevel = false,
                IdColumnName = screen.IdColumnName,
                ParentIdColumnName = screen.ParentIdColumnName
            };

        grid.DataSource = table;

        HideColumns(grid, screen.HiddenColumns);

        grid.ExpandAll();
        return grid;
    }

    private static bool IsBudgetTransactionScreen(
        AddinScreenModel screen)
    {
        IEnumerable<string> columns =
            screen.Columns
                .Concat(screen.HiddenColumns)
                .Concat(screen.Rows.SelectMany(static row => row.Keys));

        return screen.AllowAddTransaction &&
            columns.Contains(
                "BudgetMonthRowId",
                StringComparer.OrdinalIgnoreCase) &&
            columns.Contains(
                "TransactionId",
                StringComparer.OrdinalIgnoreCase);
    }

    private static void HideColumns(
        DataGridView grid,
        IEnumerable<string> columnNames)
    {
        foreach (string columnName in columnNames)
        {
            HideColumn(grid, columnName);
        }
    }

    private static void HideColumn(
        DataGridView grid,
        string columnName)
    {
        DataGridViewColumn? column =
            FindGridColumn(
                grid,
                columnName);

        if (column is not null)
        {
            column.Visible = false;
        }
    }

    private static DataGridViewColumn? FindGridColumn(
        DataGridView grid,
        string columnName)
    {
        if (grid.Columns.Contains(columnName))
        {
            return grid.Columns[columnName];
        }

        foreach (DataGridViewColumn column in grid.Columns)
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

    private static bool IsTransactionRow(DataGridViewRow row)
    {
        string rowType =
            Convert.ToString(
                GetCellValue(row, "RowType"))
            ?? string.Empty;

        return string.Equals(
            rowType,
            "Transaction",
            StringComparison.OrdinalIgnoreCase);
    }

    private static DataTable CreateDataTable(
        AddinScreenModel screen)
    {
        DataTable table = new();

        IEnumerable<string> columns =
            screen.Columns
                .Concat(screen.HiddenColumns)
                .Concat(screen.Rows.SelectMany(static row => row.Keys))
                .Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (string column in columns)
        {
            if (!string.IsNullOrWhiteSpace(column) &&
                !table.Columns.Contains(column))
            {
                table.Columns.Add(column, typeof(object));
            }
        }

        foreach (IReadOnlyDictionary<string, object?> sourceRow in screen.Rows)
        {
            DataRow row = table.NewRow();

            foreach (DataColumn column in table.Columns)
            {
                if (sourceRow.TryGetValue(
                        column.ColumnName,
                        out object? value))
                {
                    row[column] = value ?? DBNull.Value;
                }
            }

            table.Rows.Add(row);
        }

        return table;
    }

    private static string BuildSubtitle(
        string subtitle,
        string assemblyPath,
        DateTime? assemblyLastWriteUtc,
        string assemblyVersion,
        DateTime? deployedUtc)
    {
        List<string> lines = [];

        if (!string.IsNullOrWhiteSpace(subtitle))
        {
            lines.Add(subtitle);
        }

        if (!string.IsNullOrWhiteSpace(assemblyVersion))
        {
            lines.Add($"Add-in version: {assemblyVersion}");
        }

        if (deployedUtc is not null)
        {
            lines.Add(
                $"Deployed: {deployedUtc.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
        }

        if (!string.IsNullOrWhiteSpace(assemblyPath))
        {
            lines.Add($"Provider: {assemblyPath}");
        }

        if (assemblyLastWriteUtc is not null)
        {
            lines.Add(
                $"Provider DLL modified: {assemblyLastWriteUtc.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
        }

        return string.Join(Environment.NewLine, lines);
    }
}
