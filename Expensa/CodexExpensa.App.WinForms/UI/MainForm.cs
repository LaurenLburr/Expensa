using CodexExpensa.Data.Sqlite.Db;
using CodexExpensa.Data.Sqlite.Db.Schema;

namespace CodexExpensa.App.WinForms.UI;

public sealed partial class MainForm : Form
{
    private readonly SqliteDatabase _db;
    private readonly MigrationRunner _migrationRunner;
    private readonly string _dbPath;

    private readonly MenuStrip _menu;
    private readonly ToolStripMenuItem _menuTools;
    private readonly ToolStripMenuItem _menuDbStatus;

    private readonly StatusStrip _status;
    private readonly ToolStripStatusLabel _statusLabel;

    public MainForm(SqliteDatabase db, MigrationRunner migrationRunner, string dbPath)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _migrationRunner = migrationRunner ?? throw new ArgumentNullException(nameof(migrationRunner));
        _dbPath = string.IsNullOrWhiteSpace(dbPath) ? throw new ArgumentException("dbPath is required.", nameof(dbPath)) : dbPath;

        Text = "Codex Expensa";
        Width = 1200;
        Height = 800;
        StartPosition = FormStartPosition.CenterScreen;

        _menu = new MenuStrip();
        _menuTools = new ToolStripMenuItem("&Tools");
        _menuDbStatus = new ToolStripMenuItem("&DB Status", null, OnDbStatusClicked);

        _menuTools.DropDownItems.Add(_menuDbStatus);
        _menu.Items.Add(_menuTools);

        _status = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel();
        _status.Items.Add(_statusLabel);

        MainMenuStrip = _menu;
        Controls.Add(_menu);
        Controls.Add(_status);

        var label = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "Codex Expensa\n\nTools → DB Status shows migration health.\n\nNext: Banks + Accounts CRUD.",
            Font = new Font(Font.FontFamily, 14f, FontStyle.Regular)
        };

        Controls.Add(label);

        Load += (_, _) =>
        {
            _statusLabel.Text = $"db: {_dbPath}";
        };
    }

    private void OnDbStatusClicked(object? sender, EventArgs e)
    {
        using var dlg = new DbStatusForm(_db, _migrationRunner, _dbPath);
        dlg.ShowDialog(this);
    }
}