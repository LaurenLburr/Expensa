using CodexExpensa.ExtensionDevHost.Commands;
using CodexExpensa.ExtensionDevHost.Commands.Services;
using CodexExpensa.ExtensionDevHost.Models;
using CodexExpensa.ExtensionDevHost.Services;
using CodexExpensa.ExtensionDevHost.UI;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost;

public partial class MainForm : Form
{
    private readonly CommandRegistry _commandRegistry;
    private readonly string _commandConfigPath;
    private readonly string _mainFormSettingsPath;
    private readonly ExtensionProjectRegistrationStore _registrationStore = new();

    private DocsEditorForm? _docsEditorForm;

    public MainForm()
    {
        InitializeComponent();

        Text = "Extension Dev Host";

        string settingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions");

        _commandConfigPath = Path.Combine(
            settingsFolder,
            "CommandMenuConfig.json");

        _mainFormSettingsPath = Path.Combine(
            settingsFolder,
            "MainFormSettings.json");

        _commandRegistry = new CommandRegistry(
            new JsonCommandMenuConfigStore(_commandConfigPath));

        _commandRegistry.MenuDefinitionChanged += (_, _) => RebuildMenu();

        RegisterCommands();
        _commandRegistry.LoadConfig();

        BuildNavigationTree();
        RebuildMenu();

        Shown += (_, _) => RestoreMainSplitterDistance();
        FormClosing += (_, _) => SaveMainSplitterDistance();
        mainSplitContainer.SplitterMoved += (_, _) => SaveMainSplitterDistance();
    }

    public CommandRegistry CommandRegistry => _commandRegistry;

    public string CommandConfigPath => _commandConfigPath;

    private void RegisterCommands()
    {
        _commandRegistry.Register(new OpenAiApiKeyCommand());
        _commandRegistry.Register(new OpenAiAddinDesignerCommand());
        _commandRegistry.Register(new AiTestCommand());
        _commandRegistry.Register(new AiScaffoldFilesCommand());
        _commandRegistry.Register(new NewAddinProjectCommand());
        _commandRegistry.Register(new NewProjectSpaceCommand());
        _commandRegistry.Register(new OpenManageExtensionsCommand());
        _commandRegistry.Register(new OpenCommandCatalogCommand());
        _commandRegistry.Register(new OpenQueryCatalogCommand());
        _commandRegistry.Register(new ExitApplicationCommand());
    }

    private void BuildNavigationTree()
    {
        navigationTreeView.BeginUpdate();

        try
        {
            navigationTreeView.Nodes.Clear();

            TreeNode project = new("Project");
            project.Nodes.Add(CreateCommandNode("OpenAI API Key", "Project.OpenAiApiKey"));
            project.Nodes.Add(CreateCommandNode("AI Add-in Designer", "Tools.AiAddinDesigner"));

            TreeNode addinProjects = BuildAddinProjectsNode();

            TreeNode templates = new("Templates");
            templates.Nodes.Add(CreateDocsNode("Add-in Design Spec Template", "Docs.AddinDesignSpecTemplate"));
            templates.Nodes.Add(CreateDocsNode("Expensa Integration Spec Template", "Docs.ExpensaIntegrationSpecTemplate"));

            TreeNode tools = new("Tools");
            tools.Nodes.Add(CreateCommandNode("Manage Extensions", "Tools.ManageExtensions"));
            tools.Nodes.Add(CreateCommandNode("Command Catalog", "Tools.CommandCatalog"));
            tools.Nodes.Add(CreateCommandNode("Query Catalog", "Tools.QueryCatalog"));

            TreeNode docs = new("Docs");
            docs.Nodes.Add(CreateDocsNode("AI Add-in Design Workflow", "Docs.AiAddinDesignWorkflow"));

            navigationTreeView.Nodes.Add(project);
            navigationTreeView.Nodes.Add(addinProjects);
            navigationTreeView.Nodes.Add(templates);
            navigationTreeView.Nodes.Add(tools);
            navigationTreeView.Nodes.Add(docs);

            navigationTreeView.ExpandAll();
        }
        finally
        {
            navigationTreeView.EndUpdate();
        }
    }

    private TreeNode BuildAddinProjectsNode()
    {
        TreeNode addinProjects = new("Add-in Projects");

        foreach (ExtensionProjectRegistration registration in _registrationStore.GetAll())
        {
            addinProjects.Nodes.Add(BuildRegisteredProjectNode(registration));
        }

        return addinProjects;
    }

