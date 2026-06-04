# CommonTree Budgets MainForm Hook Phase 3

This slice switches the Extension Manager Budgets add-in navigation to the CommonTree path.

## MainForm changes

Adds:

```csharp
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;
```

Routes Budgets `Database` node to:

```csharp
ShowEmbeddedForm(new BudgetsDatabasePanelForm());
```

Routes Budgets `Test` node to:

```csharp
ShowEmbeddedForm(new BudgetsTreeLoadVerificationFormCommonTree());
```

Adds:

```csharp
IsBudgetsAddinProject(...)
```

## Not changed

This slice does not remove the older Budgets-specific tree verification form yet.

The older form remains available until the CommonTree Budgets path is stable.
