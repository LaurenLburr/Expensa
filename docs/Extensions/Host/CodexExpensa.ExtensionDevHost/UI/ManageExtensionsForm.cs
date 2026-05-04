using System.Diagnostics;
using CodexExpensa.ExtensionDevHost.Models;
using CodexExpensa.ExtensionDevHost.Services;

namespace CodexExpensa.ExtensionDevHost.UI;

public sealed class ManageExtensionsForm : Form
{
    private readonly ExtensionProjectRegistrationStore _store = new();

    private readonly TreeView _tree = new();
    private readonly DataGridView _grid = new();
    private readonly BindingSource _bindingSource = new();

    private readonly Button _refreshButton = new();
    private readonly Button _openFolderButton = new();
    private readonly Button _unregisterButton = new();
    private readonly Button _deleteFilesButton = new();
    private readonly TextBox _databasePathTextBox = new();

    private readonly SplitContainer _split = new();

    private List<ExtensionProjectRegistration> _all = new();

    public event EventHandler? RegistrationsChanged;

    public ManageExtensionsForm()
    {
        Text = "Manage Extensions";
        Width = 1100;
        Height = 600;
        StartPosition = FormStartPosition.CenterParent;

        InitializeUi();
        LoadRegistrations();

        Shown += (_, _) => ApplySafeSplitterLayout();
        Resize += (_, _) => ApplySafeSplitterLayout();
    }

