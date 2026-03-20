using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Windows.Forms.VisualStyles;
using CodexExpensa.App.WinForms.Services;
using CodexExpensa.App.WinForms.UI.Transactions;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Transactions;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.App.WinForms.UI.Budgets;

public sealed class BudgetMonthForm : Form
{
    private const int DeleteButtonWidth = 18;
    private const int DeleteButtonHeight = 16;
    private const int DeleteButtonLeftMargin = 2;
    private const int DeleteButtonTextGap = 4;

    private readonly IDatabaseSession _db;
    private readonly ITransactionRepository _transactions;
    private readonly int _year;
    private readonly int _month;
    private readonly string _budgetMonthId;
    private readonly GridLayoutService _gridLayoutService;

    private readonly Panel _rightPanel;
    private readonly Panel _gridHost;
    private readonly DataGridView _grid;
    private readonly ContextMenuStrip _payeeContextMenu;
    private readonly ToolStripMenuItem _changeStatusMenuItem;
    private readonly ToolStripMenuItem _viewStatusHistoryMenuItem;

    private BindingList<BudgetRow> _rows = new();

    private string? _contextTransactionId;
    private string? _contextPayeeName;

    private bool _isApplyingGridLayout;
    private bool _isSavingAmount;

    public BudgetMonthForm(IDatabaseSession db, ITransactionRepository transactions, int year, int month)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
        _year = year;
        _month = month;
        _budgetMonthId = $"{year:D4}-{month:D2}";
        _gridLayoutService = new GridLayoutService(_db);

        Text = $"Budget {new DateTime(year, month, 1):MMMM yyyy}";
        Width = 1200;
        Height = 700;
        StartPosition = FormStartPosition.CenterParent;

        _rightPanel = new Panel
        {
            Dock = DockStyle.Right,
            Width = 260,
            BackColor = SystemColors.Control
        };

