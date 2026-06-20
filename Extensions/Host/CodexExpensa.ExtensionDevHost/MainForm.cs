
// This file is part of the Codex-Expensa Extension Manager
using CodexExpensa.ExtensionDevHost.Commands;
using CodexExpensa.ExtensionDevHost.Commands.Services;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;
using CodexExpensa.ExtensionDevHost.Models;
using CodexExpensa.ExtensionDevHost.Services;
using CodexExpensa.ExtensionDevHost.UI;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
    private readonly AddinProjectUiSurfaceResolver _addinUiSurfaceResolver = new();

    private readonly ContextMenuStrip _addinProjectContextMenu = new();
    private string _contextMenuProjectName = string.Empty;

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

        ConfigureAddinProjectContextMenu();
    }

    public CommandRegistry CommandRegistry => _commandRegistry;

    public string CommandConfigPath => _commandConfigPath;

    private void ConfigureAddinProjectContextMenu()
    {
        _addinProjectContextMenu.Items.Clear();

        ToolStripMenuItem enableMenuItem = new("Enable Add-in");
        enableMenuItem.Click += (_, _) => SetContextAddinEnabled(isEnabled: true);

        ToolStripMenuItem disableMenuItem = new("Disable Add-in");
        disableMenuItem.Click += (_, _) => SetContextAddinEnabled(isEnabled: false);

        ToolStripMenuItem setDisplaySortMenuItem = new("Set Display Sort...");
        setDisplaySortMenuItem.Click += (_, _) => SetContextAddinDisplaySort();

        _addinProjectContextMenu.Items.Add(enableMenuItem);
        _addinProjectContextMenu.Items.Add(disableMenuItem);
        _addinProjectContextMenu.Items.Add(new ToolStripSeparator());
        _addinProjectContextMenu.Items.Add(setDisplaySortMenuItem);
    }

    private void SetContextAddinEnabled(bool isEnabled)
    {
        if (string.IsNullOrWhiteSpace(_contextMenuProjectName))
        {
            return;
        }

        SetAddinProjectEnabled(_contextMenuProjectName, isEnabled);
        BuildNavigationTree();
        RebuildMenu();
    }

    private void SetContextAddinDisplaySort()
    {
        if (string.IsNullOrWhiteSpace(_contextMenuProjectName))
        {
            return;
        }

        ExtensionProjectRegistration? registration =
            _registrationStore.GetAll()
                .FirstOrDefault(candidate =>
                    string.Equals(candidate.ProjectName, _contextMenuProjectName, StringComparison.OrdinalIgnoreCase));

        if (registration is null)
        {
            MessageBox.Show(
                this,
                $"Could not find registered add-in: {_contextMenuProjectName}",
                "Set Display Sort",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        if (!TryPromptForAddinDisplaySort(registration.ProjectName, registration.DisplaySort, out int displaySort))
        {
            return;
        }

        registration.DisplaySort = displaySort;
        _registrationStore.Upsert(registration);
        BuildNavigationTree();
        RebuildMenu();
    }

    private bool TryPromptForAddinDisplaySort(string projectName, int currentValue, out int displaySort)
    {
        using Form dialog = new()
        {
            Text = $"Display Sort - {projectName}",
            Width = 320,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MinimizeBox = false,
            MaximizeBox = false
        };

        Label label = new()
        {
            Text = "Display sort:",
            AutoSize = true,
            Left = 16,
            Top = 20
        };

        NumericUpDown input = new()
        {
            Left = 120,
            Top = 16,
            Width = 150,
            Minimum = 0,
            Maximum = 100000,
            Value = Math.Clamp(currentValue, 0, 100000)
        };

        Button okButton = new()
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Left = 116,
            Top = 64,
            Width = 75
        };

        Button cancelButton = new()
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Left = 198,
            Top = 64,
            Width = 75
        };

        dialog.Controls.Add(label);
        dialog.Controls.Add(input);
        dialog.Controls.Add(okButton);
        dialog.Controls.Add(cancelButton);
        dialog.AcceptButton = okButton;
        dialog.CancelButton = cancelButton;

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            displaySort = currentValue;
            return false;
        }

        displaySort = decimal.ToInt32(input.Value);
        return true;
    }

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
            tools.Nodes.Add(CreateCommandNode("Deploy All Enabled Add-ins to Expensa", "Tools.DeployAllAddinsToExpensa"));
            tools.Nodes.Add(CreateCommandNode("Expensa Add-in Loader Test", "Tools.ExpensaAddinLoaderTest"));

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

        bool isEnabled =
            IsAddinProjectEnabled(registration.ProjectName);

        TreeNode projectNode = new(isEnabled ? registration.ProjectName : $"{registration.ProjectName} (Disabled)")
        {
            Name = $"addin.{CreateSafeTreeNodeName(registration.ProjectName)}",
            Tag = new ProjectNavigationTag(registration.ProjectName, projectFolder)
        };

        projectNode.ForeColor =
            isEnabled
                ? SystemColors.WindowText
                : SystemColors.GrayText;

        projectNode.ToolTipText =
            isEnabled
                ? "Add-in is enabled."
                : "Add-in is disabled. Disabled add-ins are excluded from Deploy All and the Deploy menu.";

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

        if (e.Button == MouseButtons.Right &&
            e.Node.Tag is ProjectNavigationTag projectTag)
        {
            ShowAddinProjectContextMenu(e.Node, projectTag, e.Location);
            return;
        }

        ShowNavigationNode(e.Node);
    }

    private void ShowAddinProjectContextMenu(TreeNode node, ProjectNavigationTag projectTag, Point location)
    {
        _contextMenuProjectName = projectTag.ProjectName;

        bool isEnabled =
            IsAddinProjectEnabled(projectTag.ProjectName);

        if (_addinProjectContextMenu.Items.Count >= 2)
        {
            _addinProjectContextMenu.Items[0].Enabled = !isEnabled;
            _addinProjectContextMenu.Items[1].Enabled = isEnabled;
        }

        _addinProjectContextMenu.Show(navigationTreeView, location);
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

            case "Tools.DeployAllAddinsToExpensa":
                ShowLandingText(
                    "Deploy All Enabled Add-ins to Expensa" + Environment.NewLine + Environment.NewLine +
                    "Right-click an add-in project node to enable or disable it." + Environment.NewLine + Environment.NewLine +
                    "Only enabled add-ins appear in the Deploy menu and Deploy All operation.");
                break;

            case "Tools.DeployWebsitesAddinToExpensa":
                ShowLandingText(
                    "Deploy Websites Add-in to Expensa" + Environment.NewLine + Environment.NewLine +
                    "This legacy command now routes through the generic enabled add-in deployment pipeline.");
                break;

            case "Tools.ExpensaAddinLoaderTest":
                ShowEmbeddedForm(new ExpensaAddinLoaderTestForm());
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

            case "Tools.DeployAllAddinsToExpensa":
                DeployAllAddinsToExpensa();
                break;

            case "Tools.DeployWebsitesAddinToExpensa":
                DeployRegisteredAddinByName("WebsitesAddin");
                break;

            case "Tools.ExpensaAddinLoaderTest":
                ShowEmbeddedForm(new ExpensaAddinLoaderTestForm());
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







    private bool IsAddinProjectEnabled(string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName))
        {
            return true;
        }

        MainFormSettings settings = LoadMainFormSettings();

        if (settings.AddinProjectEnabledStates is null ||
            !settings.AddinProjectEnabledStates.TryGetValue(projectName, out bool isEnabled))
        {
            return true;
        }

        return isEnabled;
    }

    private void SetAddinProjectEnabled(string projectName, bool isEnabled)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectName);

        MainFormSettings settings = LoadMainFormSettings();

        settings.AddinProjectEnabledStates ??=
            new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        settings.AddinProjectEnabledStates[projectName] = isEnabled;

        SaveMainFormSettings(settings);
    }

    private IReadOnlyList<ExtensionProjectRegistration> GetEnabledAddinRegistrations()
    {
        return _registrationStore.GetAll()
            .Where(registration => IsAddinProjectEnabled(registration.ProjectName))
            .OrderBy(static registration => registration.ProjectName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private void ShowAddinDatabaseNode(AddinProjectDatabaseNavigationTag tag)
    {
        if (_addinUiSurfaceResolver.TryCreateDatabaseForm(tag.ProjectName, tag.ProjectFolder, out Form? form) &&
            form is not null)
        {
            ShowEmbeddedForm(form);
            return;
        }

        ShowEmbeddedForm(new GenericAddinDatabasePanelForm(tag.ProjectName, tag.ProjectFolder));
    }

    private void OpenAddinTestNode(AddinProjectTestNavigationTag tag)
    {
        if (_addinUiSurfaceResolver.TryCreateTestForm(tag.ProjectName, tag.ProjectFolder, out Form? form) &&
            form is not null)
        {
            ShowEmbeddedForm(form);
            return;
        }

        ShowEmbeddedForm(new GenericAddinTreeLoadVerificationForm(tag.ProjectName, tag.ProjectFolder));
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


    private void AddDeploymentMenu()
    {
        ToolStripMenuItem deployMenu = new("Deploy");

        ToolStripMenuItem deployAllAddinsMenuItem = new("Deploy All Enabled Add-ins to Expensa");
        deployAllAddinsMenuItem.Click += (_, _) => DeployAllAddinsToExpensa();

        deployMenu.DropDownItems.Add(deployAllAddinsMenuItem);
        deployMenu.DropDownItems.Add(new ToolStripSeparator());

        IReadOnlyList<ExtensionProjectRegistration> enabledRegistrations =
            GetEnabledAddinRegistrations();

        if (enabledRegistrations.Count == 0)
        {
            deployAllAddinsMenuItem.Enabled = false;

            deployMenu.DropDownItems.Add(
                new ToolStripMenuItem("No enabled add-ins")
                {
                    Enabled = false
                });
        }
        else
        {
            foreach (ExtensionProjectRegistration registration in enabledRegistrations)
            {
                ToolStripMenuItem deployAddinMenuItem =
                    new($"Deploy {registration.ProjectName} to Expensa");

                deployAddinMenuItem.Click += (_, _) => DeployRegisteredAddinToExpensa(registration);

                deployMenu.DropDownItems.Add(deployAddinMenuItem);
            }
        }

        mainMenuStrip.Items.Add(deployMenu);
    }

    private void DeployAllAddinsToExpensa()
    {
        IReadOnlyList<ExtensionProjectRegistration> enabledRegistrations =
            GetEnabledAddinRegistrations();

        if (enabledRegistrations.Count == 0)
        {
            MessageBox.Show(
                this,
                "There are no enabled add-ins to deploy. Right-click an add-in project node and enable at least one add-in.",
                "Deploy All Enabled Add-ins to Expensa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        List<GenericAddinDeploymentResult> successes = [];
        List<string> failures = [];

        foreach (ExtensionProjectRegistration registration in enabledRegistrations)
        {
            try
            {
                successes.Add(DeployRegisteredAddinToExpensaCore(registration));
            }
            catch (Exception exception)
            {
                failures.Add($"{registration.ProjectName}:{Environment.NewLine}{exception.Message}");
            }
        }

        string successText =
            successes.Count == 0
                ? "No add-ins were deployed successfully."
                : string.Join(
                    Environment.NewLine + Environment.NewLine,
                    successes.Select(static result =>
                        $"{result.ProjectName}{Environment.NewLine}" +
                        $"Source: {result.SourceFolder}{Environment.NewLine}" +
                        $"Destination: {result.DestinationFolder}{Environment.NewLine}" +
                        $"Version: {result.Version}{Environment.NewLine}" +
                        $"Deployed: {result.DeployedUtc.ToLocalTime():yyyy-MM-dd HH:mm:ss}{Environment.NewLine}" +
                        $"Files copied: {result.FileCount}"));

        string failureText =
            failures.Count == 0
                ? string.Empty
                : $"{Environment.NewLine}{Environment.NewLine}Failures:{Environment.NewLine}{string.Join(Environment.NewLine + Environment.NewLine, failures)}";

        MessageBox.Show(
            this,
            successText + failureText,
            "Deploy All Enabled Add-ins to Expensa",
            MessageBoxButtons.OK,
            failures.Count == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }

    private void DeployRegisteredAddinByName(string projectName)
    {
        ExtensionProjectRegistration? registration =
            GetEnabledAddinRegistrations()
                .FirstOrDefault(candidate =>
                    string.Equals(candidate.ProjectName, projectName, StringComparison.OrdinalIgnoreCase) ||
                    candidate.ProjectName.Contains(projectName, StringComparison.OrdinalIgnoreCase) ||
                    projectName.Contains(candidate.ProjectName, StringComparison.OrdinalIgnoreCase));

        if (registration is null)
        {
            MessageBox.Show(
                this,
                $"Could not find enabled registered add-in: {projectName}",
                "Deploy Add-in to Expensa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        DeployRegisteredAddinToExpensa(registration);
    }

    private void DeployRegisteredAddinToExpensa(ExtensionProjectRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);

        if (!IsAddinProjectEnabled(registration.ProjectName))
        {
            MessageBox.Show(
                this,
                $"{registration.ProjectName} is disabled. Enable it from the add-in project context menu before deploying.",
                $"Deploy {registration.ProjectName} to Expensa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        try
        {
            GenericAddinDeploymentResult result =
                DeployRegisteredAddinToExpensaCore(registration);

            MessageBox.Show(
                this,
                $"{registration.ProjectName} deployed successfully.{Environment.NewLine}{Environment.NewLine}" +
                $"Source:{Environment.NewLine}{result.SourceFolder}{Environment.NewLine}{Environment.NewLine}" +
                $"Destination:{Environment.NewLine}{result.DestinationFolder}{Environment.NewLine}{Environment.NewLine}" +
                $"Version: {result.Version}{Environment.NewLine}" +
                $"Deployed: {result.DeployedUtc.ToLocalTime():yyyy-MM-dd HH:mm:ss}{Environment.NewLine}" +
                $"Files copied: {result.FileCount}",
                $"Deploy {registration.ProjectName} to Expensa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.ToString(),
                $"Deploy {registration.ProjectName} Failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static GenericAddinDeploymentResult DeployRegisteredAddinToExpensaCore(ExtensionProjectRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);

        string repositoryRoot = FindRepositoryRoot();
        string projectFolder = ResolveProjectFolder(registration);
        string projectFile = FindGenericAddinProjectFile(projectFolder, registration.ProjectName);

        BuildProject(projectFile);

        string sourceFolder = FindGenericAddinOutputFolder(projectFile, registration.ProjectName);
        string destinationFolder = GetGenericExpensaAddinDestinationFolder(repositoryRoot, registration.ProjectName);

        Directory.CreateDirectory(destinationFolder);

        int copiedCount = CopyDirectoryContents(sourceFolder, destinationFolder);

        string deployedDll = Path.Combine(destinationFolder, $"{registration.ProjectName}.dll");

        if (!File.Exists(deployedDll))
        {
            throw new FileNotFoundException(
                $"Deployment completed, but {registration.ProjectName}.dll was not found in the Expensa destination folder.",
                deployedDll);
        }

        AddinDeploymentManifest manifest =
            WriteDeploymentManifest(
                registration.ProjectName,
                deployedDll,
                Path.Combine(sourceFolder, $"{registration.ProjectName}.dll"),
                destinationFolder);

        return new GenericAddinDeploymentResult(
            registration.ProjectName,
            sourceFolder,
            destinationFolder,
            copiedCount,
            manifest.Version,
            manifest.DeployedUtc);
    }

    private static string FindGenericAddinProjectFile(string projectFolder, string projectName)
    {
        string directProjectFile = Path.Combine(projectFolder, $"{projectName}.csproj");

        if (File.Exists(directProjectFile))
        {
            return directProjectFile;
        }

        string? discoveredProject =
            Directory.Exists(projectFolder)
                ? Directory.GetFiles(projectFolder, "*.csproj", SearchOption.TopDirectoryOnly)
                    .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault()
                : null;

        if (!string.IsNullOrWhiteSpace(discoveredProject))
        {
            return discoveredProject;
        }

        throw new FileNotFoundException($"Could not find a project file for {projectName} in {projectFolder}.");
    }

    private static string FindGenericAddinOutputFolder(string projectFile, string projectName)
    {
        string projectFolder =
            Path.GetDirectoryName(projectFile) ??
            throw new DirectoryNotFoundException(projectFile);

        string[] candidateFrameworks = [ "net8.0-windows", "net8.0" ];

        foreach (string framework in candidateFrameworks)
        {
            string candidateFolder = Path.Combine(projectFolder, "bin", "Debug", framework);
            string candidateDll = Path.Combine(candidateFolder, $"{projectName}.dll");

            if (File.Exists(candidateDll))
            {
                return candidateFolder;
            }
        }

        BuildProject(projectFile);

        foreach (string framework in candidateFrameworks)
        {
            string candidateFolder = Path.Combine(projectFolder, "bin", "Debug", framework);
            string candidateDll = Path.Combine(candidateFolder, $"{projectName}.dll");

            if (File.Exists(candidateDll))
            {
                return candidateFolder;
            }
        }

        throw new FileNotFoundException($"Could not find the built {projectName}.dll. Build {projectName} and try again.");
    }

    private static string GetGenericExpensaAddinDestinationFolder(string repositoryRoot, string projectName)
    {
        return Path.Combine(
            repositoryRoot,
            "Expensa",
            "Extensions",
            projectName);
    }

    private void DeployWebsitesAddinToExpensa()
    {
        try
        {
            WebsitesAddinDeploymentResult result = DeployWebsitesAddinToExpensaCore();

            MessageBox.Show(
                this,
                $"WebsitesAddin deployed successfully.{Environment.NewLine}{Environment.NewLine}" +
                $"Source:{Environment.NewLine}{result.SourceFolder}{Environment.NewLine}{Environment.NewLine}" +
                $"Destination:{Environment.NewLine}{result.DestinationFolder}{Environment.NewLine}{Environment.NewLine}" +
                $"Version: {result.Version}{Environment.NewLine}" +
                $"Deployed: {result.DeployedUtc.ToLocalTime():yyyy-MM-dd HH:mm:ss}{Environment.NewLine}" +
                $"Files copied: {result.FileCount}",
                "Deploy Websites Add-in to Expensa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.ToString(),
                "Deploy Websites Add-in Failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static WebsitesAddinDeploymentResult DeployWebsitesAddinToExpensaCore()
    {
        string repositoryRoot = FindRepositoryRoot();
        string projectFile = FindWebsitesAddinProjectFile(repositoryRoot);

        BuildProject(projectFile);

        string sourceFolder = FindWebsitesAddinOutputFolder(repositoryRoot);
        string destinationFolder = GetExpensaWebsitesAddinDestinationFolder(repositoryRoot);

        Directory.CreateDirectory(destinationFolder);

        int copiedCount = CopyDirectoryContents(sourceFolder, destinationFolder);

        string deployedDll = Path.Combine(destinationFolder, "WebsitesAddin.dll");
        if (!File.Exists(deployedDll))
        {
            throw new FileNotFoundException(
                "Deployment completed, but WebsitesAddin.dll was not found in the Expensa destination folder.",
                deployedDll);
        }

        AddinDeploymentManifest manifest =
            WriteDeploymentManifest(
                "WebsitesAddin",
                deployedDll,
                Path.Combine(sourceFolder, "WebsitesAddin.dll"),
                destinationFolder);

        return new WebsitesAddinDeploymentResult(
            sourceFolder,
            destinationFolder,
            copiedCount,
            manifest.Version,
            manifest.DeployedUtc);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            bool hasExtensionsFolder =
                Directory.Exists(Path.Combine(directory.FullName, "Extensions"));

            bool hasExpensaFolder =
                Directory.Exists(Path.Combine(directory.FullName, "Expensa"));

            if (hasExtensionsFolder && hasExpensaFolder)
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find the CodexExpensa repository root. Expected a parent folder containing both Extensions and Expensa folders.");
    }

    private static string FindWebsitesAddinProjectFile(
        string repositoryRoot)
    {
        string[] candidateFiles =
        [
            Path.Combine(repositoryRoot, "Extensions", "Modules", "WebsitesAddin", "WebsitesAddin.csproj"),
            Path.Combine(repositoryRoot, "Extensions", "WebsitesAddin", "WebsitesAddin.csproj"),
            Path.Combine(repositoryRoot, "Extensions", "Addins", "WebsitesAddin", "WebsitesAddin.csproj")
        ];

        foreach (string candidateFile in candidateFiles)
        {
            if (File.Exists(candidateFile))
            {
                return candidateFile;
            }
        }

        string extensionsFolder =
            Path.Combine(repositoryRoot, "Extensions");

        string? discoveredProject =
            Directory.Exists(extensionsFolder)
                ? Directory.GetFiles(extensionsFolder, "WebsitesAddin.csproj", SearchOption.AllDirectories)
                    .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault()
                : null;

        if (!string.IsNullOrWhiteSpace(discoveredProject))
        {
            return discoveredProject;
        }

        throw new FileNotFoundException(
            "Could not find WebsitesAddin.csproj under the Extensions folder.");
    }

    private static void BuildProject(
        string projectFile)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = "dotnet",
            Arguments = $"build \"{projectFile}\" --configuration Debug",
            WorkingDirectory = Path.GetDirectoryName(projectFile) ?? Environment.CurrentDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using Process process = new()
        {
            StartInfo = startInfo
        };

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"dotnet build failed for WebsitesAddin.{Environment.NewLine}{Environment.NewLine}" +
            $"Project:{Environment.NewLine}{projectFile}{Environment.NewLine}{Environment.NewLine}" +
            $"Output:{Environment.NewLine}{output}{Environment.NewLine}{Environment.NewLine}" +
            $"Error:{Environment.NewLine}{error}");
    }

    private static string FindWebsitesAddinOutputFolder(
        string repositoryRoot)
    {
        string[] candidateFolders =
        [
            Path.Combine(repositoryRoot, "Extensions", "Modules", "WebsitesAddin", "bin", "Debug", "net8.0-windows"),
            Path.Combine(repositoryRoot, "Extensions", "WebsitesAddin", "bin", "Debug", "net8.0-windows"),
            Path.Combine(repositoryRoot, "Extensions", "Addins", "WebsitesAddin", "bin", "Debug", "net8.0-windows")
        ];

        foreach (string candidateFolder in candidateFolders)
        {
            string candidateDll =
                Path.Combine(candidateFolder, "WebsitesAddin.dll");

            if (File.Exists(candidateDll))
            {
                return candidateFolder;
            }
        }

        string extensionsFolder =
            Path.Combine(repositoryRoot, "Extensions");

        string? discoveredDll =
            Directory.Exists(extensionsFolder)
                ? Directory.GetFiles(extensionsFolder, "WebsitesAddin.dll", SearchOption.AllDirectories)
                    .Where(static path =>
                        path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) &&
                        path.Contains($"{Path.DirectorySeparatorChar}Debug{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) &&
                        !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) &&
                        !path.Contains($"{Path.DirectorySeparatorChar}ref{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault()
                : null;

        if (!string.IsNullOrWhiteSpace(discoveredDll))
        {
            return Path.GetDirectoryName(discoveredDll) ?? throw new DirectoryNotFoundException(discoveredDll);
        }

        throw new FileNotFoundException(
            "Could not find the built WebsitesAddin.dll. Build the WebsitesAddin project and try again.");
    }

    private static string GetExpensaWebsitesAddinDestinationFolder(
        string repositoryRoot)
    {
        return Path.Combine(
            repositoryRoot,
            "Expensa",
            "Extensions",
            "WebsitesAddin");
    }

    private static AddinDeploymentManifest WriteDeploymentManifest(
        string projectName,
        string deployedAssemblyPath,
        string sourceAssemblyPath,
        string destinationFolder)
    {
        string version = GetAssemblyVersion(deployedAssemblyPath);
        DateTime deployedUtc = DateTime.UtcNow;

        AddinDeploymentManifest manifest =
            new(
                projectName,
                version,
                deployedUtc,
                sourceAssemblyPath,
                deployedAssemblyPath);

        string manifestPath =
            Path.Combine(
                destinationFolder,
                "deployment.json");

        string json =
            JsonSerializer.Serialize(
                manifest,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        File.WriteAllText(
            manifestPath,
            json);

        return manifest;
    }

    private static string GetAssemblyVersion(
        string assemblyPath)
    {
        try
        {
            AssemblyName assemblyName =
                AssemblyName.GetAssemblyName(assemblyPath);

            string? informationalVersion =
                FileVersionInfo.GetVersionInfo(assemblyPath)
                    .ProductVersion;

            if (!string.IsNullOrWhiteSpace(informationalVersion))
            {
                int metadataSeparator =
                    informationalVersion.IndexOf('+');

                return metadataSeparator >= 0
                    ? informationalVersion[..metadataSeparator]
                    : informationalVersion;
            }

            return assemblyName.Version?.ToString()
                ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    private static int CopyDirectoryContents(
        string sourceFolder,
        string destinationFolder)
    {
        int copiedCount = 0;

        foreach (string sourceFile in Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories))
        {
            string relativePath =
                Path.GetRelativePath(sourceFolder, sourceFile);

            if (ShouldSkipDeploymentFile(relativePath))
            {
                continue;
            }

            string destinationFile =
                Path.Combine(destinationFolder, relativePath);

            string? destinationDirectory =
                Path.GetDirectoryName(destinationFile);

            if (!string.IsNullOrWhiteSpace(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            File.Copy(sourceFile, destinationFile, overwrite: true);
            copiedCount++;
        }

        return copiedCount;
    }

    private static bool ShouldSkipDeploymentFile(
        string relativePath)
    {
        string normalized =
            relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        if (normalized.Contains($"{Path.DirectorySeparatorChar}ref{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        string extension =
            Path.GetExtension(relativePath);

        return string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase);
    }

    private sealed record GenericAddinDeploymentResult(
        string ProjectName,
        string SourceFolder,
        string DestinationFolder,
        int FileCount,
        string Version,
        DateTime DeployedUtc);

    private sealed record AddinDeploymentManifest(
        string AddIn,
        string Version,
        DateTime DeployedUtc,
        string SourceAssembly,
        string DeployedAssembly);

    private sealed record WebsitesAddinDeploymentResult(
        string SourceFolder,
        string DestinationFolder,
        int FileCount,
        string Version,
        DateTime DeployedUtc);


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

        AddDeploymentMenu();

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

        public Dictionary<string, bool> AddinProjectEnabledStates { get; set; } =
            new(StringComparer.OrdinalIgnoreCase);
    }
}
