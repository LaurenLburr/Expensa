using System.Diagnostics;
using CodexExpensa.Data.Sqlite.Db;
using CodexExpensa.Data.Sqlite.Db.Schema;

namespace CodexExpensa.App.WinForms.UI;

public sealed partial   class DbStatusForm : Form
{
    private readonly SqliteDatabase _db;
    private readonly MigrationRunner _migrationRunner;
    private readonly string _dbPath;

    private readonly TextBox _txtDbPath;
    private readonly Button _btnOpenFolder;
    private readonly Button _btnRefresh;
    private readonly ListView _list;

    public DbStatusForm(SqliteDatabase db, MigrationRunner migrationRunner, string dbPath)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _migrationRunner = migrationRunner ?? throw new ArgumentNullException(nameof(migrationRunner));
        _dbPath = string.IsNullOrWhiteSpace(dbPath)
            ? throw new ArgumentException("dbPath is required.", nameof(dbPath))
            : dbPath;

        Text = "DB Status";
        Width = 950;
        Height = 650;
        StartPosition = FormStartPosition.CenterParent;

        var panelTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 95,
            Padding = new Padding(10)
        };

        var lblPath = new Label
        {
            Text = "DB Path:",
            AutoSize = true,
            Left = 10,
            Top = 12
        };

        _txtDbPath = new TextBox
        {
            Left = 80,
            Top = 8,
            Width = 820,
            ReadOnly = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        _btnOpenFolder = new Button
        {
            Text = "Open Folder",
            Left = 80,
            Top = 38,
            Width = 120
        };
        _btnOpenFolder.Click += (_, _) => OpenDbFolder();

        _btnRefresh = new Button
        {
            Text = "Refresh",
            Left = 210,
            Top = 38,
            Width = 100
        };
        _btnRefresh.Click += (_, _) => LoadStatus();

        _txtDbPath.Text = _dbPath;

        panelTop.Controls.Add(lblPath);
        panelTop.Controls.Add(_txtDbPath);
        panelTop.Controls.Add(_btnOpenFolder);
        panelTop.Controls.Add(_btnRefresh);

        _list = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true
        };

        _list.Columns.Add("MigrationId", 420);
        _list.Columns.Add("Status", 160);

        Controls.Add(_list);
        Controls.Add(panelTop);

        Shown += (_, _) => LoadStatus();
    }

    private void LoadStatus()
    {
        _list.BeginUpdate();
        _list.Items.Clear();

        var rows = _migrationRunner.GetStatus(_db);

        foreach (var row in rows)
        {
            var item = new ListViewItem(row.MigrationId);
            item.SubItems.Add(row.Status);
            _list.Items.Add(item);
        }

        _list.EndUpdate();
    }

    private void OpenDbFolder()
    {
        try
        {
            var folder = Path.GetDirectoryName(_dbPath);
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                MessageBox.Show(
                    "DB folder not found.",
                    "Codex Expensa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = folder,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not open folder:\n\n{ex.Message}",
                "Codex Expensa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}