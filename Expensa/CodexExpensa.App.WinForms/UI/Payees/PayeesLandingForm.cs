using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.App.WinForms.Services;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Payees;

namespace CodexExpensa.App.WinForms.UI.Payees;

public sealed class PayeesLandingForm : Form
{
    private readonly IDatabaseSession _db;
    private readonly GridLayoutService _gridLayoutService;
    private readonly Action<string> _openPayeeDetails;
    private readonly Action<Payee>? _savePayee;

    private IReadOnlyList<Payee> _all = Array.Empty<Payee>();

    private readonly ToolStrip _tool;
    private readonly ToolStripLabel _lbl;
    private readonly ToolStripTextBox _txtSearch;

    private readonly DataGridView _grid;

    private string _sortColumn = nameof(Row.IncludeInBudgetTemplate);
    private bool _sortDescending = true;

    private bool _isBinding;
    private bool _isApplyingGridLayout;

    public PayeesLandingForm(
        IDatabaseSession db,
        Action<string> openPayeeDetails,
        Action<Payee>? savePayee = null)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _openPayeeDetails = openPayeeDetails ?? throw new ArgumentNullException(nameof(openPayeeDetails));
        _savePayee = savePayee;
        _gridLayoutService = new GridLayoutService(_db);

        Text = "Payees";
        FormBorderStyle = FormBorderStyle.None;
        TopLevel = false;
        Dock = DockStyle.Fill;

        _tool = new ToolStrip();
        _lbl = new ToolStripLabel("Search:");
        _txtSearch = new ToolStripTextBox
        {
            AutoSize = false,
            Width = 240
        };
        _txtSearch.TextChanged += (_, _) => Rebind();

