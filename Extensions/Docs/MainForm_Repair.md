# MainForm Repair

## Files

```text
Extensions/Host/CodexExpensa.ExtensionDevHost/Extensions/MainForm.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/Extensions/MainForm.Designer.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/Extensions/MainForm.resx
```

## Fixes

- Replaces the placeholder `MainForm.cs` with a real implementation.
- Restores the designer-backed WinForms structure.
- Restores `MenuStrip`, `SplitContainer`, `TreeView`, and content panel.
- Registers the current command set.
- Adds `AI Add-in Designer` under the AI node.
- Adds registered Manage Extensions projects into the main tree under `Add-in Projects > Registered Projects`.
- Adds `General Coding Rules` to the docs tree.
- Hosts `DocsEditorForm` in the right-side content panel for document editing.
- Refreshes the tree after Manage Extensions, New Add-in Project, or AI Scaffold Files commands.

## Note

I could not run `dotnet build` inside this environment because the .NET SDK is not installed here. The files were checked for consistency against the uploaded project source.
