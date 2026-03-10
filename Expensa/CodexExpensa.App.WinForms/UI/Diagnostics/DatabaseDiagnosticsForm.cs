using System.Data;
using CodexExpensa.Core.Abstractions;

namespace CodexExpensa.App.WinForms.UI.Diagnostics;

public sealed class DatabaseDiagnosticsForm : Form
{
    private readonly IDatabaseSession _db;
    private readonly DataGridView _grid;
    private readonly Button _btnLoad;

    public DatabaseDiagnosticsForm(IDatabaseSession db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));

        Text = "Database Diagnostics";
        Width = 800;
        Height = 500;

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        _btnLoad = new Button
        {
            Text = "Load Payees",
            Dock = DockStyle.Top,
            Height = 40
        };

        _btnLoad.Click += BtnLoad_Click;

        Controls.Add(_grid);
        Controls.Add(_btnLoad);
    }

    private void BtnLoad_Click(object? sender, EventArgs e)
    {
        try
        {
            DataTable table = _db.QueryDataTable("Payee.SelectAll");
            _grid.DataSource = table;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Database Error");
        }
    }
}