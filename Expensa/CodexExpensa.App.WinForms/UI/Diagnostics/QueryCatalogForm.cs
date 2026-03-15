using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using CodexExpensa.Core.Abstractions;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.App.WinForms.UI.Diagnostics;

public sealed class QueryCatalogForm : Form
{
    private readonly IDatabaseSession _db;

    private readonly SplitContainer _outerSplit;
    private readonly SplitContainer _rightSplit;

    private readonly DataGridView _gridCatalog;
    private readonly TextBox _txtSql;
    private readonly DataGridView _gridResults;

    private readonly ToolStrip _tool;
    private readonly ToolStripButton _btnRefresh;
    private readonly ToolStripButton _btnRun;
    private readonly ToolStripButton _btnCopySql;
    private readonly ToolStripButton _btnSaveCatalog;
    private readonly ToolStripLabel _lblSearch;
    private readonly ToolStripTextBox _txtSearch;
    private readonly ToolStripLabel _lblCategory;
    private readonly ToolStripComboBox _cmbCategory;
    private readonly ToolStripLabel _lblStatus;

    private DataTable? _catalogTable;
    private readonly Dictionary<string, Dictionary<string, string>> _parameterMemory
        = new(StringComparer.OrdinalIgnoreCase);

    private const string CatalogDumpPath =
        @"D:\Git\CodexExpensa\Expensa\ChatGpt\sqlquery_catalog_dump.sql";

