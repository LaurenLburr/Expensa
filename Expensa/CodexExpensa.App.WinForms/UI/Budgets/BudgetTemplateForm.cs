using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.Core.Domain.Payees;

namespace CodexExpensa.App.WinForms.UI.Budgets;

public sealed class BudgetTemplateForm : Form
{
    private readonly IPayeeRepository _payees;

    private readonly ToolStrip _tool;
    private readonly ToolStripButton _btnRefresh;
    private readonly ToolStripLabel _lblSort;
    private readonly ToolStripComboBox _cmbSort;
    private readonly ToolStripButton _btnDirection;
    private readonly ToolStripLabel _lblHint;

    private readonly DataGridView _grid;

    private bool _loading;
    private bool _sortDescending;

    private enum SortMode
    {
        Name,
        RowIndex
    }

    public BudgetTemplateForm(IPayeeRepository payees)
    {
        _payees = payees ?? throw new ArgumentNullException(nameof(payees));

        Text = "Budget Template";
        FormBorderStyle = FormBorderStyle.None;

        _tool = new ToolStrip { Dock = DockStyle.Top };

        _btnRefresh = new ToolStripButton("Refresh");
        _btnRefresh.Click += (_, _) => LoadRows();

        _lblSort = new ToolStripLabel("Sort:");
        _cmbSort = new ToolStripComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            AutoSize = false,
            Width = 110
        };
        _cmbSort.Items.Add("Name");
        _cmbSort.Items.Add("Row Index");
        _cmbSort.SelectedIndex = 1;
        _cmbSort.SelectedIndexChanged += (_, _) => LoadRows();

        _btnDirection = new ToolStripButton("Asc");
        _btnDirection.Click += (_, _) =>
        {
            _sortDescending = !_sortDescending;
            _btnDirection.Text = _sortDescending ? "Desc" : "Asc";
            LoadRows();
        };

        _lblHint = new ToolStripLabel("Tick payees to include them in the template. Changes save immediately.");

        _tool.Items.Add(_btnRefresh);
        _tool.Items.Add(new ToolStripSeparator());
        _tool.Items.Add(_lblSort);
        _tool.Items.Add(_cmbSort);
        _tool.Items.Add(_btnDirection);
        _tool.Items.Add(new ToolStripSeparator());
        _tool.Items.Add(_lblHint);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };

        _grid.CurrentCellDirtyStateChanged += (_, _) =>
        {
            // Commit checkbox edits immediately so CellValueChanged fires.
            if (_grid.IsCurrentCellDirty)
                _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };

        _grid.CellValueChanged += Grid_CellValueChanged;

        Controls.Add(_grid);
        Controls.Add(_tool);

        Load += (_, _) => LoadRows();
    }

    private void LoadRows()
    {
        _loading = true;

        try
        {
            var all = _payees.GetAll();

            IEnumerable<Payee> sorted = ApplySort(all);

            var rows = sorted
                .Select(p => new Row
                {
                    PayeeId = p.PayeeId,
                    PayeeName = p.PayeeName,
                    Include = p.IncludeInBudgetTemplate,
                    SortIndex = p.SortIndex,
                    IsActive = p.IsActive
                })
                .ToList();

            _grid.DataSource = rows;

            HideColumn(nameof(Row.PayeeId));
            RenameColumn(nameof(Row.PayeeName), "Payee");
            RenameColumn(nameof(Row.Include), "Include");
            RenameColumn(nameof(Row.SortIndex), "Sort");
            RenameColumn(nameof(Row.IsActive), "Active");

            if (_grid.Columns[nameof(Row.Include)] is DataGridViewColumn includeCol)
                includeCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            if (_grid.Columns[nameof(Row.SortIndex)] is DataGridViewColumn sortCol)
                sortCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            if (_grid.Columns[nameof(Row.IsActive)] is DataGridViewColumn activeCol)
                activeCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }
        finally
        {
            _loading = false;
        }
    }

    private IEnumerable<Payee> ApplySort(IEnumerable<Payee> payees)
    {
        var sortMode = GetSortMode();

        return sortMode switch
        {
            SortMode.Name => _sortDescending
                ? payees.OrderByDescending(p => p.PayeeName)
                        .ThenBy(p => p.SortIndex)
                : payees.OrderBy(p => p.PayeeName)
                        .ThenBy(p => p.SortIndex),

            SortMode.RowIndex => _sortDescending
                ? payees.OrderByDescending(p => p.SortIndex)
                        .ThenBy(p => p.PayeeName)
                : payees.OrderBy(p => p.SortIndex)
                        .ThenBy(p => p.PayeeName),

            _ => payees.OrderBy(p => p.SortIndex).ThenBy(p => p.PayeeName)
        };
    }

    private SortMode GetSortMode()
    {
        return _cmbSort.SelectedIndex switch
        {
            0 => SortMode.Name,
            1 => SortMode.RowIndex,
            _ => SortMode.RowIndex
        };
    }

    private void Grid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_loading)
            return;

        if (e.RowIndex < 0 || e.ColumnIndex < 0)
            return;

        if (_grid.Rows[e.RowIndex].DataBoundItem is not Row row)
            return;

        var columnName = _grid.Columns[e.ColumnIndex].DataPropertyName;
        if (!string.Equals(columnName, nameof(Row.Include), StringComparison.Ordinal))
            return;

        try
        {
            _payees.UpdateIncludeInBudgetTemplate(row.PayeeId, row.Include);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Reload to reflect DB truth.
            LoadRows();
        }
    }

    private void HideColumn(string name)
    {
        if (_grid.Columns.Contains(name))
            _grid.Columns[name].Visible = false;
    }

    private void RenameColumn(string name, string header)
    {
        if (_grid.Columns.Contains(name))
            _grid.Columns[name].HeaderText = header;
    }

    private sealed class Row
    {
        public string PayeeId { get; set; } = string.Empty;

        public string PayeeName { get; set; } = string.Empty;

        public bool Include { get; set; }

        public int SortIndex { get; set; }

        public bool IsActive { get; set; }
    }
}