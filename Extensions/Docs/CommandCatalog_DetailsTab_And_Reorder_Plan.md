# Command Catalog Details Tab + Up/Down Reorder Plan

## What this zip contains

This is **not** a source-code implementation yet.

Only a catch-up/implementation plan is included because the current conversation only contains the catch-up document and not the actual current source files needed to safely modify:

- `CommandCatalogForm`
- `OpenCommandCatalogCommand`
- `MainForm`
- `CommandRegistry`
- any JSON config service/model classes involved

The uploaded catch-up file confirms the architecture and desired direction, but it is not enough to produce trustworthy full replacement files without risking regressions.

## Requested behavior to implement

### Details tab
Add a **Details** tab to the Command Catalog UI that shows the selected command and allows editing of:

- `MenuText`
- `TopLevelMenu`
- `MenuOrder`
- `ItemOrder`

Recommended read-only fields:

- `CommandKey`
- command type/source
- default values vs effective values, if available

### Up/Down controls
For order editing, use:

- numeric controls
- **Up** and **Down** buttons

#### Item order behavior
Move the selected command one position earlier/later **within the same top-level menu**.

#### Menu order behavior (Option A)
Move the **entire top-level menu group** earlier/later relative to other top-level menus.

### Rebuild behavior
As requested, `RebuildMenu()` should:

1. loop through all commands
2. group by `TopLevelMenu`
3. sort menu groups by current effective order
4. renumber menu groups cleanly (`10, 20, 30...`)
5. sort items inside each menu
6. renumber items cleanly (`10, 20, 30...`)
7. rebuild the visible menu

This makes `RebuildMenu()` both:
- a renderer
- a normalizer

## Why implementation is paused here

The catch-up document explicitly says to work from real provided files and not invent project structures. The current conversation includes only the catch-up document, not the current code files.

Without the real current files, producing “full files” would require guessing:
- namespaces
- constructor signatures
- controls already present on the form
- existing JSON config load/save APIs
- exact registry/model names
- how selection state is currently tracked

That would be a great way to create a very polished bug.

## Files needed next to implement safely

Please provide the current versions of:

- `Host/CodexExpensa.ExtensionDevHost/UI/CommandCatalogForm.cs`
- `Host/CodexExpensa.ExtensionDevHost/Commands/OpenCommandCatalogCommand.cs`
- `Host/CodexExpensa.ExtensionDevHost/UI/MainForm.cs`
- the current command metadata/registry class file
- the current JSON config model/service file, if separate

## Recommended implementation order once files are uploaded

1. Add the Details tab UI
2. Bind selection from the list/grid to the Details tab
3. Add Up/Down buttons for item movement
4. Add Up/Down buttons for top-level menu movement
5. Update `RebuildMenu()` to renumber menu and item order every rebuild
6. Save normalized JSON after rebuild-triggering edits
7. Refresh the list/grid/details state after save/rebuild

## Source used

The current plan is based on the uploaded catch-up document in this conversation.
