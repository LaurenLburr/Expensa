# Budgets Add-in Phase 1

This slice creates the initial Budgets add-in module.

## Goal

Load this tree shape:

```text
Budgets
    2026
        January
        February
```

## Included module files

```text
Extensions\Modules\BudgetsAddin
```

## Runtime command

```text
Budgets.LoadTree
```

## Data source

Initial query reads distinct year/month values from:

```sql
[Budget].[BudgetMonth]
```

## Important

This first slice does not yet wire the Budgets node into the Expensa main tree.

Next slice should modify the Expensa-side tree loader to call:

```text
BudgetsAddin.BudgetLoadRuntimeSmokeRunner
```

using the same dependency-aware loading pattern already fixed for Websites.
