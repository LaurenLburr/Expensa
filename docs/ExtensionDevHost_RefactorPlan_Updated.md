# ExtensionDevHost Refactor Plan (Updated Progress)

## Overall Goal

Transform ExtensionDevHost from a collection of popup utilities into a cohesive workspace shell driven from the main TreeView and embedded content panel.

---

# Completed Work

## Navigation Refactor

### Completed
- Added top-level navigation structure:
  - Project
  - Add-in Projects
  - Templates
  - Tools
  - Docs

### Removed
- AI node
- Project Space node
- Duplicate standalone utility nodes

---

# Project Node

## Completed

### OpenAI API Key
- Converted from popup dialog into embedded workspace panel.
- Loads into `contentPanel`.
- Supports:
  - key editing
  - save
  - reload
  - local validation/testing

### AI Add-in Designer
- Converted into embedded workspace panel.
- Loads on single click.
- Added:
  - add-in name
  - project type
  - summary
  - goals/requirements
  - AI workspace area

### Project Space Creation
The AI Add-in Designer now:
- creates module folders
- creates Docs folder
- creates:
  - CatchUp.md
  - Addin_Design_Spec.md
  - Expensa_Integration_Design_Spec.md
- registers the project
- refreshes the main tree

### Removed
- New Project Space node

---

# Add-in Projects Node

## Completed

Each registered project now contains:

```text
Project
├── Catch-Up Doc
├── Design Spec
├── Expensa Integration Spec
├── Project Folder
└── Docs Folder
```

### Docs Folder
- Enumerates markdown files
- Opens selected docs in DocsEditorForm

### Folder Behavior
- Double-click still opens physical folders

---

# Embedded Workspace Conversion

## Completed

### Tools → Manage Extensions
- Embedded into main content panel
- Added refresh event support

### Tools → Command Catalog
- Embedded into main content panel

### Tools → Query Catalog
- Embedded into main content panel

### Query Catalog
Added:
- SqlQuery table inspection
- Ensure SqlQuery table support

---

# Tree Behavior

## Completed

### Single Click Navigation
Added:
```csharp
NavigationTreeView_NodeMouseClick
```

Tree nodes now respond immediately even when already selected.

### Double Click Behavior
Preserved for:
- opening folders
- external launch actions

---

# Docs Editor Improvements

## Completed

### Markdown Rendering
- Markdown preview rendering working
- Source editor toggle working

### Toolbar
- Standard toolbar behavior
- icon upgrades
- markdown formatting actions

### AI Review
- prompts to save before review
- AI change highlighting
- persistent highlighting until save
- wait cursor during review

### Layout Persistence
- splitter persistence
- main form size persistence
- main form position persistence

---

# Upload / Packaging Improvements

## Completed

### Upload Packaging Script
PowerShell packaging script now:
- runs from solution root
- outputs to:
  ```text
  Extensions\Upload
  ```
- removes previous zip files
- skips:
  - bin
  - obj
  - non-source output files

---

# Remaining Work

## AI Workflow

### Planned
- real AI conversation integration
- streaming AI responses
- AI-generated design refinement
- scaffold regeneration workflows
- AI-generated integration specs

---

# Docs System

## Planned
- project-level editable coding rules
- AI-assisted doc rewriting
- document diff visualization improvements
- richer markdown support

---

# Extension System

## Planned
- extension deployment/update workflow
- backup support before deploy
- extension version management
- enable/disable workflow improvements

---

# Future Workspace Goals

## Planned
- dockable workspace panels
- tabbed document interface
- live AI side-panel
- integrated scaffold diff viewer
- solution-aware navigation
- integrated build/test output panels

---

# Architectural Direction

The application is now transitioning from:

```text
Popup utility launcher
```

to:

```text
Persistent IDE-style workspace shell
```

The major architectural foundation is now in place:
- embedded workspace hosting
- centralized navigation
- project-aware tree structure
- integrated docs workflow
- AI-driven project scaffolding foundation