    public QueryCatalogForm(IDatabaseSession db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));

        Text = "Query Catalog";
        Width = 1400;
        Height = 850;
        StartPosition = FormStartPosition.CenterParent;

        _tool = new ToolStrip();

        _btnRefresh = new ToolStripButton("Refresh");
        _btnRefresh.Click += (_, _) => LoadCatalog();

        _btnRun = new ToolStripButton("Run");
        _btnRun.Click += (_, _) => RunSelectedQuery();

        _btnCopySql = new ToolStripButton("Copy SQL");
        _btnCopySql.Click += (_, _) => CopySelectedSql();

        _btnSaveCatalog = new ToolStripButton("Save Catalog");
        _btnSaveCatalog.Click += (_, _) => SaveCatalogToFile();

        _lblSearch = new ToolStripLabel("Search:");
        _txtSearch = new ToolStripTextBox
        {
            AutoSize = false,
            Width = 220
        };
        _txtSearch.TextChanged += (_, _) => ApplyCatalogFilter();

        _lblCategory = new ToolStripLabel("Category:");
        _cmbCategory = new ToolStripComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            AutoSize = false,
            Width = 140
        };
        _cmbCategory.SelectedIndexChanged += (_, _) => ApplyCatalogFilter();

        _lblStatus = new ToolStripLabel("Ready");

        _tool.Items.Add(_btnRefresh);
        _tool.Items.Add(_btnRun);
        _tool.Items.Add(_btnCopySql);
        _tool.Items.Add(_btnSaveCatalog);
        _tool.Items.Add(new ToolStripSeparator());
        _tool.Items.Add(_lblSearch);
        _tool.Items.Add(_txtSearch);
        _tool.Items.Add(new ToolStripSeparator());
        _tool.Items.Add(_lblCategory);
        _tool.Items.Add(_cmbCategory);
        _tool.Items.Add(new ToolStripSeparator());
        _tool.Items.Add(_lblStatus);

        _outerSplit = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 560
        };

        _rightSplit = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterDistance = 260
        };

        _gridCatalog = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            MultiSelect = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoGenerateColumns = true
        };

        _gridCatalog.SelectionChanged += GridCatalog_SelectionChanged;
        _gridCatalog.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0)
                RunSelectedQuery();
        };

        _txtSql = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = false,
            Font = new Font("Consolas", 10F)
        };

        _gridResults = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoGenerateColumns = true
        };

        _outerSplit.Panel1.Controls.Add(_gridCatalog);

        _rightSplit.Panel1.Controls.Add(_txtSql);
        _rightSplit.Panel2.Controls.Add(_gridResults);

        _outerSplit.Panel2.Controls.Add(_rightSplit);

        Controls.Add(_outerSplit);
        Controls.Add(_tool);

        _tool.Dock = DockStyle.Top;

        Load += (_, _) => LoadCatalog();
    }

    private void LoadCatalog()
    {
        try
        {
            _catalogTable = _db.QueryDataTable("SqlQuery.SelectCatalog");

            PopulateCategoryFilter();
            ApplyCatalogFilter();

            _lblStatus.Text = $"Loaded {_catalogTable.Rows.Count} queries";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Query Catalog");
            _lblStatus.Text = "Load failed";
        }
    }

    private void PopulateCategoryFilter()
    {
        if (_catalogTable is null)
            return;

        string? selected = _cmbCategory.SelectedItem?.ToString();

        var categories = _catalogTable.AsEnumerable()
            .Select(r => r["Category"] == DBNull.Value ? string.Empty : r["Category"].ToString() ?? string.Empty)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        _cmbCategory.SelectedIndexChanged -= CmbCategory_SelectedIndexChangedProxy;

        _cmbCategory.Items.Clear();
        _cmbCategory.Items.Add("(All)");

        foreach (string category in categories)
            _cmbCategory.Items.Add(category);

        if (!string.IsNullOrWhiteSpace(selected) && _cmbCategory.Items.Contains(selected))
            _cmbCategory.SelectedItem = selected;
        else
            _cmbCategory.SelectedIndex = 0;

        _cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChangedProxy;
    }

    private void CmbCategory_SelectedIndexChangedProxy(object? sender, EventArgs e)
    {
        ApplyCatalogFilter();
    }

    private void ApplyCatalogFilter()
    {
        if (_catalogTable is null)
            return;

        string search = _txtSearch.Text?.Trim() ?? string.Empty;
        string selectedCategory = _cmbCategory.SelectedItem?.ToString() ?? "(All)";

        IEnumerable<DataRow> query = _catalogTable.AsEnumerable();

        if (!string.Equals(selectedCategory, "(All)", StringComparison.Ordinal))
        {
            query = query.Where(r =>
                string.Equals(
                    r["Category"] == DBNull.Value ? string.Empty : r["Category"].ToString(),
                    selectedCategory,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(r =>
            {
                string queryName = r["QueryName"] == DBNull.Value ? string.Empty : r["QueryName"].ToString() ?? string.Empty;
                string category = r["Category"] == DBNull.Value ? string.Empty : r["Category"].ToString() ?? string.Empty;
                string description = r["Description"] == DBNull.Value ? string.Empty : r["Description"].ToString() ?? string.Empty;
                string sql = r["SqlText"] == DBNull.Value ? string.Empty : r["SqlText"].ToString() ?? string.Empty;

                return queryName.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || category.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || description.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || sql.Contains(search, StringComparison.OrdinalIgnoreCase);
            });
        }

        DataTable filteredTable = _catalogTable.Clone();

        foreach (DataRow row in query)
            filteredTable.ImportRow(row);

        _gridCatalog.DataSource = filteredTable;

        ConfigureCatalogGrid();

        if (filteredTable.Rows.Count > 0)
        {
            _gridCatalog.ClearSelection();
            _gridCatalog.Rows[0].Selected = true;
            _gridCatalog.CurrentCell = _gridCatalog.Rows[0].Cells[0];
            UpdateSqlPreviewFromSelection();
        }
        else
        {
            _txtSql.Text = string.Empty;
            _gridResults.DataSource = null;
        }

        _lblStatus.Text = $"Showing {filteredTable.Rows.Count} of {_catalogTable.Rows.Count} queries";
    }

    private void ConfigureCatalogGrid()
    {
        if (_gridCatalog.Columns.Contains("SqlText"))
            _gridCatalog.Columns["SqlText"]!.Visible = false;

        if (_gridCatalog.Columns.Contains("Category"))
            _gridCatalog.Columns["Category"]!.Width = 120;

        if (_gridCatalog.Columns.Contains("QueryName"))
            _gridCatalog.Columns["QueryName"]!.Width = 260;

        if (_gridCatalog.Columns.Contains("Description"))
            _gridCatalog.Columns["Description"]!.Width = 320;
    }

    private void GridCatalog_SelectionChanged(object? sender, EventArgs e)
    {
        UpdateSqlPreviewFromSelection();
    }

    private void UpdateSqlPreviewFromSelection()
    {
        if (_gridCatalog.CurrentRow?.DataBoundItem is not DataRowView rowView)
        {
            _txtSql.Text = "";
            return;
        }

        _txtSql.Text = rowView["SqlText"]?.ToString() ?? "";
    }

    private void CopySelectedSql()
    {
        this.Cursor = Cursors.WaitCursor;

        if (_gridCatalog.CurrentRow?.DataBoundItem is not DataRowView rowView)
            return;

        Clipboard.SetText(rowView["SqlText"]?.ToString() ?? "");
        _lblStatus.Text = "SQL copied";

        this.Cursor = Cursors.Default;
    }

    private void RunSelectedQuery()
    {
        if (_gridCatalog.CurrentRow?.DataBoundItem is not DataRowView rowView)
            return;

        string queryName = rowView["QueryName"]?.ToString() ?? "";
        string sql = rowView["SqlText"]?.ToString() ?? "";

        List<string> parameterNames = GetParameterNames(sql);
        DbParameter[] parameters = Array.Empty<DbParameter>();

        if (parameterNames.Count > 0)
        {
            var values = PromptForParameters(parameterNames, queryName);
            if (values == null)
            {
                _lblStatus.Text = "Run cancelled";
                return;
            }

            parameters = values
                .Select(x => (DbParameter)new SqliteParameter(x.Key, ParseParameterValue(x.Value)))
                .ToArray();
        }

        Stopwatch sw = Stopwatch.StartNew();

        if (LooksLikeSelect(sql))
        {
            DataTable results = parameterNames.Count == 0
                ? _db.QueryDataTable(queryName)
                : _db.QueryDataTable(queryName, parameters);

            sw.Stop();

            _gridResults.DataSource = results;
            _lblStatus.Text = $"{results.Rows.Count} rows returned in {sw.ElapsedMilliseconds} ms";
        }
        else
        {
            if (parameterNames.Count == 0)
                _db.Execute(queryName);
            else
                _db.Execute(queryName, parameters);

            sw.Stop();

            _gridResults.DataSource = null;
            _lblStatus.Text = $"Query executed in {sw.ElapsedMilliseconds} ms";
        }
    }

    private void SaveCatalogToFile()
    {
        DataTable table = _db.QueryDataTable("SqlQuery.SelectCatalog");

        var sb = new StringBuilder();

        sb.AppendLine("-- SqlQuery Catalog Dump");
        sb.AppendLine("-- Generated " + DateTime.UtcNow.ToString("u"));
        sb.AppendLine();

        foreach (DataRow r in table.Rows)
        {
            string queryName = EscapeSql(r["QueryName"]);
            string category = EscapeSql(r["Category"]);
            string description = EscapeSql(r["Description"]);
            string sqlText = EscapeSql(r["SqlText"]);

            bool isActive = true;
            if (table.Columns.Contains("IsActive") && r["IsActive"] != DBNull.Value)
                isActive = Convert.ToBoolean(r["IsActive"]);

            sb.AppendLine("INSERT INTO [SqlQuery]");
            sb.AppendLine("(");
            sb.AppendLine("    [QueryName],");
            sb.AppendLine("    [Category],");
            sb.AppendLine("    [Description],");
            sb.AppendLine("    [SqlText],");
            sb.AppendLine("    [IsActive]");
            sb.AppendLine(")");
            sb.AppendLine("VALUES");
            sb.AppendLine("(");
            sb.AppendLine($"    '{queryName}',");
            sb.AppendLine($"    '{category}',");
            sb.AppendLine($"    '{description}',");
            sb.AppendLine($"    '{sqlText}',");
            sb.AppendLine($"    {(isActive ? 1 : 0)}");
            sb.AppendLine(")");
            sb.AppendLine("ON CONFLICT([QueryName]) DO UPDATE SET");
            sb.AppendLine("    [Category] = excluded.[Category],");
            sb.AppendLine("    [Description] = excluded.[Description],");
            sb.AppendLine("    [SqlText] = excluded.[SqlText],");
            sb.AppendLine("    [IsActive] = excluded.[IsActive];");
            sb.AppendLine();
        }

        File.WriteAllText(CatalogDumpPath, sb.ToString());

        _lblStatus.Text = "Catalog saved";
    }
    private Dictionary<string, string>? PromptForParameters(
        IReadOnlyList<string> parameterNames,
        string queryName)
    {
        using Form form = new()
        {
            Text = $"Parameters - {queryName}",
            Width = 500,
            Height = Math.Max(200, 120 + parameterNames.Count * 34),
            StartPosition = FormStartPosition.CenterParent
        };

        Panel panel = new() { Dock = DockStyle.Fill };

        var boxes = new Dictionary<string, TextBox>(StringComparer.Ordinal);

        _parameterMemory.TryGetValue(queryName, out var previous);

        int top = 10;

        foreach (var name in parameterNames)
        {
            Label label = new()
            {
                Text = name,
                Left = 10,
                Top = top + 4,
                Width = 140
            };

            TextBox box = new()
            {
                Left = 160,
                Top = top,
                Width = 300
            };

            if (previous != null && previous.TryGetValue(name, out var val))
                box.Text = val;

            panel.Controls.Add(label);
            panel.Controls.Add(box);

            boxes[name] = box;

            top += 34;
        }

        Button ok = new()
        {
            Text = "OK",
            Left = 300,
            Width = 70,
            Top = top + 10,
            DialogResult = DialogResult.OK
        };

        Button cancel = new()
        {
            Text = "Cancel",
            Left = 380,
            Width = 70,
            Top = top + 10,
            DialogResult = DialogResult.Cancel
        };

        panel.Controls.Add(ok);
        panel.Controls.Add(cancel);

        form.Controls.Add(panel);
        form.AcceptButton = ok;
        form.CancelButton = cancel;

        if (form.ShowDialog(this) != DialogResult.OK)
            return null;

        var results = boxes.ToDictionary(x => x.Key, x => x.Value.Text ?? "", StringComparer.Ordinal);

        _parameterMemory[queryName] = new Dictionary<string, string>(results, StringComparer.Ordinal);

        return results;
    }

    private static List<string> GetParameterNames(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return new();

        var matches = Regex.Matches(sql, @"(?<!\w)([@$][A-Za-z_][A-Za-z0-9_]*)");

        return matches
            .Select(m => m.Value)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();
    }

    private static object ParseParameterValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DBNull.Value;

        if (int.TryParse(value, out int i))
            return i;

        if (decimal.TryParse(value, out decimal d))
            return d;

        if (bool.TryParse(value, out bool b))
            return b;

        return value;
    }

    private static string EscapeSql(object value)
    {
        if (value == DBNull.Value)
            return "";

        return value.ToString()!
            .Replace("'", "''")
            .Replace("\r", "")
            .Replace("\n", "\\n");
    }

    private static bool LooksLikeSelect(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return false;

        string trimmed = sql.TrimStart();

        return trimmed.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("WITH", StringComparison.OrdinalIgnoreCase);
    }
}