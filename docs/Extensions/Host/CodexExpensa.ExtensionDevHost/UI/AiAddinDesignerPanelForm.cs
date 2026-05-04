using CodexExpensa.ExtensionDevHost.Models;
using CodexExpensa.ExtensionDevHost.Services;

namespace CodexExpensa.ExtensionDevHost.UI;

public partial class AiAddinDesignerPanelForm : Form
{
    private readonly ExtensionProjectRegistrationStore _registrationStore = new();
    private readonly AddinProjectScaffolder _scaffolder = new();

    public AiAddinDesignerPanelForm()
    {
        InitializeComponent();
        solutionRootTextBox.Text = FindSolutionRoot();
    }

    public event EventHandler? ProjectSpaceChanged;

    private void CreateProjectSpaceButton_Click(object? sender, EventArgs e)
    {
        CreateOrUpdateProjectSpace();
    }

    private void StartConversationButton_Click(object? sender, EventArgs e)
    {
        if (!CreateOrUpdateProjectSpace())
        {
            return;
        }

        statusLabel.Text =
            "Project space is ready. The next implementation step will connect this panel to the real AI conversation service.";
    }

    private void GenerateDocsButton_Click(object? sender, EventArgs e)
    {
        if (!CreateOrUpdateProjectSpace())
        {
            return;
        }

        statusLabel.Text = "Required project docs exist. Future step: ask AI to rewrite/update them.";
    }

    private bool CreateOrUpdateProjectSpace()
    {
        string projectName = addinNameTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(projectName))
        {
            MessageBox.Show(
                this,
                "Enter an add-in name before creating the project space.",
                "AI Add-in Designer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return false;
        }

        string solutionRoot = solutionRootTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(solutionRoot) || !Directory.Exists(solutionRoot))
        {
            MessageBox.Show(
                this,
                "The solution root folder is required and must exist.",
                "AI Add-in Designer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return false;
        }

        try
        {
            string sanitizedProjectName = SanitizeProjectName(projectName);
            string projectFolder = Path.Combine(solutionRoot, "Modules", sanitizedProjectName);
            string docsFolder = Path.Combine(projectFolder, "Docs");

            if (!Directory.Exists(projectFolder))
            {
                NewAddinProjectRequest request = new()
                {
                    ProjectName = sanitizedProjectName,
                    SolutionRootFolder = solutionRoot,
                    AssemblyName = sanitizedProjectName,
                    Description = summaryTextBox.Text,
                    RegisterProject = true,
                    UseChatGpt = false,
                    ChatGptPrompt = goalsTextBox.Text
                };

                AddinProjectScaffolderResult result = _scaffolder.Create(
                    request,
                    _registrationStore.GetNextSortOrder());

                _registrationStore.Upsert(result.Registration);
            }
            else
            {
                ExtensionProjectRegistration registration = new()
                {
                    ProjectName = sanitizedProjectName,
                    AssemblyName = sanitizedProjectName,
                    RelativeBinPath = Path.Combine("Modules", sanitizedProjectName, "bin", "Debug", "net8.0-windows"),
                    IsEnabled = true,
                    SortOrder = _registrationStore.GetNextSortOrder()
                };

                _registrationStore.Upsert(registration);
            }

            Directory.CreateDirectory(docsFolder);

            EnsureDocument(
                Path.Combine(docsFolder, "CatchUp.md"),
                BuildCatchUpDocument(sanitizedProjectName));

            EnsureDocument(
                Path.Combine(docsFolder, "Addin_Design_Spec.md"),
                BuildDesignSpecDocument(sanitizedProjectName));

            EnsureDocument(
                Path.Combine(docsFolder, "Expensa_Integration_Design_Spec.md"),
                BuildExpensaIntegrationSpecDocument(sanitizedProjectName));

            projectFolderTextBox.Text = projectFolder;
            docsFolderTextBox.Text = docsFolder;

            statusLabel.Text = $"Project space ready: {projectFolder}";
            ProjectSpaceChanged?.Invoke(this, EventArgs.Empty);

            MessageBox.Show(
                this,
                "Project space, docs folder, required docs, and registration are ready.",
                "AI Add-in Designer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Create Project Space",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }
    }

    private static void EnsureDocument(string path, string content)
    {
        string? folder = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        if (!File.Exists(path))
        {
            File.WriteAllText(path, content);
        }
    }

    private static string BuildCatchUpDocument(string projectName)
    {
        return $"""
# {projectName} – Catch-Up

## Current Status

Project space created from the AI Add-in Designer.

## Recent Changes

- Created project folder.
- Created Docs folder.
- Created required project documents.
- Registered project with the Extension Manager database.

## Next Steps

- Refine the design spec with AI.
- Refine the Expensa integration spec.
- Generate or revise scaffold code.

""";
    }

    private string BuildDesignSpecDocument(string projectName)
    {
        return $"""
# {projectName} – Add-in Design Spec

## Purpose

{summaryTextBox.Text.Trim()}

## Goals / Requirements

{goalsTextBox.Text.Trim()}

## User Workflow

1. 
2. 
3. 

## Navigation / Tree Structure

```text
Add-in Projects
└── {projectName}
    ├── Catch-Up Doc
    ├── Design Spec
    ├── Expensa Integration Spec
    ├── Project Folder
    └── Docs Folder
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

## Open Questions

- 

""";
    }

    private string BuildExpensaIntegrationSpecDocument(string projectName)
    {
        return $"""
# {projectName} – Expensa Integration Design Spec

## Purpose

Describe how this add-in will be consumed by Expensa.

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

1. Extension Manager reads project registration.
2. Expensa loads the add-in project metadata.
3. Expensa displays the add-in in the appropriate navigation area.

## Deployment / Update Flow

Describe how this add-in should be deployed or updated into Expensa.

## Risks / Open Questions

- 

""";
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

    private static string SanitizeProjectName(string value)
    {
        string trimmed = value.Trim();
        char[] invalid = Path.GetInvalidFileNameChars();

        string cleaned = new(trimmed
            .Where(ch => !invalid.Contains(ch))
            .Select(ch => char.IsLetterOrDigit(ch) || ch is '_' or '.' ? ch : '_')
            .ToArray());

        return cleaned.Trim('_');
    }
}
