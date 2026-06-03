# Expensa Websites Add-in Tree Integration

## Purpose

This patch changes Expensa so the main navigation tree is populated only by the Websites add-in.

The previous hardcoded tree roots are no longer built by `MainForm.BuildNavigationTree()`.
Instead, the form calls the Websites add-in runtime loader and renders the add-in result into the existing `treeNav` control.

## What Changed

### Modified

- `CodexExpensa.App.WinForms/UI/MainForm.cs`
  - Startup now loads the Websites add-in tree.
  - Refresh now reloads the Websites add-in tree.
  - `BuildNavigationTree()` no longer adds Banks, Accounts, or Budgets nodes.
  - Selected website/category payloads display in a website details panel.

### Added

- `CodexExpensa.App.WinForms/UI/Websites/WebsiteAddinTreeLoader.cs`
- `CodexExpensa.App.WinForms/UI/Websites/WebsiteAddinRuntimeInvoker.cs`
- `CodexExpensa.App.WinForms/UI/Websites/WebsiteAddinAssemblyLocator.cs`
- `CodexExpensa.App.WinForms/UI/Websites/WebsiteRuntimeExecutionResult.cs`
- `CodexExpensa.App.WinForms/UI/Websites/WebsiteTreeLoadOptions.cs`
- `CodexExpensa.App.WinForms/UI/Websites/WebsiteTreeLoadResult.cs`
- `CodexExpensa.App.WinForms/UI/Websites/WebsiteLoadResult.cs`
- `CodexExpensa.App.WinForms/UI/Websites/WebsiteLoadResultParser.cs`
- `CodexExpensa.App.WinForms/UI/Websites/WebsiteTreeNode.cs`
- `CodexExpensa.App.WinForms/UI/Websites/WebsiteTreeViewRenderer.cs`
- `CodexExpensa.App.WinForms/UI/Websites/WebsiteTreeNodeTagReader.cs`
- `CodexExpensa.App.WinForms/UI/Websites/IHostWebsiteTreeNodePayload.cs`
- `CodexExpensa.App.WinForms/UI/Websites/HostWebsiteTreeNodePayload.cs`
- `CodexExpensa.App.WinForms/UI/Websites/HostWebsiteCategoryGroupTreeNodePayload.cs`
- `CodexExpensa.App.WinForms/UI/Websites/HostWebsiteTreeNodeType.cs`
- `CodexExpensa.App.WinForms/UI/Websites/WebsiteDetailsForm.cs`
- `CodexExpensa.App.WinForms/UI/Websites/WebsiteDetailsForm.Designer.cs`

## Websites Add-in Location

At runtime, Expensa searches for:

```text
Modules\WebsitesAddin\WebsitesAddin.dll
```

The search checks the app output folder, current working folder, and ancestor folders.

## Important

This patch does not deploy the Websites add-in DLL itself. The add-in still needs to be built/copied into a supported `Modules\WebsitesAddin` folder.
