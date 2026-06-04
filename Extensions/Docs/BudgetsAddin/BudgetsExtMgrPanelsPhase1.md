# Budgets Extension Manager Panels Phase 1

Adds host-side Budgets plumbing:

```text
BudgetsDatabasePanelForm
BudgetsTreeLoadVerificationForm
HostBudgetRuntimeModuleInvoker
HostBudgetTreeContributionLoader
HostBudgetTreeViewRenderer
```

This slice does not patch `MainForm.cs`.

Manual MainForm hook still needed:

```csharp
ShowEmbeddedForm(new BudgetsDatabasePanelForm());
ShowEmbeddedForm(new BudgetsTreeLoadVerificationForm());
```
