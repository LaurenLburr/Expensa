namespace CodexExpensa.ExtensionDevHost.Services;

public sealed class WorkspaceDocumentService
{
    private static readonly WorkspaceDocumentDefinition[] DefaultDocuments =
    [
        new(
            "AddinDesignSpecTemplate.md",
            """
# Add-in Design Spec Template

## Purpose

Describe the add-in purpose.

## Requirements

- 

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

## Open Questions

- 

"""),

        new(
            "ExpensaIntegrationSpecTemplate.md",
            """
# Expensa Integration Spec Template

## Purpose

Describe how the add-in integrates into Expensa.

## Integration Points

- Navigation
- Commands
- Database
- Query Catalog
- Docs

## Host Touchpoints

| Host Area | Integration Point | Notes |
|---|---|---|
| Main navigation tree | | |
| Menus/commands | | |
| Detail panel/content host | | |
| Database/session services | | |
| Query catalog / SqlQuery | | |
| Settings/configuration | | |

## Open Questions

- 

"""),

        new(
            "AiAddinDesignWorkflow.md",
            """
# AI Add-in Design Workflow

## Current Workflow

1. Open AI Add-in Designer.
2. Enter add-in information.
3. Create project space.
4. Generate required docs.
5. Iterate with AI.

## Planned Workflow

1. Real AI conversation integration.
2. AI-generated specs.
3. Scaffold generation.
4. Regeneration workflows.
5. Deployment support.

"""),

        new(
            "WorkspaceRoadmap.md",
            """
# Workspace Roadmap

## Current State

The application has transitioned from popup-driven tooling into an embedded workspace shell.

## Completed

- Embedded workspace panels
- Embedded docs workflow
- AI Add-in Designer
- Project space generation
- Query Catalog
- Command Catalog
- Manage Extensions workspace
- Markdown editing/rendering
- AI review workflow
- Upload packaging automation
- Main form size/position persistence
- Splitter persistence

## Next Major Goals

1. Real AI conversation streaming
2. AI-generated scaffold regeneration
3. Diff-aware AI editing
4. Dockable panels
5. Tabbed documents
6. Integrated build/test output
7. Deployment/update workflow
8. Workspace document catalog management
9. Project-level coding rules
10. AI-assisted design review and rewrite

## Near-Term Cleanup

- Extract more workspace services out of MainForm.
- Make DocsEditorForm less dependent on WebBrowser quirks.
- Add better document dirty-state tracking.
- Add a proper document catalog instead of hardcoded tree nodes.

## Long-Term Direction

The host should become a lightweight IDE-style shell for designing, scaffolding, reviewing, and maintaining Expensa add-ins.

""")
    ];

    public WorkspaceDocumentService()
    {
        string settingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions");

        WorkspaceDocsFolder = Path.Combine(settingsFolder, "WorkspaceDocs");
    }

    public string WorkspaceDocsFolder { get; }

    public void EnsureDefaultDocuments()
    {
        Directory.CreateDirectory(WorkspaceDocsFolder);

        foreach (WorkspaceDocumentDefinition document in DefaultDocuments)
        {
            EnsureDocument(document.FileName, document.Content);
        }
    }

    public string GetDocumentPath(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        return Path.Combine(WorkspaceDocsFolder, fileName);
    }

    private void EnsureDocument(string fileName, string content)
    {
        string path = GetDocumentPath(fileName);

        if (!File.Exists(path))
        {
            File.WriteAllText(path, content);
            return;
        }

        string existing = File.ReadAllText(path);

        if (string.IsNullOrWhiteSpace(existing))
        {
            File.WriteAllText(path, content);
        }
    }

    private sealed record WorkspaceDocumentDefinition(string FileName, string Content);
}
