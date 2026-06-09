# Expensa Aggregate Tree Add-in Loader

This slice ports the Extension Manager aggregate loader pattern into Expensa.

Expensa no longer calls only:

```text
WebsiteAddinTreeLoader
```

from `MainForm`.

Instead, `MainForm` uses:

```text
TreeAddinTreeViewLoader
```

which loads known deployed tree add-ins:

```text
WebsitesAddin
BudgetsAddin
```

## Deployment expectation

The add-ins must be deployed under:

```text
Expensa\CodexExpensa.App.WinForms\bin\Debug\net8.0-windows\Modules\WebsitesAddin
Expensa\CodexExpensa.App.WinForms\bin\Debug\net8.0-windows\Modules\BudgetsAddin
```

Each module folder must include its `.dll`, `.deps.json`, `.runtimeconfig.json`, and dependency DLLs.

## Runtime loading

The new loader uses `AssemblyDependencyResolver`, matching the proven Extension Manager loader approach.

## Tree rendering

Websites still renders through the existing website parser/renderer so website node selection keeps working.

Budgets renders year/month nodes using existing Expensa budget tags:

```text
Budget.Month.YYYY.MM
```

so selecting a month opens the existing `BudgetMonthForm`.
