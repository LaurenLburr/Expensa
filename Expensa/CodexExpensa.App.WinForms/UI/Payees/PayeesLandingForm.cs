using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.Core.Domain.Payees;

namespace CodexExpensa.App.WinForms.UI.Payees;

public sealed class PayeesLandingForm : Form
{
    private readonly Action<string> _openPayeeDetails;

    private IReadOnlyList<Payee> _all = Array.Empty<Payee>();

    private readonly ToolStrip _tool;
    private readonly ToolStripLabel _lbl;
    private readonly ToolStripTextBox _txtSearch;

    private readonly DataGridView _grid;

    private string _sortColumn = nameof(Row.PayeeName);
    private bool _sortDescending = false;

    public PayeesLandingForm(Action<string> openPayeeDetails)
    {
        _openPayeeDetails = openPayeeDetails ?? throw new ArgumentNullException(nameof(openPayeeDetails));

        Text = "Payees";
        FormBorderStyle = FormBorderStyle.None;

        _tool = new ToolStrip { Dock = DockStyle.Top };
        _lbl = new ToolStripLabel("Search:");
        _txtSearch = new ToolStripTextBox { AutoSize = false, Width = 300 };
        _txtSearch.TextChanged += (_, _) => ApplyFilter();

        _tool.Items.Add(_lbl);
        _tool.Items.Add(_txtSearch);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };

        _grid.ColumnHeaderMouseClick += Grid_ColumnHeaderMouseClick;
        _grid.CellDoubleClick += (_, _) => OpenSelected();
        _grid.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                OpenSelected();
            }
        };

        Controls.Add(_grid);
        Controls.Add(_tool);
    }

    private void Grid_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        var column = _grid.Columns[e.ColumnIndex];

        if (column.DataPropertyName == _sortColumn)
            _sortDescending = !_sortDescending;
        else
        {
            _sortColumn = column.DataPropertyName;
            _sortDescending = false;
        }

        ApplyFilter();
    }

    public void SetPayees(IReadOnlyList<Payee> payees)
    {
        _all = payees ?? Array.Empty<Payee>();
        ApplyFilter();
    }

    public void FocusSearch()
    {
        _txtSearch.Focus();
        _txtSearch.SelectAll();
    }

    private void ApplyFilter()
    {
        var q = (_txtSearch.Text ?? string.Empty).Trim();

        IEnumerable<Payee> filtered = _all;

        if (!string.IsNullOrWhiteSpace(q))
        {
            filtered = filtered.Where(p =>
                p.PayeeName.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        IEnumerable<Payee> sorted = _sortColumn switch
        {
            nameof(Row.PayeeName) => _sortDescending
                ? filtered.OrderByDescending(p => p.PayeeName)
                          .ThenBy(p => p.SortIndex)
                : filtered.OrderBy(p => p.PayeeName)
                          .ThenBy(p => p.SortIndex),

            nameof(Row.SortIndex) => _sortDescending
                ? filtered.OrderByDescending(p => p.SortIndex)
                          .ThenBy(p => p.PayeeName)
                : filtered.OrderBy(p => p.SortIndex)
                          .ThenBy(p => p.PayeeName),

            nameof(Row.IsActive) => _sortDescending
                ? filtered.OrderByDescending(p => p.IsActive)
                          .ThenBy(p => p.PayeeName)
                : filtered.OrderBy(p => p.IsActive)
                          .ThenBy(p => p.PayeeName),

            nameof(Row.IncludeInBudgetTemplate) => _sortDescending
                ? filtered.OrderByDescending(p => p.IncludeInBudgetTemplate)
                          .ThenBy(p => p.PayeeName)
                : filtered.OrderBy(p => p.IncludeInBudgetTemplate)
                          .ThenBy(p => p.PayeeName),

            _ => filtered.OrderBy(p => p.PayeeName)
                         .ThenBy(p => p.SortIndex)
        };

        var rows = sorted
            .Select(p => new Row
            {
                PayeeId = p.PayeeId,
                PayeeName = p.PayeeName,
                IncludeInBudgetTemplate = p.IncludeInBudgetTemplate,
                SortIndex = p.SortIndex,
                IsActive = p.IsActive
            })
            .ToList();

        _grid.DataSource = rows;

        if (_grid.Columns.Contains(nameof(Row.PayeeId)))
            _grid.Columns[nameof(Row.PayeeId)].Visible = false;

        if (_grid.Columns.Contains(nameof(Row.PayeeName)))
            _grid.Columns[nameof(Row.PayeeName)].HeaderText = "Payee";

        if (_grid.Columns.Contains(nameof(Row.IncludeInBudgetTemplate)))
        {
            _grid.Columns[nameof(Row.IncludeInBudgetTemplate)].HeaderText = "In Template";
            _grid.Columns[nameof(Row.IncludeInBudgetTemplate)].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _grid.Columns[nameof(Row.IncludeInBudgetTemplate)].SortMode = DataGridViewColumnSortMode.Automatic;
        }

        if (_grid.Columns.Contains(nameof(Row.SortIndex)))
        {
            _grid.Columns[nameof(Row.SortIndex)].HeaderText = "Sort";
            _grid.Columns[nameof(Row.SortIndex)].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _grid.Columns[nameof(Row.SortIndex)].SortMode = DataGridViewColumnSortMode.Automatic;
        }

        if (_grid.Columns.Contains(nameof(Row.IsActive)))
        {
            _grid.Columns[nameof(Row.IsActive)].HeaderText = "Active";
            _grid.Columns[nameof(Row.IsActive)].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _grid.Columns[nameof(Row.IsActive)].SortMode = DataGridViewColumnSortMode.Automatic;

        }

        foreach (DataGridViewColumn col in _grid.Columns)
            col.HeaderCell.SortGlyphDirection = SortOrder.None;

        if (_grid.Columns.Contains(_sortColumn))
        {
            _grid.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                _sortDescending ? SortOrder.Descending : SortOrder.Ascending;
        }
    }

    private void OpenSelected()
    {
        if (_grid.CurrentRow?.DataBoundItem is not Row r)
            return;

        if (string.IsNullOrWhiteSpace(r.PayeeId))
            return;

        _openPayeeDetails(r.PayeeId);
    }

    private sealed class Row
    {
        public string PayeeId { get; set; } = string.Empty;

        public string PayeeName { get; set; } = string.Empty;

        public bool IncludeInBudgetTemplate { get; set; }

        public int SortIndex { get; set; }

        public bool IsActive { get; set; }
    }
}