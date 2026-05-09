# Extension Dev Host Refactor Step 7 - Document Workspace

## Scope

Converts remaining static document nodes into editable workspace documents.

## Templates

These nodes now open directly in DocsEditorForm:

Templates
- Add-in Design Spec Template
- Expensa Integration Spec Template

## Docs

These nodes now open directly in DocsEditorForm:

Docs
- AI Add-in Design Workflow

## Workspace Document Storage

Workspace-level markdown docs are now stored under:

%AppData%\Expensa\Extensions\WorkspaceDocs

## Automatic Creation

The application now ensures these documents exist at startup:

- AddinDesignSpecTemplate.md
- ExpensaIntegrationSpecTemplate.md
- AiAddinDesignWorkflow.md

## Architectural Direction

The tree is now transitioning into a true document workspace where:
- navigation nodes map to editable documents
- templates are live markdown files
- workflow docs are editable
- AI workflows can directly rewrite documents later
