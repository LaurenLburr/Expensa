# Payees Database Uses DatabasePanelTemplate

This changes the design from copy/paste layout to actual inheritance.

## Before

`PayeesDatabasePanelForm` copied the template controls into its own designer.

That meant future changes to:

```text
DatabasePanelTemplate
```

would not affect Payees.

## After

`PayeesDatabasePanelForm` now derives from:

```csharp
DatabasePanelTemplate
```

and only supplies Payees-specific data loading.

## Shared template behavior

`DatabasePanelTemplate` now exposes protected helpers:

```csharp
ConfigureDatabasePanel(...)
SetSummaryText(...)
SetGridDataSource(...)
SetRowStatus(...)
```

and virtual link handlers:

```csharp
OnDatabasePathLinkClicked()
OnUpdateFromProdClicked()
OnUpdateFromDevClicked()
```

## Result

A future layout change to `DatabasePanelTemplate` affects every database page that derives from it.

Payees now uses the template correctly.
