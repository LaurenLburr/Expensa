# Generic Add-in UI Surface Resolver

MainForm no longer knows about specific add-in kinds such as Websites, Budgets, or Payees.

It now asks:

```csharp
AddinProjectUiSurfaceResolver
```

to create add-in UI surfaces by convention.

## Database convention

For:

```text
WebsitesAddin
```

the resolver looks for:

```text
CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites.WebsitesDatabasePanelForm
```

For:

```text
PayeesAddin
```

it will look for:

```text
CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Payees.PayeesDatabasePanelForm
```

## Test convention

For:

```text
BudgetsAddin
```

the resolver looks for:

```text
CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets.BudgetsTreeLoadVerificationFormCommonTree
```

## Why

This prevents MainForm from growing hard-coded branches:

```text
if Websites
if Budgets
if Payees
if Accounts
...
```

MainForm should know only that something is an add-in.
