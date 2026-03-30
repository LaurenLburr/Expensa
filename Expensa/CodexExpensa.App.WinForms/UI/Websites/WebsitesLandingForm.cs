using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Websites;

public sealed class WebsitesLandingForm : Form
{
    private readonly Action<string> _openDetails;

    private readonly List<Row> _allRows = new();

    private readonly ToolStrip _tool;
    private readonly ToolStripLabel _lblSearch;
    private readonly ToolStripTextBox _txtSearch;
    private readonly DataGridView _grid;

    private sealed class Row
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
    }

    public WebsitesLandingForm(Action<string> openDetails)
    {
        _openDetails = openDetails ?? throw new ArgumentNullException(nameof(openDetails));

        Text = "Websites";
        FormBorderStyle = FormBorderStyle.None;
        Dock = DockStyle.Fill;

        _tool = new ToolStrip
        {
            Dock = DockStyle.Top
        };

        _lblSearch = new ToolStripLabel("Filter");
        _txtSearch = new ToolStripTextBox
        {
            AutoSize = false,
            Width = 240
        };
        _txtSearch.TextChanged += (_, _) => ApplyFilter();

        _tool.Items.Add(_lblSearch);
        _tool.Items.Add(_txtSearch);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Row.Name),
            HeaderText = "Name",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Row.Url),
            HeaderText = "URL",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Row.Tags),
            HeaderText = "Tags",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        _grid.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (_grid.Rows[e.RowIndex].DataBoundItem is Row row)
            {
                _openDetails(row.Id);
            }
        };

        Controls.Add(_grid);
        Controls.Add(_tool);
    }

    public void SetRows(IEnumerable<(string Id, string Name, string Url, string Tags)> rows)
    {
        _allRows.Clear();

        if (rows is not null)
        {
            _allRows.AddRange(rows.Select(r => new Row
            {
                Id = r.Id,
                Name = r.Name,
                Url = r.Url,
                Tags = r.Tags
            }));
        }

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        string filter = _txtSearch.Text.Trim();

        IEnumerable<Row> rows = _allRows;

        if (!string.IsNullOrWhiteSpace(filter))
        {
            rows = rows.Where(row =>
                row.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                row.Url.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                row.Tags.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        _grid.DataSource = null;
        _grid.DataSource = rows.ToList();
    }
}
