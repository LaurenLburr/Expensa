using CodexExpensa.ExtensionDevHost.Services;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.ExtensionDevHost.UI;

public partial class DatabaseConnectionPanelForm : Form
{
    private readonly DatabaseConnectionSettingsStore _settingsStore = new();

    public DatabaseConnectionPanelForm()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        DatabaseConnectionSettings settings = _settingsStore.Load();

        sandboxCopyRadioButton.Checked = settings.Mode == DatabaseConnectionMode.SandboxCopy;
        productionRadioButton.Checked = settings.Mode == DatabaseConnectionMode.Production;
        inMemoryRadioButton.Checked = settings.Mode == DatabaseConnectionMode.InMemory;

        productionDatabasePathTextBox.Text = settings.ProductionDatabasePath;
        sandboxDatabasePathTextBox.Text = settings.SandboxDatabasePath;
        settingsPathTextBox.Text = _settingsStore.SettingsPath;
        UpdateEffectivePath();
        SetStatus("Loaded database connection settings.");
    }

    private void SaveSettingsButton_Click(object? sender, EventArgs e)
    {
        SaveSettings();
        SetStatus("Saved database connection settings.");
    }

    private void SaveSettings()
    {
        _settingsStore.Save(new DatabaseConnectionSettings
        {
            Mode = GetSelectedMode(),
            ProductionDatabasePath = productionDatabasePathTextBox.Text.Trim(),
            SandboxDatabasePath = sandboxDatabasePathTextBox.Text.Trim()
        });

        UpdateEffectivePath();
    }

    private DatabaseConnectionMode GetSelectedMode()
    {
        if (productionRadioButton.Checked)
        {
            return DatabaseConnectionMode.Production;
        }

        if (inMemoryRadioButton.Checked)
        {
            return DatabaseConnectionMode.InMemory;
        }

        return DatabaseConnectionMode.SandboxCopy;
    }

    private string GetEffectiveDatabasePath()
    {
        return GetSelectedMode() switch
        {
            DatabaseConnectionMode.Production => productionDatabasePathTextBox.Text.Trim(),
            DatabaseConnectionMode.InMemory => ":memory:",
            _ => sandboxDatabasePathTextBox.Text.Trim()
        };
    }

    private void UpdateEffectivePath()
    {
        effectiveDatabasePathTextBox.Text = GetEffectiveDatabasePath();
    }

    private void CopyProductionToSandboxButton_Click(object? sender, EventArgs e)
    {
        try
        {
            SaveSettings();

            string source = productionDatabasePathTextBox.Text.Trim();
            string target = sandboxDatabasePathTextBox.Text.Trim();

            if (!File.Exists(source))
            {
                MessageBox.Show(this, "Production database was not found.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string? folder = Path.GetDirectoryName(target);
            if (!string.IsNullOrWhiteSpace(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.Copy(source, target, overwrite: true);
            sandboxCopyRadioButton.Checked = true;
            SaveSettings();
            SetStatus("Copied production database to sandbox.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void TestConnectionButton_Click(object? sender, EventArgs e)
    {
        try
        {
            SaveSettings();
            string path = GetEffectiveDatabasePath();
            string connectionString = string.Equals(path, ":memory:", StringComparison.OrdinalIgnoreCase)
                ? "Data Source=:memory:"
                : $"Data Source={path}";

            using SqliteConnection connection = new(connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY name;";

            tableListBox.Items.Clear();
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                tableListBox.Items.Add(reader.GetString(0));
            }

            SetStatus($"Connection OK. Tables found: {tableListBox.Items.Count}");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BrowseProductionButton_Click(object? sender, EventArgs e) => BrowseDatabaseFile(productionDatabasePathTextBox);
    private void BrowseSandboxButton_Click(object? sender, EventArgs e) => BrowseDatabaseFile(sandboxDatabasePathTextBox);
    private void DatabaseMode_CheckedChanged(object? sender, EventArgs e) => UpdateEffectivePath();
    private void OpenSettingsFolderButton_Click(object? sender, EventArgs e) => OpenFolder(_settingsStore.SettingsFolder);
    private void OpenProductionFolderButton_Click(object? sender, EventArgs e) => OpenParentFolder(productionDatabasePathTextBox.Text);
    private void OpenSandboxFolderButton_Click(object? sender, EventArgs e) => OpenParentFolder(sandboxDatabasePathTextBox.Text);

    private static void BrowseDatabaseFile(TextBox target)
    {
        using OpenFileDialog dialog = new()
        {
            Filter = "SQLite database files (*.db;*.sqlite;*.sqlite3)|*.db;*.sqlite;*.sqlite3|All files (*.*)|*.*",
            CheckFileExists = false
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            target.Text = dialog.FileName;
        }
    }

    private static void OpenParentFolder(string filePath)
    {
        string? folder = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            OpenFolder(folder);
        }
    }

    private static void OpenFolder(string folder)
    {
        Directory.CreateDirectory(folder);
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = folder, UseShellExecute = true });
    }

    private void SetStatus(string message) => statusLabel.Text = message;

    private void ShowError(Exception ex)
    {
        SetStatus("Database action failed.");
        MessageBox.Show(this, ex.Message, "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}