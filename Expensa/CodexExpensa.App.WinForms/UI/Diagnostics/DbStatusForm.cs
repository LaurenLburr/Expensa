using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using CodexExpensa.Core.Abstractions;

namespace CodexExpensa.App.WinForms.UI;

public sealed partial class DbStatusForm : Form
{
    private readonly IDatabaseSession _dbSession;
    private readonly IMigrationStatusProvider _migrationStatus;
    private readonly string _dbPath;

    private readonly TextBox _txtDbPath;
    private readonly Button _btnOpenFolder;
    private readonly Button _btnRefresh;
    private readonly ListView _list;

    public DbStatusForm(
        IDatabaseSession dbSession,
        IMigrationStatusProvider migrationStatus,
        string dbPath)
    {
        _dbSession = dbSession ?? throw new ArgumentNullException(nameof(dbSession));
        _migrationStatus = migrationStatus ?? throw new ArgumentNullException(nameof(migrationStatus));
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

        _list.Columns.Add("MigrationId", 520);
        _list.Columns.Add("Status", 160);

        Controls.Add(_list);
        Controls.Add(panelTop);

        Shown += (_, _) => LoadStatus();
    }

    private void LoadStatus()
    {
        _list.BeginUpdate();
        _list.Items.Clear();

        IReadOnlyList<MigrationStatusRow> rows;

        try
        {
            rows = _migrationStatus.GetStatus(_dbSession);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.ToString(),
                "Failed to load migration status",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            _list.EndUpdate();
            return;
        }

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
                    this,
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
                this,
                $"Could not open folder:\n\n{ex.Message}",
                "Codex Expensa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}