using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.Core.Domain.Banks;

namespace CodexExpensa.App.WinForms.UI.Banks;

public sealed class BanksLandingForm : Form
{
    private readonly Action<string> _openBankDetails;
    private IReadOnlyList<Bank> _banks = Array.Empty<Bank>();

    private readonly ToolStrip _tool;
    private readonly ToolStripLabel _lbl;
    private readonly DataGridView _grid;

    public BanksLandingForm(Action<string> openBankDetails)
    {
        _openBankDetails = openBankDetails ?? throw new ArgumentNullException(nameof(openBankDetails));

        Text = "Banks";
        FormBorderStyle = FormBorderStyle.None;

        _tool = new ToolStrip { Dock = DockStyle.Top };
        _lbl = new ToolStripLabel("Double-click a bank to edit.");
        _tool.Items.Add(_lbl);

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

    public void SetBanks(IReadOnlyList<Bank> banks)
    {
        _banks = banks ?? Array.Empty<Bank>();

        var rows = _banks
            .OrderBy(b => b.BankName)
            .ThenBy(b => b.RoutingNumber)
            .Select(b => new Row
            {
                BankId = b.BankId,
                BankName = b.BankName,
                RoutingNumber = b.RoutingNumber,
                Url = b.Url ?? string.Empty,
                IsActive = b.IsActive
            })
            .ToList();

        _grid.DataSource = rows;

        if (_grid.Columns.Contains(nameof(Row.BankId)))
            _grid.Columns[nameof(Row.BankId)].Visible = false;

        if (_grid.Columns.Contains(nameof(Row.RoutingNumber)))
            _grid.Columns[nameof(Row.RoutingNumber)].HeaderText = "Routing #";

        if (_grid.Columns.Contains(nameof(Row.IsActive)))
            _grid.Columns[nameof(Row.IsActive)].HeaderText = "Active";
    }

    public void SelectBank(string bankId)
    {
        if (string.IsNullOrWhiteSpace(bankId))
            return;

        foreach (DataGridViewRow row in _grid.Rows)
        {
            if (row.DataBoundItem is Row r && r.BankId == bankId)
            {
                _grid.ClearSelection();
                row.Selected = true;
                _grid.FirstDisplayedScrollingRowIndex = Math.Max(0, row.Index);
                break;
            }
        }
    }

    private void OpenSelected()
    {
        if (_grid.CurrentRow?.DataBoundItem is not Row r)
            return;

        if (string.IsNullOrWhiteSpace(r.BankId))
            return;

        _openBankDetails(r.BankId);
    }

    private sealed class Row
    {
        public string BankId { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string RoutingNumber { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}