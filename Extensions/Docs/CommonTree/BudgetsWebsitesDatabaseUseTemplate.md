# Budgets and Websites Database Forms Use DatabasePanelTemplate

This converts the other two database pages to the same template inheritance pattern used by Payees.

## Converted

```text
BudgetsDatabasePanelForm
WebsitesDatabasePanelForm
```

Both now inherit:

```csharp
DatabasePanelTemplate
```

## Why

A layout change to `DatabasePanelTemplate` now affects all database pages that derive from it.

This avoids copy/pasted designer layouts.
