# Extension Dev Host Refactor Plan

## Goal

Turn the main tree into the primary workspace. Tree selection should load sub-forms or document editors into the main content panel. Separate popup forms should mostly go away except for simple dialogs or folder-opening actions.

---

# Phase 1 — Clean Up Main Tree Structure

## New top-level tree layout

```text
Project
├── OpenAI API Key
├── AI Add-in Designer
└── New Project Space

Add-in Projects
└── [Project Name]
    ├── Catch-Up Doc
    ├── Design Spec
    ├── Expensa Integration Spec
    ├── Project Folder
    └── Docs Folder
        └── [doc files...]

Templates
├── Add-in Design Spec Template
└── Expensa Integration Spec Template

Tools
├── Manage Extensions
├── Command Catalog
└── Query Catalog

Docs
├── AI Add-in Design Workflow
└── other remaining docs
```

Remove these from Add-in Projects:

```text
Manage Extensions
AI Scaffold Files
Global Catch-Up Doc
General Coding Rules
Add-in Design Spec Template
Expensa Integration Spec Template
```

Remove the AI node entirely after moving its useful items.

Remove Project Space as a top-level node after moving New Project Space to Project.

---

# Phase 2 — Project Node Changes

## OpenAI API Key

Change from popup/dialog behavior to an embedded sub-form in the main content panel.

Expected behavior:

```text
Project → OpenAI API Key
```

loads an API key management panel with:

```text
Key name
masked key display
show/hide button
save/update button
test API button
```

Move AI Test here as a button/link:

```text
Test OpenAI Connection
```

## AI Add-in Designer

Move to:

```text
Project → AI Add-in Designer
```

Clicking the tree node should load the AI designer into the main content panel, not open a separate form.

## New Project Space

Move to:

```text
Project → New Project Space
```

This can still be a workflow form/panel, but it should load in the main content area.

---

# Phase 3 — Add-in Projects Node

Each registered project should look like this:

```text
Add-in Projects
└── WebsiteTagger
    ├── Catch-Up Doc
    ├── Design Spec
    ├── Expensa Integration Spec
    ├── Project Folder
    └── Docs Folder
        ├── CatchUp.md
        ├── Addin_Design_Spec.md
        └── Expensa_Integration_Design_Spec.md
```

## Project documents

Clicking these loads the markdown editor:

```text
Catch-Up Doc
Design Spec
Expensa Integration Spec
```

## Expensa Integration Spec

This document should specifically describe:

```text
how the add-in will be consumed by Expensa
navigation integration
commands
forms
database/query catalog impact
deployment/update behavior
```

## Project Folder

Keep double-click only behavior:

```text
double-click → open folder in File Explorer
single-click → show simple info panel
```

## Docs Folder

Double-click opens folder.

Single-click expands/list docs.

Each document under Docs Folder should open in the text editor.

---

# Phase 4 — Templates Node

Create a new top-level node:

```text
Templates
```

Move these here:

```text
Add-in Design Spec Template
Expensa Integration Spec Template
```

These should open in the markdown editor.

---

# Phase 5 — Tools Node

Current tools should become embedded panels instead of separate popup forms.

```text
Tools
├── Manage Extensions
├── Command Catalog
└── Query Catalog
```

Clicking a tool node should load its UI into the main content panel.

Target behavior:

```text
single-click tool node → loads tool sub-form into contentPanel
```

No separate window unless explicitly needed.

---

# Phase 6 — Docs Node Cleanup

Remove:

```text
Global Catch-Up Doc
General Coding Spec
```

Keep:

```text
AI Add-in Design Workflow
other useful general docs
```

Templates move out to the new Templates node.

---

# Phase 7 — Implementation Order

## Step 1

Refactor tree-building only.

No behavior changes yet.

## Step 2

Add a generic content-host method:

```csharp
ShowEmbeddedForm(Form form)
```

This becomes the standard way to load sub-forms into contentPanel.

## Step 3

Convert OpenAI API Key into an embedded panel/form.

## Step 4

Convert AI Add-in Designer into embedded content.

## Step 5

Convert Tools forms into embedded content.

## Step 6

Clean up Add-in Projects node and add Docs Folder document listing.

## Step 7

Add Templates node.

## Step 8

Final cleanup:

```text
remove obsolete AI Scaffold Files references
remove obsolete top-level AI node
remove obsolete Project Space node
remove old popup-only commands where no longer needed
```

## Step 9

Update docs:

```text
Catch-Up doc
General Coding Rules
Navigation design spec
Expensa integration design spec
```

## Recommended first coding target

Start with the tree structure and embedded-form host method. That gives us the foundation without breaking every command at once.
