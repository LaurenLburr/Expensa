using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost;

public sealed class ProjectSpaceForm : Form
{
    private readonly TextBox _txtName;
    private readonly TextBox _txtRoot;
    private readonly TextBox _txtSolution;
    private readonly Button _btnBrowse;
    private readonly Button _btnCreate;

    public ProjectSpaceForm()
    {
        Text = "Create Project Space";
        Width = 600;
        Height = 250;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 3
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

        layout.Controls.Add(new Label { Text = "Project Space Name", Dock = DockStyle.Fill }, 0, 0);
        _txtName = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_txtName, 1, 0);

        layout.Controls.Add(new Label { Text = "Root Folder", Dock = DockStyle.Fill }, 0, 1);
        _txtRoot = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_txtRoot, 1, 1);

        _btnBrowse = new Button { Text = "Browse", Dock = DockStyle.Fill };
        _btnBrowse.Click += (_, _) => BrowseFolder();
        layout.Controls.Add(_btnBrowse, 2, 1);

        layout.Controls.Add(new Label { Text = "Solution File", Dock = DockStyle.Fill }, 0, 2);
        _txtSolution = new TextBox
        {
            Dock = DockStyle.Fill,
            Text = "Extension_Manager.slnx"
        };
        layout.Controls.Add(_txtSolution, 1, 2);

        _btnCreate = new Button { Text = "Create", Dock = DockStyle.Fill };
        _btnCreate.Click += (_, _) => CreateProjectSpace();
        layout.Controls.Add(_btnCreate, 1, 3);

        Controls.Add(layout);
    }

    private void BrowseFolder()
    {
        using var dlg = new FolderBrowserDialog();
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            _txtRoot.Text = dlg.SelectedPath;
        }
    }

    private void CreateProjectSpace()
    {
        string name = _txtName.Text.Trim();
        string root = _txtRoot.Text.Trim();
        string solution = _txtSolution.Text.Trim();

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(root))
        {
            MessageBox.Show("Name and Root Folder are required.");
            return;
        }

        string basePath = Path.Combine(root, name, "Extensions");

        var created = new StringBuilder();

        CreateDir(basePath, "Host", created);
        CreateDir(basePath, "Core", created);
        CreateDir(basePath, "Modules", created);
        CreateDir(basePath, "Tests", created);

        string slnPath = Path.Combine(basePath, solution);
        if (!File.Exists(slnPath))
        {
            File.WriteAllText(slnPath, GetBasicSolutionFile());
            created.AppendLine($"Created: {slnPath}");
        }

        MessageBox.Show(created.Length == 0
            ? "Nothing created."
            : created.ToString());
    }

    private static void CreateDir(string basePath, string name, StringBuilder log)
    {
        string path = Path.Combine(basePath, name);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            log.AppendLine($"Created: {path}");
        }
    }

    private static string GetBasicSolutionFile()
    {
        return "Microsoft Visual Studio Solution File, Format Version 12.00\n# Visual Studio Version 17\n";
    }
}