    private void InitializeUi()
    {
        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(10)
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

        _databasePathTextBox.Dock = DockStyle.Fill;
        _databasePathTextBox.ReadOnly = true;
        _databasePathTextBox.Text = _store.DatabasePath;
        layout.Controls.Add(_databasePathTextBox, 0, 0);

        _split.Dock = DockStyle.Fill;
        _split.Orientation = Orientation.Vertical;
        _split.FixedPanel = FixedPanel.Panel1;

        _tree.Dock = DockStyle.Fill;
        _tree.AfterSelect += (_, _) => FilterGrid();
        _split.Panel1.Controls.Add(_tree);

        _grid.Dock = DockStyle.Fill;
        _grid.AutoGenerateColumns = false;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.ReadOnly = true;

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Project",
            DataPropertyName = nameof(ExtensionProjectRegistration.ProjectName),
            Width = 200
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Assembly",
            DataPropertyName = nameof(ExtensionProjectRegistration.AssemblyName),
            Width = 220
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Relative Bin Path",
            DataPropertyName = nameof(ExtensionProjectRegistration.RelativeBinPath),
            Width = 250
        });

        _grid.Columns.Add(new DataGridViewCheckBoxColumn
        {
            HeaderText = "Enabled",
            DataPropertyName = nameof(ExtensionProjectRegistration.IsEnabled),
            Width = 80
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Sort",
            DataPropertyName = nameof(ExtensionProjectRegistration.SortOrder),
            Width = 70
        });

        _grid.DataSource = _bindingSource;
        _split.Panel2.Controls.Add(_grid);

        layout.Controls.Add(_split, 0, 1);

        FlowLayoutPanel buttons = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft
        };

        _refreshButton.Text = "Refresh";
        _refreshButton.AutoSize = true;
        _refreshButton.Click += (_, _) => LoadRegistrations();

        _openFolderButton.Text = "Open Folder";
        _openFolderButton.AutoSize = true;
        _openFolderButton.Click += (_, _) => OpenSelectedFolder();

        _unregisterButton.Text = "Unregister";
        _unregisterButton.AutoSize = true;
        _unregisterButton.Click += (_, _) => UnregisterSelected();

        _deleteFilesButton.Text = "Delete Files + Unregister";
        _deleteFilesButton.AutoSize = true;
        _deleteFilesButton.Click += (_, _) => DeleteSelectedFilesAndUnregister();

        buttons.Controls.Add(_refreshButton);
        buttons.Controls.Add(_openFolderButton);
        buttons.Controls.Add(_unregisterButton);
        buttons.Controls.Add(_deleteFilesButton);

        layout.Controls.Add(buttons, 0, 2);

        Controls.Add(layout);
    }

    private void ApplySafeSplitterLayout()
    {
        if (_split.Width <= 0)
        {
            return;
        }

        const int desiredLeftWidth = 300;
        const int minimumLeftWidth = 180;
        const int minimumRightWidth = 350;

        int maximumAllowedLeftWidth = _split.Width - minimumRightWidth;

        if (maximumAllowedLeftWidth < minimumLeftWidth)
        {
            return;
        }

        int safeLeftWidth = Math.Min(desiredLeftWidth, maximumAllowedLeftWidth);
        safeLeftWidth = Math.Max(minimumLeftWidth, safeLeftWidth);

        if (_split.SplitterDistance != safeLeftWidth)
        {
            _split.SplitterDistance = safeLeftWidth;
        }
    }

    private void LoadRegistrations()
    {
        _all = _store.GetAll().ToList();
        BuildTree();
        _bindingSource.DataSource = _all;
    }

    private void BuildTree()
    {
        _tree.Nodes.Clear();

        TreeNode root = new("Extensions");
        TreeNode enabled = new("Enabled");
        TreeNode disabled = new("Disabled");

        foreach (ExtensionProjectRegistration ext in _all)
        {
            TreeNode node = new(ext.ProjectName) { Tag = ext };

            if (ext.IsEnabled)
            {
                enabled.Nodes.Add(node);
            }
            else
            {
                disabled.Nodes.Add(node);
            }
        }

        root.Nodes.Add(enabled);
        root.Nodes.Add(disabled);

        _tree.Nodes.Add(root);
        root.ExpandAll();
    }

    private void FilterGrid()
    {
        if (_tree.SelectedNode?.Tag is ExtensionProjectRegistration ext)
        {
            _bindingSource.DataSource = new List<ExtensionProjectRegistration> { ext };
        }
        else if (_tree.SelectedNode?.Text == "Enabled")
        {
            _bindingSource.DataSource = _all.Where(static x => x.IsEnabled).ToList();
        }
        else if (_tree.SelectedNode?.Text == "Disabled")
        {
            _bindingSource.DataSource = _all.Where(static x => !x.IsEnabled).ToList();
        }
        else
        {
            _bindingSource.DataSource = _all;
        }
    }

    private ExtensionProjectRegistration? GetSelectedRegistration()
    {
        return _bindingSource.Current as ExtensionProjectRegistration;
    }

    private void OpenSelectedFolder()
    {
        ExtensionProjectRegistration? registration = GetSelectedRegistration();
        if (registration is null)
        {
            MessageBox.Show(this, "Select an extension first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string projectFolder = ResolveProjectFolder(registration);
        if (!Directory.Exists(projectFolder))
        {
            MessageBox.Show(this, "Project folder was not found:" + Environment.NewLine + projectFolder, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = projectFolder,
            UseShellExecute = true
        });
    }

    private void UnregisterSelected()
    {
        ExtensionProjectRegistration? registration = GetSelectedRegistration();
        if (registration is null)
        {
            MessageBox.Show(this, "Select an extension first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        DialogResult confirm = MessageBox.Show(
            this,
            $"Unregister '{registration.ProjectName}'? Files will be left on disk.",
            Text,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        _store.Delete(registration.ProjectName);
        LoadRegistrations();
        RegistrationsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void DeleteSelectedFilesAndUnregister()
    {
        ExtensionProjectRegistration? registration = GetSelectedRegistration();
        if (registration is null)
        {
            MessageBox.Show(this, "Select an extension first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string projectFolder = ResolveProjectFolder(registration);

        DialogResult confirm = MessageBox.Show(
            this,
            $"Delete files and unregister '{registration.ProjectName}'?{Environment.NewLine}{Environment.NewLine}{projectFolder}",
            Text,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        if (Directory.Exists(projectFolder))
        {
            Directory.Delete(projectFolder, recursive: true);
        }

        _store.Delete(registration.ProjectName);
        LoadRegistrations();
        RegistrationsChanged?.Invoke(this, EventArgs.Empty);
    }

    private static string ResolveProjectFolder(ExtensionProjectRegistration registration)
    {
        string solutionRoot = FindSolutionRoot();

        if (!string.IsNullOrWhiteSpace(registration.RelativeBinPath))
        {
            string relativeBin = registration.RelativeBinPath;
            string modulesMarker = "Modules" + Path.DirectorySeparatorChar;

            string normalized = relativeBin
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            int markerIndex = normalized.IndexOf(modulesMarker, StringComparison.OrdinalIgnoreCase);
            if (markerIndex >= 0)
            {
                string afterModules = normalized[(markerIndex + modulesMarker.Length)..];
                string[] parts = afterModules.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0)
                {
                    return Path.Combine(solutionRoot, "Modules", parts[0]);
                }
            }
        }

        return Path.Combine(solutionRoot, "Modules", registration.ProjectName);
    }

    private static string FindSolutionRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (string.Equals(directory.Name, "Extensions", StringComparison.OrdinalIgnoreCase))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Extensions");
    }
}
