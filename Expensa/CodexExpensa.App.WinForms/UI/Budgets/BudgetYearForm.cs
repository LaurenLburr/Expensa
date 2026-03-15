using System.Data;
using System.Data.Common;
using CodexExpensa.Core.Abstractions;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.App.WinForms.UI.Budgets;

public sealed class BudgetYearForm : Form
{
    private readonly IDatabaseSession _db;
    private readonly int _year;

    private readonly Panel _rightPanel;
    private readonly Panel _gridHost;
    private readonly DataGridView _grid;
    private readonly LinkLabel _lnkCreateBudget;

    public BudgetYearForm(IDatabaseSession db, int year)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _year = year;

        Text = $"Budget Year {year}";
        Width = 1000;
        Height = 650;

        _rightPanel = new Panel
        {
            Dock = DockStyle.Right,
            Width = 240,
            BackColor = SystemColors.Control
        };

        _gridHost = new Panel
        {
            Dock = DockStyle.Fill
        };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };

       
        _lnkCreateBudget = new LinkLabel
        {
            Text = "Create Budget",
            Dock = DockStyle.Top,
            Height = 28,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(8, 4, 0, 4),
            LinkBehavior = LinkBehavior.HoverUnderline
        };
        _lnkCreateBudget.LinkClicked += LnkCreateBudget_LinkClicked;
        _lnkCreateBudget.Enabled = false;

        _grid.SelectionChanged += (_, _) =>
        {
            _lnkCreateBudget.Enabled = _grid.CurrentRow != null;
        };
        _grid.CellDoubleClick += Grid_CellDoubleClick;

        _gridHost.Controls.Add(_grid);
        _rightPanel.Controls.Add(_lnkCreateBudget);

        Controls.Add(_gridHost);
        Controls.Add(_rightPanel);

        Load += BudgetYearForm_Load;
    }

    private void BudgetYearForm_Load(object? sender, EventArgs e)
    {
        LoadMissingMonths();
    }

    private void LoadMissingMonths()
    {
        DataTable table = _db.QueryDataTable(
            "BudgetMonth.MissingMonthsForYear",
            new DbParameter[]
            {
                new SqliteParameter("@Year", _year)
            });

        _grid.DataSource = table;
    }

    private void CreateBudget(int month)
    {
        _db.Execute(
            "BudgetMonth.Create",
            new DbParameter[]
            {
            new SqliteParameter("@BudgetMonthId", $"{_year:D4}-{month:D2}"),
            new SqliteParameter("@Year", _year),
            new SqliteParameter("@Month", month)
            });

        LoadMissingMonths();
    }

    private void Grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        if (_grid.Rows[e.RowIndex].DataBoundItem is not DataRowView rowView)
            return;

        int month = Convert.ToInt32(rowView["MonthNumber"]);

        CreateBudget(month);
    }

    private void LnkCreateBudget_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not DataRowView rowView)
            return;

        int month = Convert.ToInt32(rowView["Month"]);

        CreateBudget(month);
    }

}