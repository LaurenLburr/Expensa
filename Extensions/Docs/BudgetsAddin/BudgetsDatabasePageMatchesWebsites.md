# Budgets Database Page Matches Websites

This slice replaces the placeholder Budgets database page with a Websites-style add-in database page.

## Adds

```text
HostBudgetDatabasePathService
HostBudgetRuntimeDatabaseSelectionService
HostBudgetDatabaseCopyService
HostBudgetExpensaProdDatabaseCopyService
BudgetsDatabasePanelForm
BudgetFlatRowBuilder
```

## Runtime database

Budgets now uses:

```text
%APPDATA%\Expensa\Extensions\Runtime\BudgetsAddin\budgets.current.db
```

## Copy from Expensa Prod

The page copies:

```text
%LOCALAPPDATA%\CodexExpensa\db\codexexpensa.db
```

to:

```text
budgets.current.db
```

That gives the Budgets add-in access to the same real Expensa tables as the app.

## Repository fix

`SqliteBudgetRepository` now reads:

```sql
[BudgetMonth].[BudgetMonthId]
[BudgetMonth].[Year]
[BudgetMonth].[Month]
```

instead of querying a nonexistent `[Budget]` table.
