using System.ComponentModel;
using System.Data;
using System.Data.Common;
using CodexExpensa.App.WinForms.Services;
using CodexExpensa.Core.Abstractions;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.App.WinForms.UI.Budgets;

public sealed class BudgetMonthForm : Form
{
    private readonly IDatabaseSession _db;
    private readonly int _year;
    private readonly int _month;
    private readonly string _budgetMonthId;
    private readonly GridLayoutService _gridLayoutService;

    private readonly Panel _rightPanel;
    private readonly Panel _gridHost;
    private readonly DataGridView _grid;
    private readonly ContextMenuStrip _payeeContextMenu;
    private readonly ToolStripMenuItem _fixEntryMenuItem;

    private BindingList<BudgetRow> _rows = new();

    private string? _contextTransactionId;
    private string? _contextPayeeName;

    private bool _isApplyingGridLayout;
    private bool _isSavingAmount;

    public BudgetMonthForm(IDatabaseSession db, int year, int month)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
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

        _grid.CellContentClick += Grid_CellContentClick;
        _grid.CellMouseDown += Grid_CellMouseDown;
        _grid.CellFormatting += Grid_CellFormatting;
        _grid.CellEndEdit += Grid_CellEndEdit;
        _grid.ColumnDisplayIndexChanged += Grid_LayoutChanged;
        _grid.ColumnWidthChanged += Grid_LayoutChanged;

        _payeeContextMenu = new ContextMenuStrip();
        _fixEntryMenuItem = new ToolStripMenuItem("Fix Entry...");
        _fixEntryMenuItem.Click += FixEntryMenuItem_Click;
        _payeeContextMenu.Items.Add(_fixEntryMenuItem);

        _gridHost.Controls.Add(_grid);

        Controls.Add(_gridHost);
        Controls.Add(_rightPanel);

        BuildRightPanelPlaceholder();
        BuildGridColumns();

        Load += BudgetMonthForm_Load;
    }

    private void BudgetMonthForm_Load(object? sender, EventArgs e)
    {
        LoadBudgetRows();
        ApplySavedGridLayout();
    }

    private void BuildRightPanelPlaceholder()
    {
        Label title = new()
        {
            Text = "Future Controls",
            Dock = DockStyle.Top,
            Height = 28,
            TextAlign = ContentAlignment.MiddleLeft,
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
            Width = 220
        };

        DataGridViewTextBoxColumn bankAcct = new()
        {
            Name = "BankAcct",
            DataPropertyName = nameof(BudgetRow.AccountId),
            HeaderText = "Bank Acct",
            Width = 90,
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

        DataGridViewLinkColumn action = new()
        {
            Name = "Action",
            DataPropertyName = nameof(BudgetRow.Action),
            HeaderText = "Action",
            Width = 95,
            LinkBehavior = LinkBehavior.HoverUnderline,
            TrackVisitedState = false,
            UseColumnTextForLinkValue = false
        };

        DataGridViewTextBoxColumn group = new()
        {
            Name = "Group",
            DataPropertyName = nameof(BudgetRow.Group),
            HeaderText = "Group",
            Width = 70,
            ReadOnly = true
        };

        DataGridViewTextBoxColumn confirm = new()
        {
            Name = "Confirm",
            DataPropertyName = nameof(BudgetRow.Confirm),
            HeaderText = "Confirm",
            Width = 180,
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
        _grid.Columns.Add(bankAcct);
        _grid.Columns.Add(amount);
        _grid.Columns.Add(action);
        _grid.Columns.Add(group);
        _grid.Columns.Add(confirm);
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
                Group = string.Empty,
                Confirm = string.Empty,
                TransactionId = null,
                ActualAmount = null,
                IsCleared = false
            };

            if (table.Columns.Contains("TransactionId") && r["TransactionId"] != DBNull.Value)
                row.TransactionId = r["TransactionId"]?.ToString();

            if (table.Columns.Contains("IsCleared") && r["IsCleared"] != DBNull.Value)
                row.IsCleared = Convert.ToBoolean(r["IsCleared"]);

            if (table.Columns.Contains("Group") && r["Group"] != DBNull.Value)
                row.Group = r["Group"]?.ToString() ?? string.Empty;

            if (table.Columns.Contains("Confirm") && r["Confirm"] != DBNull.Value)
                row.Confirm = r["Confirm"]?.ToString() ?? string.Empty;

            if (table.Columns.Contains("ActualAmount") && r["ActualAmount"] != DBNull.Value)
                row.ActualAmount = Convert.ToDecimal(r["ActualAmount"]);

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
        if (clickedColumn is null || clickedColumn.Name != "Action")
            return;

        BudgetRow? row = _grid.Rows[e.RowIndex].DataBoundItem as BudgetRow;
        if (row is null)
            return;

        switch (row.Action)
        {
            case "Add":
                AddTransaction(row);
                break;

            case "Clear":
                ClearTransaction(row);
                break;
        }

        _grid.Refresh();
    }

    private void AddTransaction(BudgetRow row)
    {
        MessageBox.Show(
            this,
            $"TODO: Add transaction for {row.PayeeName}",
            "Add Transaction",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void ClearTransaction(BudgetRow row)
    {
        MessageBox.Show(
            this,
            $"TODO: Mark transaction cleared for {row.PayeeName}",
            "Clear Transaction",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void Grid_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
            return;

        DataGridViewColumn? clickedColumn = _grid.Columns[e.ColumnIndex];
        if (clickedColumn is null || clickedColumn.Name != "PayeeName")
            return;

        BudgetRow? row = _grid.Rows[e.RowIndex].DataBoundItem as BudgetRow;
        if (row is null)
            return;

        _contextTransactionId = row.TransactionId;
        _contextPayeeName = row.PayeeName;

        _fixEntryMenuItem.Enabled = !string.IsNullOrWhiteSpace(_contextTransactionId);

        Point screenPoint = _grid.PointToScreen(new Point(e.X, e.Y));
        _payeeContextMenu.Show(screenPoint);
    }

    private void FixEntryMenuItem_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_contextTransactionId))
        {
            MessageBox.Show(
                this,
                "No transaction exists for this row.",
                "Fix Entry",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        MessageBox.Show(
            this,
            $"TODO: Fix entry for {_contextPayeeName}",
            "Fix Entry",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        BudgetRow? row = _grid.Rows[e.RowIndex].DataBoundItem as BudgetRow;
        if (row is null)
            return;

        if (string.Equals(row.Action, "Clear", StringComparison.OrdinalIgnoreCase))
        {
            e.CellStyle.BackColor = Color.LightGreen;
            return;
        }

        if (string.Equals(row.Action, "Add", StringComparison.OrdinalIgnoreCase))
        {
            e.CellStyle.BackColor = Color.MistyRose;
            return;
        }

        if (string.Equals(row.AccountId, "4", StringComparison.OrdinalIgnoreCase))
        {
            e.CellStyle.BackColor = Color.LightCyan;
            return;
        }

        if (string.Equals(row.AccountId, "9", StringComparison.OrdinalIgnoreCase))
        {
            e.CellStyle.BackColor = Color.LemonChiffon;
        }
    }
}