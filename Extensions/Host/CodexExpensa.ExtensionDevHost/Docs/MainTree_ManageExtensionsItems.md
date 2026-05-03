# Main Tree Manage Extensions Items

## Change

The registered items from Manage Extensions are now displayed directly in the main TreeView under:

```text
Add-in Projects
└── Registered Projects
    └── [ProjectName]
        ├── Catch-Up Doc
        ├── Design Spec
        ├── Expensa Integration Spec
        ├── Project Folder
        └── Docs Folder
```

## Files

```text
Extensions/Host/CodexExpensa.ExtensionDevHost/MainForm.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/MainForm.Designer.cs
```

## Behavior

- Selecting a project document opens it in `DocsEditorForm`.
- Double-clicking folder nodes opens the folder.
- Double-clicking command nodes invokes the command.
- Running Manage Extensions refreshes the main tree after the command completes.
