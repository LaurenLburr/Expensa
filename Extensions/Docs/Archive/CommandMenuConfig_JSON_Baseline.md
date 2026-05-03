# Command Menu Config JSON Baseline

This package moves command menu metadata persistence to JSON and adds live menu redraw.

## File

`%AppData%\Expensa\Extensions\CommandMenuConfig.json`

## What persists

- `CommandKey`
- `MenuText`
- `MenuOrder`
- `ItemOrder`

## What changed

- `CommandRegistry` can now:
  - load config
  - save config
  - update metadata
  - raise `MenuDefinitionChanged`
- `MainForm` rebuilds the menu when metadata changes
- `CommandCatalogForm` now provides:
  - Move Up
  - Move Down
  - Save Changes
  - Open JSON Folder
  - JSON tab for direct editing
- command defaults now come from methods in `ExtMgrCommandBase`, so runtime overrides actually work
