# Budgets template selection fix

## What changed

- Updated `BudgetsTreeLoadVerificationForm` to behave like a real `TreeTestTemplate` page instead of only showing selected node payload metadata.
- The form now binds the template grid through `ResultGrid` and `SafeDataGridViewBinding`.
- When a budget month/current/previous node is selected, the right-side template grid loads the matching `BudgetMonth` row from the add-in runtime database.
- When a year/root node is selected, the grid shows the child budget months under that node.
- The form uses the active Budgets runtime database pointer when present, falling back to the default Budgets runtime DB path.
- Added/updated structure tests covering template usage and selected-budget loading behavior.

## Database impact

Code-only change. No schema or data update is included.

## Files included

- `Host/CodexExpensa.ExtensionDevHost/CommandEngineIntegration/Budgets/BudgetsTreeLoadVerificationForm.cs`
- `Tests/CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests/Budgets/BudgetsTreeLoadVerificationFormTemplateTests.cs`

## Notes

No files under `CommandEngineIntegration/Templates` were changed.
