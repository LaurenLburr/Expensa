# Budgets transaction hierarchy test support

## Purpose

This change makes the selected Budgets grid show each `BudgetMonthRow` as a parent row, followed immediately by its associated transactions as indented child rows.

## Association rule

The current schema has no direct foreign key from `Txn` to `BudgetMonthRow`. The display therefore associates rows using both of these rules:

1. `BudgetMonthRow.Name = Payee.PayeeName`
2. `Txn.StartDate` falls in the selected `BudgetMonth.Year` and `BudgetMonth.Month`

Transactions are still filtered by the selected `BudgetMonthId`; year and month are used only to associate transactions inside that selected budget.

## Seed data

Run:

`Extensions\Host\CodexExpensa.ExtensionDevHost\CommandEngineIntegration\Budgets\SeedBudgetTransactions.sql`

against the Budgets add-in runtime database file. The script:

- targets `2026-06` by default;
- chooses the first budget row whose name matches a payee;
- chooses the first active account;
- inserts one `Projected`, one `Outstanding`, and one `Cleared` transaction;
- removes only earlier rows whose confirmation number starts with `BUDGET-STATE-SEED-`.

After running it, use **Reload add-in database into memory** from the Budgets database page.

## Files changed

- `BudgetsTreeLoadVerificationForm.cs`
- `SeedBudgetTransactions.sql`
- `BudgetsTreeLoadVerificationFormTransactionHierarchyTests.cs`

## Database impact

Data change only when the seed SQL is executed. There is no schema change.

## Template policy

No files under `CommandEngineIntegration\Templates` were changed.