    private TreeNode BuildRegisteredProjectNode(ExtensionProjectRegistration registration)
    {
        string projectFolder = ResolveProjectFolder(registration);
        string docsFolder = Path.Combine(projectFolder, "Docs");

        TreeNode projectNode = new(registration.ProjectName)
        {
            Tag = new ProjectNavigationTag(registration.ProjectName, projectFolder)
        };

        projectNode.Nodes.Add(CreateProjectDocNode(
            "Catch-Up Doc",
            registration.ProjectName,
            Path.Combine(docsFolder, "CatchUp.md"),
            ProjectDocumentKind.CatchUp));

        projectNode.Nodes.Add(CreateProjectDocNode(
            "Design Spec",
            registration.ProjectName,
            Path.Combine(docsFolder, "Addin_Design_Spec.md"),
            ProjectDocumentKind.DesignSpec));

        projectNode.Nodes.Add(CreateProjectDocNode(
            "Expensa Integration Spec",
            registration.ProjectName,
            Path.Combine(docsFolder, "Expensa_Integration_Design_Spec.md"),
            ProjectDocumentKind.ExpensaIntegrationSpec));

        projectNode.Nodes.Add(new TreeNode("Project Folder")
        {
            Tag = new FolderNavigationTag(projectFolder)
        });

        TreeNode docsFolderNode = new("Docs Folder")
        {
            Tag = new FolderNavigationTag(docsFolder)
        };

        AddProjectDocFiles(docsFolderNode, registration.ProjectName, docsFolder);

        projectNode.Nodes.Add(docsFolderNode);

        return projectNode;
    }

    private static void AddProjectDocFiles(TreeNode docsFolderNode, string projectName, string docsFolder)
    {
        if (!Directory.Exists(docsFolder))
        {
            return;
        }

        foreach (string filePath in Directory.GetFiles(docsFolder, "*.md").OrderBy(static x => x))
        {
            docsFolderNode.Nodes.Add(new TreeNode(Path.GetFileName(filePath))
            {
                Tag = new ProjectDocumentNavigationTag(
                    projectName,
                    filePath,
                    ProjectDocumentKind.Other)
            });
        }
    }

    private static TreeNode CreateCommandNode(string text, string commandKey)
    {
        return new TreeNode(text)
        {
            Tag = new CommandNavigationTag(commandKey)
        };
    }

    private static TreeNode CreateDocsNode(string text, string docsTag)
    {
        return new TreeNode(text)
        {
            Tag = new GlobalDocumentNavigationTag(docsTag)
        };
    }

    private static TreeNode CreateProjectDocNode(
        string text,
        string projectName,
        string filePath,
        ProjectDocumentKind kind)
    {
        return new TreeNode(text)
        {
            Tag = new ProjectDocumentNavigationTag(projectName, filePath, kind)
        };
    }

