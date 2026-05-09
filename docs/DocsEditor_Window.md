# Docs Editor Window

## Project

`CodexExpensa.ExtensionDevHost`

## Files

```text
UI/DocsEditorForm.cs
MainForm.cs
```

## Behavior

Adds a documentation editor window with:

- TreeView on the left
- Text editor on the right
- Save button
- Reload button
- Open Docs Folder button

The editor loads `.md` and `.txt` files from:

```text
Extensions/Docs
```

## MainForm integration

The left navigation tree now includes:

```text
Docs
└── Edit Documentation
```

Double-clicking any Docs node opens the documentation editor.
