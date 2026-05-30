
// This file is part of the Codex-Expensa Extension Manager
using CodexExpensa.ExtensionDevHost.Commands;
using CodexExpensa.ExtensionDevHost.Commands.Services;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
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
    private readonly WorkspaceDocumentService _workspaceDocuments = new();

    private DocsEditorForm? _docsEditorForm;

    private IReadOnlySet<string> _lastNavigationExpandedNodeNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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

        EnsureWorkspaceDocumentsExist();
        _lastNavigationExpandedNodeNames = LoadSavedNavigationExpandedNodeNames();
        BuildNavigationTree();
        RebuildMenu();


        Shown += (_, _) => RestoreMainSplitterDistance();
        FormClosing += (_, _) => SaveMainSplitterDistance();
        FormClosing += (_, _) => SaveNavigationExpandedNodeNames();
        mainSplitContainer.SplitterMoved += (_, _) => SaveMainSplitterDistance();
        navigationTreeView.AfterExpand += NavigationTreeView_ExpansionChanged;
        navigationTreeView.AfterCollapse += NavigationTreeView_ExpansionChanged;
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
    private void EnsureWorkspaceDocumentsExist()
    {
        _workspaceDocuments.EnsureDefaultDocuments();
    }

    private string GetWorkspaceDocsFolder()
    {
        return _workspaceDocuments.WorkspaceDocsFolder;
    }

    private void BuildNavigationTree()
    {
        IReadOnlySet<string> expandedBeforeRebuild =
            CaptureExpandedNavigationNodeNames();

        navigationTreeView.BeginUpdate();

        try
        {
            navigationTreeView.Nodes.Clear();

            TreeNode project = new("Project")
            {
                Name = "project"
            };
            project.Nodes.Add(CreateCommandNode("OpenAI API Key", "Project.OpenAiApiKey"));
            project.Nodes.Add(CreateCommandNode("Database Connection", "Project.DatabaseConnection"));
            project.Nodes.Add(CreateCommandNode("AI Add-in Designer", "Tools.AiAddinDesigner"));

            TreeNode addinProjects = BuildAddinProjectsNode();

            TreeNode templates = new("Templates")
            {
                Name = "templates"
            };
            templates.Nodes.Add(CreateWorkspaceDocNode("Add-in Design Spec Template", "AddinDesignSpecTemplate.md"));
            templates.Nodes.Add(CreateWorkspaceDocNode("Expensa Integration Spec Template", "ExpensaIntegrationSpecTemplate.md"));

            TreeNode tools = new("Tools")
            {
                Name = "tools"
            };
            tools.Nodes.Add(CreateCommandNode("Manage Extensions", "Tools.ManageExtensions"));
            tools.Nodes.Add(CreateCommandNode("Command Catalog", "Tools.CommandCatalog"));
            tools.Nodes.Add(CreateCommandNode("Query Catalog", "Tools.QueryCatalog"));
            tools.Nodes.Add(CreateCommandNode("Folder Watcher / Auto Unzip", "Tools.FolderWatcherAutoUnzip"));
            tools.Nodes.Add(CreateCommandNode("CommandEngine Runtime", "Tools.CommandEngineRuntime"));

            TreeNode docs = new("Docs")
            {
                Name = "docs"
            };

            TreeNode aiInstructions = new("AI Instructions / General Rules")
            {
                Name = "docs.aiInstructions"
            };
            aiInstructions.Nodes.Add(CreateWorkspaceDocNode("General Coding Rules", "GeneralCodingRules.md"));
            aiInstructions.Nodes.Add(CreateWorkspaceDocNode("General Coding Spec", "GeneralCodingSpec.md"));
            aiInstructions.Nodes.Add(CreateWorkspaceDocNode("AI Add-in Design Workflow", "AiAddinDesignWorkflow.md"));
            aiInstructions.Nodes.Add(CreateWorkspaceDocNode("Add-in Design Spec Template", "AddinDesignSpecTemplate.md"));
            aiInstructions.Nodes.Add(CreateWorkspaceDocNode("Expensa Integration Spec Template", "ExpensaIntegrationSpecTemplate.md"));
            aiInstructions.Nodes.Add(CreateCommandNode("Update Standard AI Docs", "Docs.UpdateStandardAiDocs"));

            docs.Nodes.Add(aiInstructions);
            docs.Nodes.Add(CreateWorkspaceDocNode("Workspace Roadmap", "Roadmap.md"));

            navigationTreeView.Nodes.Add(project);
            navigationTreeView.Nodes.Add(addinProjects);
            navigationTreeView.Nodes.Add(templates);
            navigationTreeView.Nodes.Add(tools);
            navigationTreeView.Nodes.Add(docs);

            IReadOnlySet<string> stateToRestore =
                expandedBeforeRebuild.Count > 0
                    ? expandedBeforeRebuild
                    : _lastNavigationExpandedNodeNames;

            RestoreExpandedNavigationNodeNames(stateToRestore);

            _lastNavigationExpandedNodeNames =
                CaptureExpandedNavigationNodeNames();
        }
        finally
        {
            navigationTreeView.EndUpdate();
        }
    }
    private IReadOnlySet<string> CaptureExpandedNavigationNodeNames()
    {
        HashSet<string> expandedNodeNames =
            new(StringComparer.OrdinalIgnoreCase);

        foreach (TreeNode node in navigationTreeView.Nodes)
        {
            CaptureExpandedNavigationNodeNames(node, expandedNodeNames);
        }

        return expandedNodeNames;
    }

    private static void CaptureExpandedNavigationNodeNames(
        TreeNode node,
        HashSet<string> expandedNodeNames)
    {
        if (node.IsExpanded && !string.IsNullOrWhiteSpace(node.Name))
        {
            expandedNodeNames.Add(node.Name);
        }

        foreach (TreeNode child in node.Nodes)
        {
            CaptureExpandedNavigationNodeNames(child, expandedNodeNames);
        }
    }

    private void RestoreExpandedNavigationNodeNames(
        IReadOnlySet<string> expandedNodeNames)
    {
        foreach (TreeNode node in navigationTreeView.Nodes)
        {
            RestoreExpandedNavigationNodeNames(node, expandedNodeNames);
        }
    }

    private static void RestoreExpandedNavigationNodeNames(
        TreeNode node,
        IReadOnlySet<string> expandedNodeNames)
    {
        if (!string.IsNullOrWhiteSpace(node.Name) &&
            expandedNodeNames.Contains(node.Name))
        {
            node.Expand();
        }
        else
        {
            node.Collapse();
        }

        foreach (TreeNode child in node.Nodes)
        {
            RestoreExpandedNavigationNodeNames(child, expandedNodeNames);
        }
    }

    private IReadOnlySet<string> LoadSavedNavigationExpandedNodeNames()
    {
        MainFormSettings settings =
            LoadMainFormSettings();

        return new HashSet<string>(
            settings.NavigationExpandedNodeNames ?? [],
            StringComparer.OrdinalIgnoreCase);
    }

    private void SaveNavigationExpandedNodeNames()
    {
        try
        {
            MainFormSettings settings =
                LoadMainFormSettings();

            settings.NavigationExpandedNodeNames =
                CaptureExpandedNavigationNodeNames()
                    .OrderBy(static name => name, StringComparer.OrdinalIgnoreCase)
                    .ToList();

            SaveMainFormSettings(settings);
        }
        catch
        {
            // Tree expansion persistence should never break the host.
        }
    }

    private void NavigationTreeView_ExpansionChanged(
        object? sender,
        TreeViewEventArgs e)
    {
        _lastNavigationExpandedNodeNames =
            CaptureExpandedNavigationNodeNames();

        SaveNavigationExpandedNodeNames();
    }

    private static string CreateSafeTreeNodeName(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "empty";
        }

        char[] chars =
            value.Trim()
                .Select(static character =>
                    char.IsLetterOrDigit(character)
                        ? character
                        : '.')
                .ToArray();

        return new string(chars)
            .Replace("..", ".", StringComparison.Ordinal)
            .Trim('.');
    }

    private TreeNode BuildAddinProjectsNode()
    {
        TreeNode addinProjects = new("Add-in Projects")
        {
            Name = "addinProjects"
        };

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
            Name = $"addin.{CreateSafeTreeNodeName(registration.ProjectName)}",
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

        projectNode.Nodes.Add(new TreeNode("Database")
        {
            Name = $"addin.{CreateSafeTreeNodeName(registration.ProjectName)}.database",
            Tag = new AddinProjectDatabaseNavigationTag(
                registration.ProjectName,
                projectFolder)
        });

        projectNode.Nodes.Add(new TreeNode("Test")
        {
            Name = $"addin.{CreateSafeTreeNodeName(registration.ProjectName)}.test",
            Tag = new AddinProjectTestNavigationTag(
                registration.ProjectName,
                projectFolder)
        });

        projectNode.Nodes.Add(new TreeNode("Project Folder")
        {
            Name = $"addin.{CreateSafeTreeNodeName(registration.ProjectName)}.projectFolder",
            Tag = new FolderNavigationTag(projectFolder)
        });

        TreeNode docsFolderNode = new("Docs Folder")
        {
            Name = $"addin.{CreateSafeTreeNodeName(registration.ProjectName)}.docsFolder",
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

    
    private TreeNode CreateWorkspaceDocNode(string caption, string fileName)
    {
        string path = Path.Combine(GetWorkspaceDocsFolder(), fileName);

        return new TreeNode(caption)
        {
            Name = $"workspaceDoc.{CreateSafeTreeNodeName(fileName)}",
            Tag = new ProjectDocumentNavigationTag(
                "Workspace",
                path,
                ProjectDocumentKind.Other)
        };
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
            Name = $"docs.{CreateSafeTreeNodeName(docsTag)}",
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
            Name = $"addin.{CreateSafeTreeNodeName(projectName)}.doc.{CreateSafeTreeNodeName(kind.ToString())}",
            Tag = new ProjectDocumentNavigationTag(projectName, filePath, kind)
        };
    }
    private void NavigationTreeView_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Node is null)
        {
            return;
        }

        navigationTreeView.SelectedNode = e.Node;
        ShowNavigationNode(e.Node);
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

            case AddinProjectDatabaseNavigationTag databaseTag:
                ShowAddinDatabaseNode(databaseTag);
                break;

            case AddinProjectTestNavigationTag testTag:
                OpenAddinTestNode(testTag);
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

            case AddinProjectDatabaseNavigationTag databaseTag:
                ShowAddinDatabaseNode(databaseTag);
                break;

            case AddinProjectTestNavigationTag testTag:
                OpenAddinTestNode(testTag);
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

            case "Project.DatabaseConnection":
                ShowEmbeddedForm(new DatabaseConnectionPanelForm());
                break;

            //case "Project.DatabaseConnection":
            //    ShowEmbeddedForm(new DatabaseConnectionPanelForm());
            //    break;

            case "Tools.AiAddinDesigner":
                ShowAiAddinDesignerPanel();
                break;

            case "Tools.ManageExtensions":
                ShowManageExtensionsPanel();
                break;

            case "Tools.QueryCatalog":
                ShowQueryCatalogPanel();
                break;

            case "Tools.CommandCatalog":
                ShowCommandCatalogPanel();
                break;

            case "Tools.FolderWatcherAutoUnzip":
                ShowFolderWatcherAutoUnzipPanel();
                break;

            case "Tools.CommandEngineRuntime":
                ShowCommandEngineRuntimePanel();
                break;

            case "Docs.UpdateStandardAiDocs":
                ShowStandardAiDocsUpdatePanel();
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

            case "Tools.QueryCatalog":
                ShowQueryCatalogPanel();
                break;

            case "Tools.CommandCatalog":
                ShowCommandCatalogPanel();
                break;

            case "Tools.FolderWatcherAutoUnzip":
                ShowFolderWatcherAutoUnzipPanel();
                break;

            case "Tools.CommandEngineRuntime":
                ShowCommandEngineRuntimePanel();
                break;

            case "Docs.UpdateStandardAiDocs":
                ShowStandardAiDocsUpdatePanel();
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







    private void ShowAddinDatabaseNode(AddinProjectDatabaseNavigationTag tag)
    {
        if (IsWebsitesAddinProject(tag.ProjectName))
        {
            ShowEmbeddedForm(new WebsitesDatabasePanelForm());
            return;
        }

        ShowLandingText(
            $"{tag.ProjectName} Database{Environment.NewLine}{Environment.NewLine}" +
            $"Project folder:{Environment.NewLine}{tag.ProjectFolder}{Environment.NewLine}{Environment.NewLine}" +
            "Database page is not wired yet.");
    }

    private void OpenAddinTestNode(AddinProjectTestNavigationTag tag)
    {
        if (IsWebsitesAddinProject(tag.ProjectName))
        {
            ShowEmbeddedForm(new ExtensionTreeLoadTestForm());
            return;
        }

        ShowLandingText(
            $"{tag.ProjectName} Test{Environment.NewLine}{Environment.NewLine}" +
            "No add-in-specific test form is wired yet.");
    }

    private static bool IsWebsitesAddinProject(
        string projectName)
    {
        return string.Equals(projectName, "WebsitesAddin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(projectName, "Websites Add-in", StringComparison.OrdinalIgnoreCase) ||
            projectName.Contains("Website", StringComparison.OrdinalIgnoreCase);
    }

    private void ShowCommandEngineRuntimePanel()
    {
        ShowEmbeddedForm(new ExtensionRuntimeDashboardForm());
    }

    private void ShowStandardAiDocsUpdatePanel()
    {
        ShowEmbeddedForm(new StandardAiDocsUpdateForm());
    }

    private void ShowFolderWatcherAutoUnzipPanel()
    {
        ShowEmbeddedForm(new FolderWatcherAutoUnzipForm());
    }

    private void ShowCommandCatalogPanel()
    {
        CommandCatalogForm panel = new(
            _commandRegistry,
            _commandConfigPath,
            RebuildMenu);

        ShowEmbeddedForm(panel);
    }

    private void ShowQueryCatalogPanel()
    {
        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "ExtensionMgr.db");

        ShowEmbeddedForm(new QueryCatalogForm(dbPath));
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

        if (tag.Kind == ProjectDocumentKind.DesignSpec)
        {
            ShowEmbeddedForm(new DesignSpecConversationForm(tag.ProjectName, tag.FilePath));
            return;
        }

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

    private sealed record AddinProjectDatabaseNavigationTag(
        string ProjectName,
        string ProjectFolder);

    private sealed record AddinProjectTestNavigationTag(
        string ProjectName,
        string ProjectFolder);

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

        public List<string> NavigationExpandedNodeNames { get; set; } = [];
    }
}