        _tool.Items.Add(_lbl);
        _tool.Items.Add(_txtSearch);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            MultiSelect = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoGenerateColumns = false
        };

        DataGridViewCheckBoxColumn colTemplate = new()
        {
            Name = nameof(Row.IncludeInBudgetTemplate),
            HeaderText = "In Template",
            DataPropertyName = nameof(Row.IncludeInBudgetTemplate),
            Width = 95,
            Frozen = true
        };

        DataGridViewTextBoxColumn colName = new()
        {
            Name = nameof(Row.PayeeName),
            HeaderText = "Payee Name",
            DataPropertyName = nameof(Row.PayeeName),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            ReadOnly = true
        };

        DataGridViewTextBoxColumn colSort = new()
        {
            Name = nameof(Row.SortIndex),
            HeaderText = "Sort",
            DataPropertyName = nameof(Row.SortIndex),
            Width = 70,
            ReadOnly = true
        };

        DataGridViewCheckBoxColumn colActive = new()
        {
            Name = nameof(Row.IsActive),
            HeaderText = "Active",
            DataPropertyName = nameof(Row.IsActive),
            Width = 70,
            ReadOnly = true
        };

        _grid.Columns.Add(colTemplate);
        _grid.Columns.Add(colName);
        _grid.Columns.Add(colSort);
        _grid.Columns.Add(colActive);

        _grid.CellDoubleClick += Grid_CellDoubleClick;
        _grid.ColumnHeaderMouseClick += Grid_ColumnHeaderMouseClick;
        _grid.CurrentCellDirtyStateChanged += Grid_CurrentCellDirtyStateChanged;
        _grid.CellValueChanged += Grid_CellValueChanged;
        _grid.ColumnDisplayIndexChanged += Grid_LayoutChanged;
        _grid.ColumnWidthChanged += Grid_LayoutChanged;

        Controls.Add(_grid);
        Controls.Add(_tool);

        _tool.Dock = DockStyle.Top;

        Load += (_, _) => ApplySavedLayout();
    }

    public void SetPayees(IReadOnlyList<Payee> payees)
    {
        _all = payees ?? Array.Empty<Payee>();
        Rebind();
    }

    private void Rebind()
    {
        string search = _txtSearch.Text?.Trim() ?? string.Empty;

        IEnumerable<Payee> filtered = _all;

        if (!string.IsNullOrWhiteSpace(search))
        {
            filtered = filtered.Where(p =>
                p.PayeeName.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        IEnumerable<Row> rows = filtered.Select(p => new Row
        {
            PayeeId = p.PayeeId,
            PayeeName = p.PayeeName,
            IncludeInBudgetTemplate = p.IncludeInBudgetTemplate,
            SortIndex = p.SortIndex,
            IsActive = p.IsActive
        });

        rows = ApplySort(rows);

        _isBinding = true;
        try
        {
            _grid.DataSource = rows.ToList();
            UpdateSortGlyphs();

            if (!_isApplyingGridLayout)
            {
                ApplySavedLayout();
            }
        }
        finally
        {
            _isBinding = false;
        }
    }

    private IEnumerable<Row> ApplySort(IEnumerable<Row> rows)
    {
        return (_sortColumn, _sortDescending) switch
        {
            (nameof(Row.IncludeInBudgetTemplate), true) =>
                rows.OrderByDescending(r => r.IncludeInBudgetTemplate)
                    .ThenBy(r => r.PayeeName, StringComparer.OrdinalIgnoreCase),

            (nameof(Row.IncludeInBudgetTemplate), false) =>
                rows.OrderBy(r => r.IncludeInBudgetTemplate)
                    .ThenBy(r => r.PayeeName, StringComparer.OrdinalIgnoreCase),

            (nameof(Row.PayeeName), true) =>
                rows.OrderByDescending(r => r.PayeeName, StringComparer.OrdinalIgnoreCase),

            (nameof(Row.PayeeName), false) =>
                rows.OrderBy(r => r.PayeeName, StringComparer.OrdinalIgnoreCase),

            (nameof(Row.SortIndex), true) =>
                rows.OrderByDescending(r => r.SortIndex)
                    .ThenByDescending(r => r.PayeeName, StringComparer.OrdinalIgnoreCase),

            (nameof(Row.SortIndex), false) =>
                rows.OrderBy(r => r.SortIndex)
                    .ThenBy(r => r.PayeeName, StringComparer.OrdinalIgnoreCase),

            (nameof(Row.IsActive), true) =>
                rows.OrderByDescending(r => r.IsActive)
                    .ThenByDescending(r => r.PayeeName, StringComparer.OrdinalIgnoreCase),

            (nameof(Row.IsActive), false) =>
                rows.OrderBy(r => r.IsActive)
                    .ThenBy(r => r.PayeeName, StringComparer.OrdinalIgnoreCase),

            _ =>
                rows.OrderByDescending(r => r.IncludeInBudgetTemplate)
                    .ThenBy(r => r.PayeeName, StringComparer.OrdinalIgnoreCase)
        };
    }

    private void Grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        if (_grid.Rows[e.RowIndex].DataBoundItem is not Row row)
            return;

        _openPayeeDetails(row.PayeeId);
    }

    private void Grid_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.ColumnIndex < 0)
            return;

        string clickedColumn = _grid.Columns[e.ColumnIndex].Name;

        if (_sortColumn == clickedColumn)
        {
            _sortDescending = !_sortDescending;
        }
        else
        {
            _sortColumn = clickedColumn;

            _sortDescending = clickedColumn switch
            {
                nameof(Row.IncludeInBudgetTemplate) => true,
                _ => false
            };
        }

        Rebind();
    }

    private void Grid_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (_isBinding)
            return;

        if (_grid.IsCurrentCellDirty)
        {
            _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    private void Grid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_isBinding)
            return;

        if (e.RowIndex < 0 || e.ColumnIndex < 0)
            return;

        if (_grid.Columns[e.ColumnIndex].Name != nameof(Row.IncludeInBudgetTemplate))
            return;

        if (_grid.Rows[e.RowIndex].DataBoundItem is not Row row)
            return;

        Payee? payee = _all.FirstOrDefault(p => p.PayeeId == row.PayeeId);
        if (payee is null)
            return;

        payee.IncludeInBudgetTemplate = row.IncludeInBudgetTemplate;

        _savePayee?.Invoke(payee);

        Rebind();
    }

    private void ApplySavedLayout()
    {
        if (_grid.Columns.Count == 0)
            return;

        _isApplyingGridLayout = true;
        try
        {
            _gridLayoutService.LoadLayout(
                _grid,
                "Payees.GridLayout",
                forcedFirstColumn: nameof(Row.IncludeInBudgetTemplate),
                freezeForcedFirstColumn: false);
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

        if (_isBinding)
            return;

        if (!IsHandleCreated)
            return;

        _gridLayoutService.SaveLayout(_grid, "Payees.GridLayout");
    }

    private void UpdateSortGlyphs()
    {
        foreach (DataGridViewColumn column in _grid.Columns)
        {
            column.HeaderCell.SortGlyphDirection = SortOrder.None;
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
        }

        if (_grid.Columns.Contains(_sortColumn))
        {
            _grid.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                _sortDescending ? SortOrder.Descending : SortOrder.Ascending;
        }
    }

    private sealed class Row
    {
        public string PayeeId { get; init; } = string.Empty;

        public string PayeeName { get; init; } = string.Empty;

        public bool IncludeInBudgetTemplate { get; set; }

        public int SortIndex { get; init; }

        public bool IsActive { get; init; }
    }
}