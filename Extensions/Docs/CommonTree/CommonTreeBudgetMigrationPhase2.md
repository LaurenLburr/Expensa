# CommonTree Budget Migration Phase 2

This slice migrates Budgets onto the shared CommonTree framework without removing the older Budgets loader yet.

## Added

```text
BudgetTreeProvider : AddinTreeProviderBase<BudgetTreePayload>
CommonBudgetTreeContributionLoader
BudgetsTreeLoadVerificationFormCommonTree
```

## Why this is separate

The older `BudgetsTreeLoadVerificationForm` still exists.

This lets the CommonTree path be tested before replacing the original Budgets-specific path.

## Next phase

After this builds and runs:

1. Point the Extension Manager Budgets Tree Load Test node at `BudgetsTreeLoadVerificationFormCommonTree`.
2. Delete or retire the old Budgets-specific renderer once the CommonTree version is stable.
3. Migrate Websites onto CommonTree.
