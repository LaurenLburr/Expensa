-- Seed one transaction in each transaction state for a BudgetMonthRow.
-- Run only against the Budgets add-in runtime database file, then use
-- "Reload add-in database into memory" from the Budgets database page.
--
-- Change TargetBudgetMonthId below when testing another month.

BEGIN TRANSACTION;

DELETE FROM [Txn]
WHERE [ConfirmationNumber] LIKE 'BUDGET-STATE-SEED-%';

WITH
[Settings]([TargetBudgetMonthId]) AS
(
    VALUES ('2026-06')
),
[TargetBudgetRow] AS
(
    SELECT
        [BudgetMonthRow].[BudgetMonthRowId],
        [BudgetMonthRow].[Name],
        [BudgetMonth].[Year] AS [BudgetYear],
        [BudgetMonth].[Month] AS [BudgetMonthNumber],
        [Payee].[PayeeId]
    FROM [BudgetMonthRow]
    INNER JOIN [BudgetMonth]
        ON [BudgetMonth].[BudgetMonthId] = [BudgetMonthRow].[BudgetMonthId]
    INNER JOIN [Payee]
        ON [Payee].[PayeeName] = [BudgetMonthRow].[Name]
    INNER JOIN [Settings]
        ON [Settings].[TargetBudgetMonthId] = [BudgetMonthRow].[BudgetMonthId]
    ORDER BY
        [BudgetMonthRow].[SortIndex],
        [BudgetMonthRow].[Name]
    LIMIT 1
),
[TargetAccount] AS
(
    SELECT [AccountId]
    FROM [Account]
    WHERE COALESCE([IsActive], 1) <> 0
    ORDER BY [SortIndex], [AccountNickname]
    LIMIT 1
),
[States]([StateName], [Amount], [DayNumber]) AS
(
    VALUES
        ('Projected',   101.01, 5),
        ('Outstanding', 202.02, 10),
        ('Cleared',     303.03, 15)
)
INSERT INTO [Txn]
(
    [AccountId],
    [PayeeId],
    [Status],
    [Amount],
    [StartDate],
    [ConfirmationNumber],
    [Note]
)
SELECT
    [TargetAccount].[AccountId],
    [TargetBudgetRow].[PayeeId],
    [States].[StateName],
    [States].[Amount],
    printf(
        '%04d-%02d-%02d',
        [TargetBudgetRow].[BudgetYear],
        [TargetBudgetRow].[BudgetMonthNumber],
        [States].[DayNumber]),
    'BUDGET-STATE-SEED-' || [States].[StateName],
    'Seed transaction for BudgetMonthRow ' || [TargetBudgetRow].[BudgetMonthRowId]
FROM [TargetBudgetRow]
CROSS JOIN [TargetAccount]
CROSS JOIN [States];

COMMIT;

-- Verification
SELECT
    [Txn].[TransactionId],
    [Txn].[Status],
    [Txn].[Amount],
    [Txn].[StartDate],
    [Payee].[PayeeName],
    [Txn].[ConfirmationNumber]
FROM [Txn]
LEFT JOIN [Payee]
    ON [Payee].[PayeeId] = [Txn].[PayeeId]
WHERE [Txn].[ConfirmationNumber] LIKE 'BUDGET-STATE-SEED-%'
ORDER BY [Txn].[TransactionId];