    private void NavigationTreeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node is null)
        {
            return;
        }

        ShowNavigationNode(e.Node);
    }

    private void NavigationTreeView_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Node is null)
        {
            return;
        }

        ExecuteNavigationNode(e.Node);
    }

    private void ShowNavigationNode(TreeNode node)
    {
        switch (node.Tag)
        {
            case ProjectNavigationTag projectTag:
                ShowLandingText(
                    $"{projectTag.ProjectName}{Environment.NewLine}{Environment.NewLine}" +
                    $"Project folder:{Environment.NewLine}{projectTag.ProjectFolder}{Environment.NewLine}{Environment.NewLine}" +
                    "Select a document or folder under this project.");
                break;

            case ProjectDocumentNavigationTag documentTag:
                ShowProjectDocument(documentTag);
                break;

            case GlobalDocumentNavigationTag globalDocumentTag:
                ShowGlobalDocument(globalDocumentTag);
                break;

            case FolderNavigationTag folderTag:
                ShowLandingText(
                    $"Folder:{Environment.NewLine}{folderTag.FolderPath}{Environment.NewLine}{Environment.NewLine}" +
                    "Double-click to open this folder.");
                break;

            case CommandNavigationTag commandTag:
                ShowCommandNode(commandTag);
                break;

            default:
                ShowLandingText(
                    $"{node.Text}{Environment.NewLine}{Environment.NewLine}" +
                    "Select or double-click a child item.");
                break;
        }
    }

    private void ExecuteNavigationNode(TreeNode node)
    {
        switch (node.Tag)
        {
            case CommandNavigationTag commandTag:
                ExecuteCommandNode(commandTag);
                break;

            case ProjectDocumentNavigationTag documentTag:
                ShowProjectDocument(documentTag);
                break;

            case GlobalDocumentNavigationTag globalDocumentTag:
                ShowGlobalDocument(globalDocumentTag);
                break;

            case FolderNavigationTag folderTag:
                OpenFolder(folderTag.FolderPath);
                break;
        }
    }


    private void ShowCommandNode(CommandNavigationTag commandTag)
    {
        switch (commandTag.CommandKey)
        {
            case "Project.OpenAiApiKey":
                ShowEmbeddedForm(new OpenAiApiKeyPanelForm());
                break;

            case "Tools.AiAddinDesigner":
                ShowAiAddinDesignerPanel();
                break;

            case "Tools.ManageExtensions":
                ShowManageExtensionsPanel();
                break;

            default:
                ShowLandingText(
                    $"Command:{Environment.NewLine}{commandTag.CommandKey}{Environment.NewLine}{Environment.NewLine}" +
                    "Double-click to run this command.");
                break;
        }
    }

    private void ExecuteCommandNode(CommandNavigationTag commandTag)
    {
        switch (commandTag.CommandKey)
        {
            case "Project.OpenAiApiKey":
                ShowEmbeddedForm(new OpenAiApiKeyPanelForm());
                break;

            case "Tools.AiAddinDesigner":
                ShowAiAddinDesignerPanel();
                break;

            case "Tools.ManageExtensions":
                ShowManageExtensionsPanel();
                break;

            default:
                InvokeCommand(commandTag.CommandKey);
                break;
        }
    }

    private void InvokeCommand(string commandKey)
    {
        try
        {
            _commandRegistry.Invoke(commandKey, this);

            if (string.Equals(commandKey, "Tools.ManageExtensions", StringComparison.OrdinalIgnoreCase))
            {
                BuildNavigationTree();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Command Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }




    private void ShowManageExtensionsPanel()
    {
        ManageExtensionsForm panel = new();
        panel.RegistrationsChanged += (_, _) => BuildNavigationTree();
        ShowEmbeddedForm(panel);
    }

    private void ShowAiAddinDesignerPanel()
    {
        AiAddinDesignerPanelForm panel = new();
        panel.ProjectSpaceChanged += (_, _) => BuildNavigationTree();
        ShowEmbeddedForm(panel);
    }

    private void ShowEmbeddedForm(Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        _docsEditorForm = null;

        contentPanel.Controls.Clear();

        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;

        contentPanel.Controls.Add(form);

        form.Show();
        form.PerformLayout();
        form.Refresh();
    }

    private void ShowLandingText(string text)
    {
        _docsEditorForm = null;

        contentPanel.Controls.Clear();

        Label label = new()
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = text
        };

        contentPanel.Controls.Add(label);
    }

    private void ShowGlobalDocument(GlobalDocumentNavigationTag tag)
    {
        DocsEditorForm editor = EnsureDocsEditor();

        editor.SelectDocumentByTag(tag.DocsTag);
        editor.FocusEditor();
    }

    private void ShowProjectDocument(ProjectDocumentNavigationTag tag)
    {
        EnsureProjectDocumentExists(tag);

        DocsEditorForm editor = EnsureDocsEditor();

        editor.LoadDocumentPath(tag.FilePath);
        editor.FocusEditor();
    }

    private DocsEditorForm EnsureDocsEditor()
    {
        if (_docsEditorForm is not null && !_docsEditorForm.IsDisposed)
        {
            return _docsEditorForm;
        }

        contentPanel.Controls.Clear();

        _docsEditorForm = new DocsEditorForm
        {
            TopLevel = false,
            FormBorderStyle = FormBorderStyle.None,
            Dock = DockStyle.Fill
        };

        contentPanel.Controls.Add(_docsEditorForm);
        _docsEditorForm.Show();
        _docsEditorForm.PerformLayout();

        return _docsEditorForm;
    }

    private static void EnsureProjectDocumentExists(ProjectDocumentNavigationTag tag)
    {
        string? folder = Path.GetDirectoryName(tag.FilePath);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        if (File.Exists(tag.FilePath))
        {
            return;
        }

        string content = tag.Kind switch
        {
            ProjectDocumentKind.CatchUp => CreateProjectCatchUpTemplate(tag.ProjectName),
            ProjectDocumentKind.DesignSpec => CreateProjectDesignSpecTemplate(tag.ProjectName),
            ProjectDocumentKind.ExpensaIntegrationSpec => CreateExpensaIntegrationSpecTemplate(tag.ProjectName),
            _ => $"# {tag.ProjectName}{Environment.NewLine}"
        };

        File.WriteAllText(tag.FilePath, content);
    }

    private static string CreateProjectCatchUpTemplate(string projectName)
    {
        return $"""
# {projectName} – Catch-Up

## Current Status

Describe the current state of this add-in project.

## Recent Changes

- 

## Important Decisions

- 

## Known Issues

- 

## Next Steps

- 

## Notes for AI

Use this section to summarize anything the AI needs to remember when helping with this project.
""";
    }

    private static string CreateProjectDesignSpecTemplate(string projectName)
    {
        return $"""
# {projectName} – Add-in Design Spec

## Purpose

Describe what this add-in is intended to do.

## User Workflow

1. 
2. 
3. 

## Navigation / Tree Structure

```text
Root
└── Child
```

## Commands

| Command | Purpose |
|---|---|
| | |

## Forms

| Form | Purpose |
|---|---|
| | |

## Data Model

| Entity | Key Fields |
|---|---|
| | |

## Database Design

| Table | Purpose |
|---|---|
| | |

## Revision History

| Date | Change |
|---|---|
| | |
""";
    }

    private static string CreateExpensaIntegrationSpecTemplate(string projectName)
    {
        return $"""
# {projectName} – Expensa Integration Design Spec

## Purpose

Describe how this add-in integrates with Expensa.

## Host Touchpoints

| Host Area | Integration Point | Notes |
|---|---|---|
| Main navigation tree | | |
| Menus/commands | | |
| Detail panel/content host | | |
| Database/session services | | |
| Query catalog / SqlQuery | | |
| Settings/configuration | | |

## Startup / Registration Flow

1. 
2. 
3. 

## Deployment / Update Flow

Describe how this add-in should be deployed or updated into Expensa.

## Risks / Open Questions

- 
""";
    }

    private static void OpenFolder(string folderPath)
    {
        Directory.CreateDirectory(folderPath);

        Process.Start(new ProcessStartInfo
        {
            FileName = folderPath,
            UseShellExecute = true
        });
    }

    private static string ResolveProjectFolder(ExtensionProjectRegistration registration)
    {
        string solutionRoot = FindSolutionRoot();

        if (!string.IsNullOrWhiteSpace(registration.RelativeBinPath))
        {
            string normalized = registration.RelativeBinPath
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            string modulesMarker = "Modules" + Path.DirectorySeparatorChar;
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


    private void RestoreMainSplitterDistance()
    {
        MainFormSettings settings = LoadMainFormSettings();

        if (settings.MainSplitterDistance <= 0)
        {
            return;
        }

        mainSplitContainer.SplitterDistance = GetSafeMainSplitterDistance(settings.MainSplitterDistance);
    }

    private void SaveMainSplitterDistance()
    {
        try
        {
            MainFormSettings settings = LoadMainFormSettings();
            settings.MainSplitterDistance = mainSplitContainer.SplitterDistance;
            SaveMainFormSettings(settings);
        }
        catch
        {
            // Splitter persistence should never break the host.
        }
    }

    private int GetSafeMainSplitterDistance(int requestedDistance)
    {
        if (mainSplitContainer.Width <= 0)
        {
            return requestedDistance;
        }

        int minimumDistance = Math.Max(mainSplitContainer.Panel1MinSize, 180);
        int maximumDistance = mainSplitContainer.Width - Math.Max(mainSplitContainer.Panel2MinSize, 320);

        if (maximumDistance < minimumDistance)
        {
            return Math.Max(mainSplitContainer.Panel1MinSize, Math.Min(requestedDistance, mainSplitContainer.Width - mainSplitContainer.Panel2MinSize));
        }

        return Math.Max(minimumDistance, Math.Min(requestedDistance, maximumDistance));
    }

    private MainFormSettings LoadMainFormSettings()
    {
        try
        {
            if (!File.Exists(_mainFormSettingsPath))
            {
                return new MainFormSettings();
            }

            string json = File.ReadAllText(_mainFormSettingsPath);
            return JsonSerializer.Deserialize<MainFormSettings>(json) ?? new MainFormSettings();
        }
        catch
        {
            return new MainFormSettings();
        }
    }

    private void SaveMainFormSettings(MainFormSettings settings)
    {
        string? folder = Path.GetDirectoryName(_mainFormSettingsPath);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_mainFormSettingsPath, json);
    }

    public void RebuildMenu()
    {
        if (mainMenuStrip is null)
        {
            return;
        }

        mainMenuStrip.Items.Clear();

        _commandRegistry.PopulateMenu(
            mainMenuStrip,
            this);

        MainMenuStrip = mainMenuStrip;
    }

    private sealed record CommandNavigationTag(string CommandKey);

    private sealed record GlobalDocumentNavigationTag(string DocsTag);

    private sealed record ProjectNavigationTag(string ProjectName, string ProjectFolder);

    private sealed record FolderNavigationTag(string FolderPath);

    private sealed record ProjectDocumentNavigationTag(
        string ProjectName,
        string FilePath,
        ProjectDocumentKind Kind);

    private enum ProjectDocumentKind
    {
        CatchUp,
        DesignSpec,
        ExpensaIntegrationSpec,
        Other
    }

    private sealed class MainFormSettings
    {
        public int MainSplitterDistance { get; set; }
    }
}
