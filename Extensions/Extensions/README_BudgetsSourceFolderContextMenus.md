# Budgets Update-Link Context Menus

## Change

Adds right-click context menus to the existing Budgets database update links.

### Update Data from Prod

- `Reload` — runs the same Prod refresh as clicking the link.
- `Open Prod folder location` — opens the folder containing the Prod source database.

### Update Data from Dev

- `Reload` — runs the same Dev refresh as clicking the link.
- `Open Dev folder location` — opens the folder containing the Dev source database.

The reload command continues to use the existing memory-database workflow: the source file is opened only long enough to copy into memory and is then closed.

## Root

Extract this zip into:

`D:\Git\CodexExpensa`

The archive is rooted at `Extensions\...`.

## Files

### Replaced

- `Extensions\Host\CodexExpensa.ExtensionDevHost\CommandEngineIntegration\Budgets\BudgetsDatabasePanelForm.cs`
- `Extensions\Tests\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests\Budgets\BudgetsDatabasePanelSourceFolderContextMenuTests.cs`
- `Extensions\README_BudgetsSourceFolderContextMenus.md`

## Template policy

No files under `CommandEngineIntegration\Templates` were changed. The context menus are attached by the derived `BudgetsDatabasePanelForm` to the existing protected link controls.

## Database impact

Code-only. No schema change. Reload can replace the add-in runtime database from the selected source and run the existing seed process, exactly as clicking the corresponding update link already does.

## Build and test expectation

Build the Extensions solution and run the CommandEngineIntegration test project. Then open the Budgets database page, right-click each update link, and verify both `Reload` and the source-folder command.