        _gridHost = new Panel
        {
            Dock = DockStyle.Fill
        };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = false,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.CellSelect,
            MultiSelect = false
        };

        EnableDoubleBuffering(_grid);

        _grid.CellContentClick += Grid_CellContentClick;
        _grid.CellMouseClick += Grid_CellMouseClick;
        _grid.CellMouseDown += Grid_CellMouseDown;
        _grid.CellFormatting += Grid_CellFormatting;
        _grid.CellPainting += Grid_CellPainting;
        _grid.CellEndEdit += Grid_CellEndEdit;
        _grid.ColumnDisplayIndexChanged += Grid_LayoutChanged;
        _grid.ColumnWidthChanged += Grid_LayoutChanged;

        _payeeContextMenu = new ContextMenuStrip();

        _changeStatusMenuItem = new ToolStripMenuItem("Change Status...");
        _changeStatusMenuItem.Click += ChangeStatusMenuItem_Click;

        _viewStatusHistoryMenuItem = new ToolStripMenuItem("View Status History...");
        _viewStatusHistoryMenuItem.Click += ViewStatusHistoryMenuItem_Click;

        _payeeContextMenu.Items.Add(_changeStatusMenuItem);
        _payeeContextMenu.Items.Add(_viewStatusHistoryMenuItem);

        _gridHost.Controls.Add(_grid);

        Controls.Add(_gridHost);
        Controls.Add(_rightPanel);

        BuildRightPanelPlaceholder();
        BuildGridColumns();

        Load += BudgetMonthForm_Load;
        FormClosing += BudgetMonthForm_FormClosing;
    }

    private void BudgetMonthForm_Load(object? sender, EventArgs e)
    {
        LoadBudgetRows();
        ApplySavedGridLayout();
    }

    private void BudgetMonthForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_isApplyingGridLayout)
            return;

        _gridLayoutService.SaveLayout(_grid, "BudgetMonth.GridLayout");
    }

    private void BuildRightPanelPlaceholder()
    {
        Label title = new()
        {
            Text = "Future Controls",
            Dock = DockStyle.Top,
            Height = 28,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 0, 0),
            Font = new Font(Font, FontStyle.Bold)
        };

        Label note = new()
        {
            Text = "Reserved space for totals,\r\nfilters, color rules,\r\nand transaction controls.",
            Dock = DockStyle.Top,
            Height = 90,
            Padding = new Padding(8),
            AutoSize = false
        };

        _rightPanel.Controls.Add(note);
        _rightPanel.Controls.Add(title);
    }

    private void BuildGridColumns()
    {
        _grid.Columns.Clear();

        DataGridViewTextBoxColumn payee = new()
        {
            Name = "PayeeName",
            DataPropertyName = nameof(BudgetRow.PayeeName),
            HeaderText = "Payee",
            Frozen = true,
            ReadOnly = true,
            Width = 260
        };

        DataGridViewTextBoxColumn account = new()
        {
            Name = "Account",
            DataPropertyName = nameof(BudgetRow.AccountName),
            HeaderText = "Account",
            Width = 220,
            ReadOnly = true
        };

        DataGridViewTextBoxColumn amount = new()
        {
            Name = "Amount",
            DataPropertyName = nameof(BudgetRow.PlannedAmount),
            HeaderText = "Amount",
            Width = 90,
            ReadOnly = false,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
        };

        DataGridViewLinkColumn status = new()
        {
            Name = "Status",
            DataPropertyName = nameof(BudgetRow.Status),
            HeaderText = "Status",
            Width = 110,
            LinkBehavior = LinkBehavior.HoverUnderline,
            TrackVisitedState = false,
            UseColumnTextForLinkValue = false
        };

        DataGridViewTextBoxColumn confirmationNumber = new()
        {
            Name = "ConfirmationNumber",
            DataPropertyName = nameof(BudgetRow.ConfirmDisplay),
            HeaderText = "Confirmation Number",
            Width = 220,
            ReadOnly = true
        };

        DataGridViewTextBoxColumn transactionId = new()
        {
            Name = "TransactionId",
            DataPropertyName = nameof(BudgetRow.TransactionId),
            Visible = false,
            ReadOnly = true
        };

        _grid.Columns.Add(payee);
        _grid.Columns.Add(account);
        _grid.Columns.Add(amount);
        _grid.Columns.Add(status);
        _grid.Columns.Add(confirmationNumber);
        _grid.Columns.Add(transactionId);

        _grid.Columns["PayeeName"]!.DisplayIndex = 0;
        _grid.Columns["PayeeName"]!.Frozen = true;
    }

    private void LoadBudgetRows()
    {
        DataTable table = _db.QueryDataTable(
            "BudgetMonthPayee.SelectByBudgetMonthId",
            new DbParameter[]
            {
                new SqliteParameter("@BudgetMonthId", _budgetMonthId)
            });

        List<BudgetRow> list = new();

        foreach (DataRow r in table.Rows)
        {
            BudgetRow row = new()
            {
                BudgetMonthPayeeId = r["BudgetMonthPayeeId"]?.ToString() ?? string.Empty,
                BudgetMonthId = r["BudgetMonthId"]?.ToString() ?? string.Empty,
                PayeeId = r["PayeeId"]?.ToString() ?? string.Empty,
                PayeeName = r["PayeeName"]?.ToString() ?? string.Empty,
                SortIndex = Convert.ToInt32(r["SortIndex"]),
                PlannedAmount = Convert.ToDecimal(r["PlannedAmount"]),
                AccountId = r["AccountId"] == DBNull.Value ? null : r["AccountId"]?.ToString(),
                AccountName = table.Columns.Contains("AccountDisplayName") && r["AccountDisplayName"] != DBNull.Value
                    ? r["AccountDisplayName"]?.ToString()
                    : null,
                Group = string.Empty,
                ConfirmationNumber = string.Empty,
                TransactionId = null,
                ActualAmount = null,
                IsCleared = false,
                Transaction = null
            };

            if (table.Columns.Contains("TransactionId") && r["TransactionId"] != DBNull.Value)
                row.TransactionId = r["TransactionId"]?.ToString();

            if (table.Columns.Contains("IsCleared") && r["IsCleared"] != DBNull.Value)
                row.IsCleared = Convert.ToBoolean(r["IsCleared"]);

            if (table.Columns.Contains("Group") && r["Group"] != DBNull.Value)
                row.Group = r["Group"]?.ToString() ?? string.Empty;

            if (table.Columns.Contains("ConfirmationNumber") && r["ConfirmationNumber"] != DBNull.Value)
                row.ConfirmationNumber = r["ConfirmationNumber"]?.ToString() ?? string.Empty;

            if (table.Columns.Contains("ActualAmount") && r["ActualAmount"] != DBNull.Value)
                row.ActualAmount = Convert.ToDecimal(r["ActualAmount"]);

            if (!string.IsNullOrWhiteSpace(row.TransactionId) && int.TryParse(row.TransactionId, out int transactionIdValue))
                row.Transaction = CreateTransactionFromRow(row, transactionIdValue);

            list.Add(row);
        }

        _rows = new BindingList<BudgetRow>(list);
        _grid.DataSource = _rows;
    }

    private void ApplySavedGridLayout()
    {
        _isApplyingGridLayout = true;
        try
        {
            _gridLayoutService.LoadLayout(
                _grid,
                "BudgetMonth.GridLayout",
                forcedFirstColumn: "PayeeName",
                freezeForcedFirstColumn: true);
        }
        finally
        {
            _isApplyingGridLayout = false;
        }
    }

    private void Grid_LayoutChanged(object? sender, EventArgs e)
    {
        if (_isApplyingGridLayout)
            return;

        if (!IsHandleCreated)
            return;

        _gridLayoutService.SaveLayout(_grid, "BudgetMonth.GridLayout");
    }

    private void Grid_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
    {
        if (_isSavingAmount)
            return;

        if (e.RowIndex < 0 || e.ColumnIndex < 0)
            return;

        DataGridViewColumn? column = _grid.Columns[e.ColumnIndex];
        if (column is null || column.Name != "Amount")
            return;

        BudgetRow? row = _grid.Rows[e.RowIndex].DataBoundItem as BudgetRow;
        if (row is null)
            return;

        try
        {
            _isSavingAmount = true;

            _db.Execute(
                "BudgetMonthPayee.UpdatePlannedAmount",
                new DbParameter[]
                {
                    new SqliteParameter("@BudgetMonthPayeeId", row.BudgetMonthPayeeId),
                    new SqliteParameter("@PlannedAmount", row.PlannedAmount)
                });

            if (row.Transaction is not null)
            {
                Transaction updatedTransaction = new()
                {
                    AccountId = row.Transaction.AccountId,
                    PayeeId = row.Transaction.PayeeId,
                    Status = row.Transaction.Status,
                    Amount = row.PlannedAmount,
                    StartDate = row.Transaction.StartDate,
                    ConfirmationNumber = row.Transaction.ConfirmationNumber,
                    Note = row.Transaction.Note
                };

                if (row.Transaction.TransactionId > 0)
                    updatedTransaction.SetTransactionId(row.Transaction.TransactionId);

                _transactions.Update(updatedTransaction);
                ApplyTransactionToRow(row, updatedTransaction);
            }

            if (e.RowIndex >= 0 && e.RowIndex < _rows.Count)
                _rows.ResetItem(e.RowIndex);

            _grid.InvalidateRow(e.RowIndex);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"{ex.GetType().Name}: {ex.Message}",
                "Save Planned Amount",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _isSavingAmount = false;
        }
    }

    private void Grid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
            return;

        DataGridViewColumn? clickedColumn = _grid.Columns[e.ColumnIndex];
        if (clickedColumn is null || clickedColumn.Name != "Status")
            return;

        BudgetRow? row = _grid.Rows[e.RowIndex].DataBoundItem as BudgetRow;
        if (row is null)
            return;

        HandleStatusClick(row);

        if (e.RowIndex >= 0 && e.RowIndex < _rows.Count)
            _rows.ResetItem(e.RowIndex);

        _grid.InvalidateRow(e.RowIndex);
    }

    private void Grid_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left || e.RowIndex < 0 || e.ColumnIndex < 0)
            return;

        DataGridViewColumn? clickedColumn = _grid.Columns[e.ColumnIndex];
        if (clickedColumn is null || clickedColumn.Name != "PayeeName")
            return;

        BudgetRow? row = _grid.Rows[e.RowIndex].DataBoundItem as BudgetRow;
        if (row is null || row.PlannedAmount != 0m)
            return;

        Rectangle deleteBounds = GetDeleteButtonBounds(e.RowIndex, e.ColumnIndex);
        Point clickPoint = new(e.X, e.Y);

        if (!deleteBounds.Contains(clickPoint))
            return;

        HandleDeleteRowClick(row);
    }

    private void HandleDeleteRowClick(BudgetRow row)
    {
        if (row.PlannedAmount != 0m)
            return;

        MessageBox.Show(
            this,
            $"TODO: Delete budget row for {row.PayeeName}",
            "Delete Budget Row",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void HandleStatusClick(BudgetRow row)
    {
        string status = row.Status;

        if (string.IsNullOrWhiteSpace(status))
        {
            MessageBox.Show(
                this,
                "Enter a planned amount greater than zero to move this row into Projected.",
                "Budget Status",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        if (string.Equals(status, "Projected", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(row.AccountId))
            {
                MessageBox.Show(
                    this,
                    "Assign an account before creating a transaction.",
                    "Budget Status",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            Transaction transaction = new()
            {
                AccountId = row.AccountId,
                PayeeId = row.PayeeId,
                Status = TransactionStatus.Outstanding,
                Amount = row.PlannedAmount,
                StartDate = new DateTime(_year, _month, 1),
                ConfirmationNumber = $"UI dev outstanding {DateTime.Now:g}",
                Note = $"Created from BudgetMonthForm for {row.PayeeName}"
            };

            int parsedTransactionId = ParseTransactionId(row.TransactionId);
            if (parsedTransactionId > 0)
                transaction.SetTransactionId(parsedTransactionId);

            _transactions.Add(transaction);
            ApplyTransactionToRow(row, transaction);
            return;
        }

        if (string.Equals(status, "Outstanding", StringComparison.OrdinalIgnoreCase))
        {
            if (row.Transaction is null)
                return;

            _transactions.ChangeStatus(
                row.Transaction.TransactionId,
                TransactionStatus.Cleared,
                TransactionChangeReason.ManualCorrection,
                "Marked cleared from BudgetMonthForm status link.",
                "BudgetMonthForm");

            row.Transaction.Status = TransactionStatus.Cleared;
            ApplyTransactionToRow(row, row.Transaction);
            return;
        }

        if (string.Equals(status, "Cleared", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Invalid", StringComparison.OrdinalIgnoreCase))
        {
            ShowChangeStatusDialog(row);
        }
    }

    private void Grid_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
            return;

        DataGridViewColumn? clickedColumn = _grid.Columns[e.ColumnIndex];
        if (clickedColumn is null || (clickedColumn.Name != "PayeeName" && clickedColumn.Name != "Status"))
            return;

        BudgetRow? row = _grid.Rows[e.RowIndex].DataBoundItem as BudgetRow;
        if (row is null)
            return;

        _contextTransactionId = row.TransactionId;
        _contextPayeeName = row.PayeeName;

        bool hasTransaction = !string.IsNullOrWhiteSpace(_contextTransactionId);

        _changeStatusMenuItem.Enabled = hasTransaction;
        _viewStatusHistoryMenuItem.Enabled = hasTransaction;

        Point screenPoint = _grid.PointToScreen(new Point(e.X, e.Y));
        _payeeContextMenu.Show(screenPoint);
    }

    private void ChangeStatusMenuItem_Click(object? sender, EventArgs e)
    {
        if (!TryGetContextRow(out BudgetRow? row))
            return;

        ShowChangeStatusDialog(row);
    }

    private void ViewStatusHistoryMenuItem_Click(object? sender, EventArgs e)
    {
        if (!TryGetContextRow(out BudgetRow? row))
            return;

        if (row.Transaction is null)
        {
            MessageBox.Show(
                this,
                "No transaction exists for this row.",
                "View Status History",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        IReadOnlyList<TxnStatusLog> history = _transactions.GetStatusHistory(row.Transaction.TransactionId);

        using TransactionStatusHistoryForm form = new(
            row.Transaction.TransactionId,
            row.PayeeName,
            history);

        form.ShowDialog(this);
    }

    private bool TryGetContextRow(out BudgetRow? row)
    {
        row = null;

        if (string.IsNullOrWhiteSpace(_contextTransactionId))
        {
            MessageBox.Show(
                this,
                "No transaction exists for this row.",
                "Transaction",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return false;
        }

        row = _rows.FirstOrDefault(r => string.Equals(r.TransactionId, _contextTransactionId, StringComparison.Ordinal));

        if (row is null || row.Transaction is null)
        {
            MessageBox.Show(
                this,
                "The selected transaction could not be found.",
                "Transaction",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private void ShowChangeStatusDialog(BudgetRow row)
    {
        if (row.Transaction is null)
            return;

        using ChangeTransactionStatusForm form = new(row.Transaction.Status);

        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        _transactions.ChangeStatus(
            row.Transaction.TransactionId,
            form.SelectedStatus,
            form.SelectedReason,
            form.ReasonText,
            "BudgetMonthForm");

        row.Transaction.Status = form.SelectedStatus;
        ApplyTransactionToRow(row, row.Transaction);

        int rowIndex = _rows.IndexOf(row);
        if (rowIndex >= 0)
            _rows.ResetItem(rowIndex);

        _grid.Invalidate();
    }

    private void Grid_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
            return;

        DataGridViewColumn? column = _grid.Columns[e.ColumnIndex];
        if (column is null || column.Name != "PayeeName")
            return;

        if (_grid.Rows[e.RowIndex].DataBoundItem is not BudgetRow row)
            return;

        if (row.PlannedAmount != 0m)
            return;

        e.PaintBackground(e.CellBounds, true);
        e.Paint(e.CellBounds, DataGridViewPaintParts.Border | DataGridViewPaintParts.Focus);

        Rectangle buttonBounds = GetDeleteButtonBounds(e.RowIndex, e.ColumnIndex);
        ButtonRenderer.DrawButton(
            e.Graphics,
            buttonBounds,
            "X",
            Font,
            false,
            PushButtonState.Normal);

        Rectangle textBounds = new(
            buttonBounds.Right + DeleteButtonTextGap,
            e.CellBounds.Y,
            Math.Max(0, e.CellBounds.Width - (buttonBounds.Right - e.CellBounds.X) - DeleteButtonTextGap),
            e.CellBounds.Height);

        TextRenderer.DrawText(
            e.Graphics,
            row.PayeeName,
            e.CellStyle.Font ?? Font,
            textBounds,
            e.CellStyle.ForeColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

        e.Handled = true;
    }

    private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        if (sender is not DataGridView grid)
            return;

        if (grid.Rows[e.RowIndex].DataBoundItem is not BudgetRow row)
            return;

        DataGridViewRow gridRow = grid.Rows[e.RowIndex];

        if (grid.Columns[e.ColumnIndex].Name == "PayeeName")
        {
            if (row.PlannedAmount == 0m)
            {
                e.CellStyle.Padding = new Padding(
                    DeleteButtonLeftMargin + DeleteButtonWidth + DeleteButtonTextGap,
                    0,
                    0,
                    0);
            }
            else
            {
                e.CellStyle.Padding = Padding.Empty;
            }
        }

        if (e.ColumnIndex != 0)
            return;

        gridRow.DefaultCellStyle.BackColor = Color.Empty;

        string status = row.Status;

        if (string.Equals(status, "Cleared", StringComparison.OrdinalIgnoreCase))
        {
            gridRow.DefaultCellStyle.BackColor = Color.LightGreen;
            return;
        }

        if (string.Equals(status, "Outstanding", StringComparison.OrdinalIgnoreCase))
        {
            gridRow.DefaultCellStyle.BackColor = Color.LightYellow;
            return;
        }

        if (string.Equals(status, "Projected", StringComparison.OrdinalIgnoreCase))
        {
            gridRow.DefaultCellStyle.BackColor = Color.MistyRose;
            return;
        }

        if (string.Equals(status, "Invalid", StringComparison.OrdinalIgnoreCase))
        {
            gridRow.DefaultCellStyle.BackColor = Color.LightGray;
            return;
        }

        if (string.Equals(row.AccountId, "4", StringComparison.OrdinalIgnoreCase))
        {
            gridRow.DefaultCellStyle.BackColor = Color.LightCyan;
            return;
        }

        if (string.Equals(row.AccountId, "9", StringComparison.OrdinalIgnoreCase))
        {
            gridRow.DefaultCellStyle.BackColor = Color.LemonChiffon;
        }
    }

    private Rectangle GetDeleteButtonBounds(int rowIndex, int columnIndex)
    {
        Rectangle cellRect = _grid.GetCellDisplayRectangle(columnIndex, rowIndex, false);

        int x = cellRect.X + DeleteButtonLeftMargin;
        int y = cellRect.Y + Math.Max(0, (cellRect.Height - DeleteButtonHeight) / 2);

        return new Rectangle(x, y, DeleteButtonWidth, DeleteButtonHeight);
    }

    private static void EnableDoubleBuffering(DataGridView grid)
    {
        typeof(DataGridView)
            .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(grid, true);

        grid.GetType()
            .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(grid, true);
    }

    private Transaction CreateTransactionFromRow(BudgetRow row, int transactionId)
    {
        TransactionStatus transactionStatus = row.IsCleared
            ? TransactionStatus.Cleared
            : TransactionStatus.Outstanding;

        Transaction transaction = new()
        {
            AccountId = row.AccountId ?? string.Empty,
            PayeeId = row.PayeeId,
            Status = transactionStatus,
            Amount = row.ActualAmount ?? row.PlannedAmount,
            StartDate = new DateTime(_year, _month, 1),
            ConfirmationNumber = row.ConfirmationNumber,
            Note = $"Loaded into BudgetMonthForm for {row.PayeeName}"
        };

        if (transactionId > 0)
            transaction.SetTransactionId(transactionId);

        return transaction;
    }

    private void ApplyTransactionToRow(BudgetRow row, Transaction transaction)
    {
        row.Transaction = transaction;
        row.TransactionId = transaction.TransactionId == 0 ? row.TransactionId : transaction.TransactionId.ToString();
        row.ActualAmount = transaction.Amount;
        row.IsCleared = transaction.Status == TransactionStatus.Cleared;
        row.ConfirmationNumber = transaction.ConfirmationNumber ?? string.Empty;
    }

    private static int ParseTransactionId(string? transactionId)
    {
        return int.TryParse(transactionId, out int parsed)
            ? parsed
            : 0;
    }
}
