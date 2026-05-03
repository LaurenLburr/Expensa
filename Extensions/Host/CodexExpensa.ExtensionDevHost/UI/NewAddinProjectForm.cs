using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.Models;
using CodexExpensa.ExtensionDevHost.Services;
using CodexExpensa.ExtensionDevHost.Services.Ai;

namespace CodexExpensa.ExtensionDevHost.UI;

public sealed partial class NewAddinProjectForm : Form
{
    private readonly TextBox _txtProjectName;
    private readonly TextBox _txtSolutionRoot;
    private readonly TextBox _txtAssemblyName;
    private readonly TextBox _txtDescription;
    private readonly CheckBox _chkRegisterProject;
    private readonly CheckBox _chkUseChatGpt;
    private readonly TextBox _txtChatGptPrompt;
    private readonly Button _btnGenerate;

    public NewAddinProjectForm()
    {
        Text = "New Add-in Project";
        Width = 820;
        Height = 620;
        StartPosition = FormStartPosition.CenterParent;

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 8,
            Padding = new Padding(12)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));

        layout.Controls.Add(new Label { Text = "Project Name", AutoSize = true }, 0, 0);
        _txtProjectName = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_txtProjectName, 1, 0);
        layout.SetColumnSpan(_txtProjectName, 2);

        layout.Controls.Add(new Label { Text = "Solution Root", AutoSize = true }, 0, 1);
        _txtSolutionRoot = new TextBox { Dock = DockStyle.Fill, Text = GuessDefaultSolutionRoot() };
        layout.Controls.Add(_txtSolutionRoot, 1, 1);
        Button browse = new() { Text = "Browse", Dock = DockStyle.Fill };
        browse.Click += (_, _) => BrowseForFolder();
        layout.Controls.Add(browse, 2, 1);

        layout.Controls.Add(new Label { Text = "Assembly Name", AutoSize = true }, 0, 2);
        _txtAssemblyName = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_txtAssemblyName, 1, 2);
        layout.SetColumnSpan(_txtAssemblyName, 2);

        layout.Controls.Add(new Label { Text = "Description", AutoSize = true }, 0, 3);
        _txtDescription = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 70, ScrollBars = ScrollBars.Vertical };
        layout.Controls.Add(_txtDescription, 1, 3);
        layout.SetColumnSpan(_txtDescription, 2);

        _chkRegisterProject = new CheckBox { Text = "Register project immediately", Checked = true, AutoSize = true };
        layout.Controls.Add(_chkRegisterProject, 1, 4);
        layout.SetColumnSpan(_chkRegisterProject, 2);

        _chkUseChatGpt = new CheckBox { Text = "Use ChatGPT to generate starter scaffold", AutoSize = true };
        _chkUseChatGpt.CheckedChanged += (_, _) =>
        {
            bool enabled = _chkUseChatGpt.Checked;
            _txtChatGptPrompt.Enabled = enabled;
            _btnGenerate.Enabled = enabled;
        };
        layout.Controls.Add(_chkUseChatGpt, 1, 5);

        Button apiKey = new() { Text = "OpenAI API Key", AutoSize = true };
        apiKey.Click += (_, _) => { using OpenAiApiKeyForm dialog = new(); dialog.ShowDialog(this); };
        layout.Controls.Add(apiKey, 2, 5);

        layout.Controls.Add(new Label { Text = "ChatGPT Prompt", AutoSize = true }, 0, 6);
        _txtChatGptPrompt = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 160, ScrollBars = ScrollBars.Vertical, Enabled = false };
        layout.Controls.Add(_txtChatGptPrompt, 1, 6);

        _btnGenerate = new Button { Text = "Generate Suggestions", Dock = DockStyle.Top, Enabled = false };
        _btnGenerate.Click += async (_, _) => await ApplyChatGptAsync();
        layout.Controls.Add(_btnGenerate, 2, 6);

        FlowLayoutPanel buttons = new() { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft };
        Button create = new() { Text = "Create", AutoSize = true };
        create.Click += (_, _) => CreateProject();
        Button cancel = new() { Text = "Cancel", AutoSize = true };
        cancel.Click += (_, _) => Close();
        Button openRoot = new() { Text = "Open Root Folder", AutoSize = true };
        openRoot.Click += (_, _) => OpenRootFolder();
        buttons.Controls.Add(create);
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(openRoot);
        layout.Controls.Add(buttons, 1, 7);
        layout.SetColumnSpan(buttons, 2);

        Controls.Add(layout);
    }

    private static string GuessDefaultSolutionRoot()
    {
        string current = AppContext.BaseDirectory;
        DirectoryInfo? directory = new(current);

        while (directory is not null)
        {
            if (string.Equals(directory.Name, "Extensions", StringComparison.OrdinalIgnoreCase))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Extensions");
    }

    private void BrowseForFolder()
    {
        using FolderBrowserDialog dialog = new();
        dialog.SelectedPath = _txtSolutionRoot.Text.Trim();
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _txtSolutionRoot.Text = dialog.SelectedPath;
        }
    }

    private void OpenRootFolder()
    {
        string root = _txtSolutionRoot.Text.Trim();
        if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
        {
            MessageBox.Show(this, "Select a valid solution root folder first.", "New Add-in Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Process.Start(new ProcessStartInfo { FileName = root, UseShellExecute = true });
    }

    private void CreateProject()
    {
        try
        {
            NewAddinProjectRequest request = BuildRequest();
            ExtensionProjectRegistrationStore store = new();
            AddinProjectScaffolder scaffolder = new();

            int sortOrder = request.RegisterProject ? store.GetNextSortOrder() : 10;
            AddinProjectScaffolderResult result = scaffolder.Create(request, sortOrder);

            if (request.RegisterProject)
            {
                store.Upsert(result.Registration);
            }

            StringBuilder message = new();
            message.AppendLine("Created add-in project successfully.");
            message.AppendLine();
            message.AppendLine(result.ProjectFolder);

            MessageBox.Show(this, message.ToString(), "New Add-in Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "New Add-in Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private NewAddinProjectRequest BuildRequest()
    {
        string projectName = _txtProjectName.Text.Trim();
        string solutionRoot = _txtSolutionRoot.Text.Trim();
        string assemblyName = _txtAssemblyName.Text.Trim();

        if (string.IsNullOrWhiteSpace(projectName))
        {
            throw new InvalidOperationException("Project Name is required.");
        }

        if (string.IsNullOrWhiteSpace(solutionRoot) || !Directory.Exists(solutionRoot))
        {
            throw new InvalidOperationException("Solution Root folder must exist.");
        }

        if (string.IsNullOrWhiteSpace(assemblyName))
        {
            assemblyName = projectName.Replace(' ', '_');
        }

        if (_chkUseChatGpt.Checked && string.IsNullOrWhiteSpace(_txtChatGptPrompt.Text))
        {
            throw new InvalidOperationException("Enter a ChatGPT prompt or turn off the ChatGPT option.");
        }

        return new NewAddinProjectRequest
        {
            ProjectName = projectName,
            SolutionRootFolder = solutionRoot,
            AssemblyName = assemblyName,
            Description = _txtDescription.Text.Trim(),
            RegisterProject = _chkRegisterProject.Checked,
            UseChatGpt = _chkUseChatGpt.Checked,
            ChatGptPrompt = _txtChatGptPrompt.Text.Trim()
        };
    }
}
