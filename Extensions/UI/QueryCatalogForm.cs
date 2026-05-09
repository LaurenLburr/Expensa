using Microsoft.Data.Sqlite;

namespace CodexExpensa.ExtensionDevHost.UI;

public sealed class QueryCatalogForm : Form
{
    private readonly string _databasePath;

    private readonly TableLayoutPanel _layout = new();
    private readonly TextBox _databasePathTextBox = new();
    private readonly DataGridView _grid = new();
    private readonly BindingSource _bindingSource = new();
    private readonly FlowLayoutPanel _buttonPanel = new();
    private readonly Button _refreshButton = new();
    private readonly Button _ensureTableButton = new();
    private readonly Label _statusLabel = new();

    public QueryCatalogForm(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        _databasePath = databasePath;

        Text = "Query Catalog";
        Width = 1100;
        Height = 700;
        StartPosition = FormStartPosition.CenterParent;

        InitializeUi();
        LoadRows();
    }

    private void InitializeUi()
    {
        _layout.Dock = DockStyle.Fill;
        _layout.Padding = new Padding(10);
        _layout.ColumnCount = 1;
        _layout.RowCount = 4;
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));

        _databasePathTextBox.Dock = DockStyle.Fill;
        _databasePathTextBox.ReadOnly = true;
        _databasePathTextBox.Text = _databasePath;

        _grid.Dock = DockStyle.Fill;
        _grid.AutoGenerateColumns = false;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.ReadOnly = true;

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Query Name",
            DataPropertyName = nameof(QueryCatalogRow.QueryName),
            Width = 240
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Category",
            DataPropertyName = nameof(QueryCatalogRow.Category),
            Width = 160
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Active",
            DataPropertyName = nameof(QueryCatalogRow.IsActive),
            Width = 70
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Description",
            DataPropertyName = nameof(QueryCatalogRow.Description),
            Width = 320
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "SQL",
            DataPropertyName = nameof(QueryCatalogRow.SqlText),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        _grid.DataSource = _bindingSource;

        _buttonPanel.Dock = DockStyle.Fill;
        _buttonPanel.FlowDirection = FlowDirection.RightToLeft;

        _refreshButton.Text = "Refresh";
        _refreshButton.AutoSize = true;
        _refreshButton.Click += (_, _) => LoadRows();

        _ensureTableButton.Text = "Ensure SqlQuery Table";
        _ensureTableButton.AutoSize = true;
        _ensureTableButton.Click += (_, _) =>
        {
            EnsureSqlQueryTable();
            LoadRows();
        };

        _buttonPanel.Controls.Add(_refreshButton);
        _buttonPanel.Controls.Add(_ensureTableButton);

        _statusLabel.Dock = DockStyle.Fill;
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;

        _layout.Controls.Add(_databasePathTextBox, 0, 0);
        _layout.Controls.Add(_grid, 0, 1);
        _layout.Controls.Add(_buttonPanel, 0, 2);
        _layout.Controls.Add(_statusLabel, 0, 3);

        Controls.Add(_layout);
    }

    private void LoadRows()
    {
        try
        {
            if (!File.Exists(_databasePath))
            {
                _bindingSource.DataSource = new List<QueryCatalogRow>();
                _statusLabel.Text = "Database does not exist yet.";
                return;
            }

            EnsureSqlQueryTable();

            using SqliteConnection connection = new($"Data Source={_databasePath}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                "SELECT [QueryName], [Description], [SqlText], [Category], [IsActive] " +
                "FROM [SqlQuery] " +
                "ORDER BY [Category], [QueryName];";

            using SqliteDataReader reader = command.ExecuteReader();

            List<QueryCatalogRow> rows = new();

            while (reader.Read())
            {
                rows.Add(new QueryCatalogRow
                {
                    QueryName = reader.GetString(0),
                    Description = reader.GetString(1),
                    SqlText = reader.GetString(2),
                    Category = reader.GetString(3),
                    IsActive = reader.GetInt32(4) == 1
                });
            }

            _bindingSource.DataSource = rows;
            _statusLabel.Text = $"Loaded {rows.Count} query catalog row(s).";
        }
        catch (Exception ex)
        {
            _bindingSource.DataSource = new List<QueryCatalogRow>();
            _statusLabel.Text = ex.Message;
            MessageBox.Show(this, ex.Message, "Query Catalog", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void EnsureSqlQueryTable()
    {
        string? folder = Path.GetDirectoryName(_databasePath);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        using SqliteConnection connection = new($"Data Source={_databasePath}");
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "CREATE TABLE IF NOT EXISTS [SqlQuery] (" +
            "[QueryName] TEXT PRIMARY KEY, " +
            "[Description] TEXT NOT NULL DEFAULT (''), " +
            "[SqlText] TEXT NOT NULL DEFAULT (''), " +
            "[CreatedUtc] TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP, " +
            "[Category] TEXT NOT NULL DEFAULT (''), " +
            "[IsActive] INTEGER NOT NULL DEFAULT (1), " +
            "UNIQUE ([QueryName] ASC)" +
            ");";

        command.ExecuteNonQuery();
        _statusLabel.Text = "SqlQuery table is available.";
    }

    private sealed class QueryCatalogRow
    {
        public string QueryName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string SqlText { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
