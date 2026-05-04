# Extension Dev Host Refactor Step 1

## Scope

This is the first implementation step from the refactor plan.

## Changes

### Main tree structure

The top-level tree is now:

```text
Project
Add-in Projects
Templates
Tools
Docs
```

Removed top-level:

```text
AI
Project Space
```

### Project node

Now contains:

```text
OpenAI API Key
AI Add-in Designer
New Project Space
```

### Add-in Projects node

Now contains only registered add-in projects.

Removed from Add-in Projects:

```text
Manage Extensions
AI Scaffold Files
Global Catch-Up Doc
General Coding Rules
Add-in Design Spec Template
Expensa Integration Spec Template
```

### Templates node

Added:

```text
Add-in Design Spec Template
Expensa Integration Spec Template
```

### Docs node

Now keeps:

```text
AI Add-in Design Workflow
```

Removed:

```text
Global Catch-Up Doc
General Coding Spec
```

### Project Docs Folder

Each project now gets:

```text
Docs Folder
└── *.md files
```

Markdown files under the project docs folder load in the text editor.

### Embedded form host method

Added:

```csharp
ShowEmbeddedForm(Form form)
```

This is the foundation for converting pop-up tools into main-panel embedded subforms in later steps.

## Files

```text
Extensions/Host/CodexExpensa.ExtensionDevHost/Extensions/MainForm.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/Extensions/MainForm.Designer.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/Extensions/MainForm.resx
```

## Next Step

Convert `OpenAI API Key` from a popup command into an embedded subform loaded into the main content panel.
