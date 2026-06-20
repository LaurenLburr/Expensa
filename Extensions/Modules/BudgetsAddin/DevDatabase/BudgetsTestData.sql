-- ============================================================================
-- CodexExpensa Budgets deterministic test-data seed
--
-- Purpose:
--   Creates a repeatable Budgets Dev/test dataset in one SQL file.
--
-- Safety:
--   Deletes and replaces only rows owned by this script. All owned identifiers
--   begin with "test-budget-" or use the dedicated transaction note prefix.
--
-- Target schema:
--   Bank, Account, Payee, BudgetTemplateRow, BudgetMonth, BudgetMonthRow,
--   Txn, SqlQuery
--
-- Test months:
--   Current test month:  2026-06
--   Previous test month: 2026-05
--
-- Existing BudgetMonth rows for those Year/Month values are reused.
-- Their BudgetMonthId values are never replaced.
--
-- Transaction statuses:
--   Projected, Outstanding, Cleared
-- ============================================================================

PRAGMA foreign_keys = ON;

BEGIN IMMEDIATE TRANSACTION;

-- ============================================================================
-- 1. Remove the previous copy of this seed data
-- ============================================================================

DELETE FROM Txn
WHERE Note LIKE 'test-budget-seed:%'
   OR PayeeId LIKE 'test-budget-payee-%'
   OR AccountId = 'test-budget-account-checking';

DELETE FROM BudgetMonthPayee
WHERE PayeeId LIKE 'test-budget-payee-%';

DELETE FROM BudgetMonthRow
WHERE TemplateRowId LIKE 'test-budget-template-%'
   OR
   (
       BudgetMonthId IN
       (
           SELECT BudgetMonthId
           FROM BudgetMonth
           WHERE (Year = 2026 AND Month = 5)
              OR (Year = 2026 AND Month = 6)
       )
       AND
       (
           BudgetMonthRowId LIKE 'test-budget-%'
           OR BudgetMonthRowId LIKE 'test-month-row-%'
       )
   );

-- BudgetMonth headers are retained. Existing Year/Month rows are reused.

DELETE FROM BudgetTemplateRow
WHERE TemplateRowId LIKE 'test-budget-template-%';

DELETE FROM Payee
WHERE PayeeId LIKE 'test-budget-payee-%';

DELETE FROM Account
WHERE AccountId = 'test-budget-account-checking';

DELETE FROM Bank
WHERE BankId = 'test-budget-bank';

DELETE FROM SqlQuery
WHERE QueryName LIKE 'TestData.Budgets.%';


-- ============================================================================
-- 2. Dedicated bank and account
-- ============================================================================

INSERT INTO Bank
(
    BankId,
    BankName,
    RoutingNumber,
    Url,
    IsActive
)
VALUES
(
    'test-budget-bank',
    'Budget Test Bank',
    '000000000',
    NULL,
    1
);

INSERT INTO Account
(
    AccountId,
    BankId,
    AccountNickname,
    SortIndex,
    AccountNumber,
    AccountType,
    IsActive
)
VALUES
(
    'test-budget-account-checking',
    'test-budget-bank',
    'Budget Test Checking',
    9000,
    'TEST-0001',
    'Checking',
    1
);


-- ============================================================================
-- 3. Dedicated payees
-- ============================================================================

INSERT INTO Payee
(
    PayeeId,
    PayeeName,
    IncludeInBudgetTemplate,
    SortIndex,
    IsActive,
    WebsiteId,
    DefaultAccountId,
    DefaultAmount
)
VALUES
(
    'test-budget-payee-rent',
    'TEST - Rent',
    1,
    9001,
    1,
    NULL,
    'test-budget-account-checking',
    1200.00
);

INSERT INTO Payee
(
    PayeeId,
    PayeeName,
    IncludeInBudgetTemplate,
    SortIndex,
    IsActive,
    WebsiteId,
    DefaultAccountId,
    DefaultAmount
)
VALUES
(
    'test-budget-payee-power',
    'TEST - Power',
    1,
    9002,
    1,
    NULL,
    'test-budget-account-checking',
    180.00
);

INSERT INTO Payee
(
    PayeeId,
    PayeeName,
    IncludeInBudgetTemplate,
    SortIndex,
    IsActive,
    WebsiteId,
    DefaultAccountId,
    DefaultAmount
)
VALUES
(
    'test-budget-payee-internet',
    'TEST - Internet',
    1,
    9003,
    1,
    NULL,
    'test-budget-account-checking',
    95.00
);

INSERT INTO Payee
(
    PayeeId,
    PayeeName,
    IncludeInBudgetTemplate,
    SortIndex,
    IsActive,
    WebsiteId,
    DefaultAccountId,
    DefaultAmount
)
VALUES
(
    'test-budget-payee-groceries',
    'TEST - Groceries',
    1,
    9004,
    1,
    NULL,
    'test-budget-account-checking',
    500.00
);

INSERT INTO Payee
(
    PayeeId,
    PayeeName,
    IncludeInBudgetTemplate,
    SortIndex,
    IsActive,
    WebsiteId,
    DefaultAccountId,
    DefaultAmount
)
VALUES
(
    'test-budget-payee-empty',
    'TEST - No Transactions',
    1,
    9005,
    1,
    NULL,
    'test-budget-account-checking',
    75.00
);

INSERT OR IGNORE INTO Payee
(
    PayeeId,
    PayeeName,
    IncludeInBudgetTemplate,
    SortIndex,
    IsActive,
    WebsiteId,
    DefaultAccountId,
    DefaultAmount
)
VALUES
    ('budget-row-payee-mortgage', 'Mortgage', 0, 10, 1, NULL, NULL, 1850.00),
    ('budget-row-payee-electricity', 'Electricity', 0, 20, 1, NULL, NULL, 172.50),
    ('budget-row-payee-internet', 'Internet', 0, 30, 1, NULL, NULL, 95.00),
    ('budget-row-payee-groceries', 'Groceries', 0, 40, 1, NULL, NULL, 650.00),
    ('budget-row-payee-auto-expenses', 'Auto Expenses', 0, 50, 1, NULL, NULL, 275.00),
    ('budget-row-payee-entertainment', 'Entertainment', 0, 60, 1, NULL, NULL, 125.00);


-- ============================================================================
-- 4. Template rows
--
-- The current schema has BudgetTemplateRow but no separate BudgetTemplate
-- header table. These rows therefore form the deterministic test template.
-- ============================================================================

INSERT INTO BudgetTemplateRow
(
    TemplateRowId,
    Name,
    SortIndex,
    DefaultAmount,
    IsActive
)
VALUES
(
    'test-budget-template-rent',
    'TEST - Rent',
    10,
    1200.00,
    1
);

INSERT INTO BudgetTemplateRow
(
    TemplateRowId,
    Name,
    SortIndex,
    DefaultAmount,
    IsActive
)
VALUES
(
    'test-budget-template-power',
    'TEST - Power',
    20,
    180.00,
    1
);

INSERT INTO BudgetTemplateRow
(
    TemplateRowId,
    Name,
    SortIndex,
    DefaultAmount,
    IsActive
)
VALUES
(
    'test-budget-template-internet',
    'TEST - Internet',
    30,
    95.00,
    1
);

INSERT INTO BudgetTemplateRow
(
    TemplateRowId,
    Name,
    SortIndex,
    DefaultAmount,
    IsActive
)
VALUES
(
    'test-budget-template-groceries',
    'TEST - Groceries',
    40,
    500.00,
    1
);

INSERT INTO BudgetTemplateRow
(
    TemplateRowId,
    Name,
    SortIndex,
    DefaultAmount,
    IsActive
)
VALUES
(
    'test-budget-template-empty',
    'TEST - No Transactions',
    50,
    75.00,
    1
);


-- ============================================================================
-- 5. Current and previous BudgetMonth records
-- ============================================================================

INSERT INTO BudgetMonth
(
    BudgetMonthId,
    Year,
    Month,
    CreatedUtc
)
VALUES
(
    'test-budget-2026-05',
    2026,
    5,
    CURRENT_TIMESTAMP
)
ON CONFLICT(Year, Month) DO NOTHING;

INSERT INTO BudgetMonth
(
    BudgetMonthId,
    Year,
    Month,
    CreatedUtc
)
VALUES
(
    'test-budget-2026-06',
    2026,
    6,
    CURRENT_TIMESTAMP
)
ON CONFLICT(Year, Month) DO NOTHING;


-- ============================================================================
-- 6. Previous-month rows
-- ============================================================================

INSERT INTO BudgetMonthRow
(
    BudgetMonthRowId,
    BudgetMonthId,
    TemplateRowId,
    Name,
    SortIndex,
    PlannedAmount
)
VALUES
(
    'test-budget-2026-05-rent',
    (SELECT BudgetMonthId FROM BudgetMonth WHERE Year = 2026 AND Month = 5),
    'test-budget-template-rent',
    'TEST - Rent',
    10,
    1200.00
);

INSERT INTO BudgetMonthRow
(
    BudgetMonthRowId,
    BudgetMonthId,
    TemplateRowId,
    Name,
    SortIndex,
    PlannedAmount
)
VALUES
(
    'test-budget-2026-05-power',
    (SELECT BudgetMonthId FROM BudgetMonth WHERE Year = 2026 AND Month = 5),
    'test-budget-template-power',
    'TEST - Power',
    20,
    165.00
);

INSERT INTO BudgetMonthRow
(
    BudgetMonthRowId,
    BudgetMonthId,
    TemplateRowId,
    Name,
    SortIndex,
    PlannedAmount
)
VALUES
(
    'test-budget-2026-05-internet',
    (SELECT BudgetMonthId FROM BudgetMonth WHERE Year = 2026 AND Month = 5),
    'test-budget-template-internet',
    'TEST - Internet',
    30,
    95.00
);

INSERT INTO BudgetMonthRow
(
    BudgetMonthRowId,
    BudgetMonthId,
    TemplateRowId,
    Name,
    SortIndex,
    PlannedAmount
)
VALUES
(
    'test-budget-2026-05-groceries',
    (SELECT BudgetMonthId FROM BudgetMonth WHERE Year = 2026 AND Month = 5),
    'test-budget-template-groceries',
    'TEST - Groceries',
    40,
    475.00
);

INSERT INTO BudgetMonthRow
(
    BudgetMonthRowId,
    BudgetMonthId,
    TemplateRowId,
    Name,
    SortIndex,
    PlannedAmount
)
VALUES
(
    'test-budget-2026-05-empty',
    (SELECT BudgetMonthId FROM BudgetMonth WHERE Year = 2026 AND Month = 5),
    'test-budget-template-empty',
    'TEST - No Transactions',
    50,
    75.00
);


-- ============================================================================
-- 7. Current-month rows
-- ============================================================================

INSERT INTO BudgetMonthRow
(
    BudgetMonthRowId,
    BudgetMonthId,
    TemplateRowId,
    Name,
    SortIndex,
    PlannedAmount
)
VALUES
(
    'test-budget-2026-06-rent',
    (SELECT BudgetMonthId FROM BudgetMonth WHERE Year = 2026 AND Month = 6),
    'test-budget-template-rent',
    'TEST - Rent',
    10,
    1200.00
);

INSERT INTO BudgetMonthRow
(
    BudgetMonthRowId,
    BudgetMonthId,
    TemplateRowId,
    Name,
    SortIndex,
    PlannedAmount
)
VALUES
(
    'test-budget-2026-06-power',
    (SELECT BudgetMonthId FROM BudgetMonth WHERE Year = 2026 AND Month = 6),
    'test-budget-template-power',
    'TEST - Power',
    20,
    180.00
);

INSERT INTO BudgetMonthRow
(
    BudgetMonthRowId,
    BudgetMonthId,
    TemplateRowId,
    Name,
    SortIndex,
    PlannedAmount
)
VALUES
(
    'test-budget-2026-06-internet',
    (SELECT BudgetMonthId FROM BudgetMonth WHERE Year = 2026 AND Month = 6),
    'test-budget-template-internet',
    'TEST - Internet',
    30,
    95.00
);

INSERT INTO BudgetMonthRow
(
    BudgetMonthRowId,
    BudgetMonthId,
    TemplateRowId,
    Name,
    SortIndex,
    PlannedAmount
)
VALUES
(
    'test-budget-2026-06-groceries',
    (SELECT BudgetMonthId FROM BudgetMonth WHERE Year = 2026 AND Month = 6),
    'test-budget-template-groceries',
    'TEST - Groceries',
    40,
    500.00
);

INSERT INTO BudgetMonthRow
(
    BudgetMonthRowId,
    BudgetMonthId,
    TemplateRowId,
    Name,
    SortIndex,
    PlannedAmount
)
VALUES
(
    'test-budget-2026-06-empty',
    (SELECT BudgetMonthId FROM BudgetMonth WHERE Year = 2026 AND Month = 6),
    'test-budget-template-empty',
    'TEST - No Transactions',
    50,
    75.00
);


-- ============================================================================
-- 8. Current and previous BudgetMonthPayee rows
-- ============================================================================

INSERT INTO BudgetMonthPayee
(
    BudgetMonthPayeeId,
    BudgetMonthId,
    PayeeId,
    SortIndex,
    PlannedAmount,
    AccountId
)
SELECT
    'test-budget-month-payee-' || bm.Year || '-' || printf('%02d', bm.Month) || '-' || p.PayeeId,
    bm.BudgetMonthId,
    p.PayeeId,
    p.SortIndex,
    p.DefaultAmount,
    p.DefaultAccountId
FROM BudgetMonth bm
CROSS JOIN Payee p
WHERE ((bm.Year = 2026 AND bm.Month = 5)
    OR (bm.Year = 2026 AND bm.Month = 6))
  AND p.PayeeId LIKE 'test-budget-payee-%';


-- ============================================================================
-- 9. Transactions with every supported status
--
-- Txn has no BudgetMonthRowId foreign key in the current schema. Transactions
-- are associated with the test budget data through:
--   - the dedicated test payee
--   - the dedicated test account
--   - StartDate within the corresponding budget month
-- ============================================================================

-- Previous month: all cleared, providing historical comparison.
INSERT INTO Txn
(
    AccountId,
    PayeeId,
    Status,
    Amount,
    StartDate,
    ConfirmationNumber,
    Note
)
VALUES
(
    'test-budget-account-checking',
    'test-budget-payee-rent',
    'Cleared',
    1200.00,
    '2026-05-01',
    'TEST-MAY-RENT',
    'test-budget-seed: previous month cleared rent'
);

INSERT INTO Txn
(
    AccountId,
    PayeeId,
    Status,
    Amount,
    StartDate,
    ConfirmationNumber,
    Note
)
VALUES
(
    'test-budget-account-checking',
    'test-budget-payee-power',
    'Cleared',
    158.25,
    '2026-05-10',
    'TEST-MAY-POWER',
    'test-budget-seed: previous month cleared power'
);

INSERT INTO Txn
(
    AccountId,
    PayeeId,
    Status,
    Amount,
    StartDate,
    ConfirmationNumber,
    Note
)
VALUES
(
    'test-budget-account-checking',
    'test-budget-payee-internet',
    'Cleared',
    95.00,
    '2026-05-15',
    'TEST-MAY-INTERNET',
    'test-budget-seed: previous month cleared internet'
);

INSERT INTO Txn
(
    AccountId,
    PayeeId,
    Status,
    Amount,
    StartDate,
    ConfirmationNumber,
    Note
)
VALUES
(
    'test-budget-account-checking',
    'test-budget-payee-groceries',
    'Cleared',
    452.37,
    '2026-05-24',
    'TEST-MAY-GROCERIES',
    'test-budget-seed: previous month cleared groceries'
);

-- Current month: at least one transaction in every status.
INSERT INTO Txn
(
    AccountId,
    PayeeId,
    Status,
    Amount,
    StartDate,
    ConfirmationNumber,
    Note
)
VALUES
(
    'test-budget-account-checking',
    'test-budget-payee-rent',
    'Cleared',
    1200.00,
    '2026-06-01',
    'TEST-JUN-RENT',
    'test-budget-seed: current month cleared rent'
);

INSERT INTO Txn
(
    AccountId,
    PayeeId,
    Status,
    Amount,
    StartDate,
    ConfirmationNumber,
    Note
)
VALUES
(
    'test-budget-account-checking',
    'test-budget-payee-power',
    'Outstanding',
    172.50,
    '2026-06-10',
    NULL,
    'test-budget-seed: current month outstanding power'
);

INSERT INTO Txn
(
    AccountId,
    PayeeId,
    Status,
    Amount,
    StartDate,
    ConfirmationNumber,
    Note
)
VALUES
(
    'test-budget-account-checking',
    'test-budget-payee-internet',
    'Projected',
    95.00,
    '2026-06-15',
    NULL,
    'test-budget-seed: current month projected internet'
);

-- Two grocery transactions exercise multiple transactions for one budget row.
INSERT INTO Txn
(
    AccountId,
    PayeeId,
    Status,
    Amount,
    StartDate,
    ConfirmationNumber,
    Note
)
VALUES
(
    'test-budget-account-checking',
    'test-budget-payee-groceries',
    'Cleared',
    126.42,
    '2026-06-05',
    'TEST-JUN-GROCERY-1',
    'test-budget-seed: current month cleared groceries'
);

INSERT INTO Txn
(
    AccountId,
    PayeeId,
    Status,
    Amount,
    StartDate,
    ConfirmationNumber,
    Note
)
VALUES
(
    'test-budget-account-checking',
    'test-budget-payee-groceries',
    'Outstanding',
    83.17,
    '2026-06-12',
    NULL,
    'test-budget-seed: current month outstanding groceries'
);


-- ============================================================================
-- 9. SQL catalog entries
-- ============================================================================

INSERT INTO SqlQuery
(
    QueryName,
    Description,
    SqlText,
    CreatedUtc,
    Category,
    IsActive
)
VALUES
(
    'TestData.Budgets.SelectMonthRows',
    'Returns deterministic test BudgetMonthRow records for one BudgetMonthId.',
    'SELECT
    [BudgetMonthRowId],
    [BudgetMonthId],
    [TemplateRowId],
    [Name],
    [SortIndex],
    [PlannedAmount]
FROM [BudgetMonthRow]
WHERE [BudgetMonthId] = @BudgetMonthId
ORDER BY [SortIndex], [BudgetMonthRowId];',
    CURRENT_TIMESTAMP,
    'TestData',
    1
)
ON CONFLICT(QueryName) DO UPDATE SET
    Description = excluded.Description,
    SqlText = excluded.SqlText,
    Category = excluded.Category,
    IsActive = excluded.IsActive;

INSERT INTO SqlQuery
(
    QueryName,
    Description,
    SqlText,
    CreatedUtc,
    Category,
    IsActive
)
VALUES
(
    'TestData.Budgets.SelectTransactionsByMonth',
    'Returns deterministic test transactions for a requested year and month.',
    'SELECT
    t.[TransactionId],
    t.[AccountId],
    t.[PayeeId],
    p.[PayeeName],
    t.[Status],
    t.[Amount],
    t.[StartDate],
    t.[ConfirmationNumber],
    t.[Note]
FROM [Txn] t
LEFT JOIN [Payee] p
    ON p.[PayeeId] = t.[PayeeId]
WHERE t.[AccountId] = ''test-budget-account-checking''
  AND CAST(strftime(''%Y'', t.[StartDate]) AS INTEGER) = @Year
  AND CAST(strftime(''%m'', t.[StartDate]) AS INTEGER) = @Month
ORDER BY t.[StartDate], t.[TransactionId];',
    CURRENT_TIMESTAMP,
    'TestData',
    1
)
ON CONFLICT(QueryName) DO UPDATE SET
    Description = excluded.Description,
    SqlText = excluded.SqlText,
    Category = excluded.Category,
    IsActive = excluded.IsActive;

INSERT INTO SqlQuery
(
    QueryName,
    Description,
    SqlText,
    CreatedUtc,
    Category,
    IsActive
)
VALUES
(
    'TestData.Budgets.StatusTotals',
    'Returns current test-month transaction totals grouped by status.',
    'SELECT
    [Status],
    COUNT(*) AS [TransactionCount],
    ROUND(SUM([Amount]), 2) AS [TotalAmount]
FROM [Txn]
WHERE [AccountId] = ''test-budget-account-checking''
  AND [StartDate] >= ''2026-06-01''
  AND [StartDate] <  ''2026-07-01''
GROUP BY [Status]
ORDER BY
    CASE [Status]
        WHEN ''Projected''   THEN 1
        WHEN ''Outstanding'' THEN 2
        WHEN ''Cleared''     THEN 3
        ELSE 4
    END;',
    CURRENT_TIMESTAMP,
    'TestData',
    1
)
ON CONFLICT(QueryName) DO UPDATE SET
    Description = excluded.Description,
    SqlText = excluded.SqlText,
    Category = excluded.Category,
    IsActive = excluded.IsActive;

INSERT INTO SqlQuery
(
    QueryName,
    Description,
    SqlText,
    CreatedUtc,
    Category,
    IsActive
)
VALUES
(
    'TestData.Budgets.MonthRowsWithTransactionTotals',
    'Joins deterministic budget rows to transaction totals by test payee name and month.',
    'SELECT
    bmr.[BudgetMonthRowId],
    bmr.[BudgetMonthId],
    bmr.[TemplateRowId],
    bmr.[Name],
    bmr.[SortIndex],
    bmr.[PlannedAmount],
    COALESCE(SUM(CASE WHEN t.[Status] = ''Projected'' THEN t.[Amount] ELSE 0 END), 0) AS [ProjectedAmount],
    COALESCE(SUM(CASE WHEN t.[Status] = ''Outstanding'' THEN t.[Amount] ELSE 0 END), 0) AS [OutstandingAmount],
    COALESCE(SUM(CASE WHEN t.[Status] = ''Cleared'' THEN t.[Amount] ELSE 0 END), 0) AS [ClearedAmount],
    COUNT(t.[TransactionId]) AS [TransactionCount]
FROM [BudgetMonthRow] bmr
INNER JOIN [BudgetMonth] bm
    ON bm.[BudgetMonthId] = bmr.[BudgetMonthId]
LEFT JOIN [Payee] p
    ON p.[PayeeName] = bmr.[Name]
LEFT JOIN [Txn] t
    ON t.[PayeeId] = p.[PayeeId]
   AND CAST(strftime(''%Y'', t.[StartDate]) AS INTEGER) = bm.[Year]
   AND CAST(strftime(''%m'', t.[StartDate]) AS INTEGER) = bm.[Month]
WHERE bmr.[BudgetMonthId] = @BudgetMonthId
GROUP BY
    bmr.[BudgetMonthRowId],
    bmr.[BudgetMonthId],
    bmr.[TemplateRowId],
    bmr.[Name],
    bmr.[SortIndex],
    bmr.[PlannedAmount]
ORDER BY bmr.[SortIndex], bmr.[BudgetMonthRowId];',
    CURRENT_TIMESTAMP,
    'TestData',
    1
)
ON CONFLICT(QueryName) DO UPDATE SET
    Description = excluded.Description,
    SqlText = excluded.SqlText,
    Category = excluded.Category,
    IsActive = excluded.IsActive;


COMMIT;


-- ============================================================================
-- 10. Verification output
--
-- Expected current-month status totals:
--   Projected:    1 transaction,  95.00
--   Outstanding:  2 transactions, 255.67
--   Cleared:      2 transactions, 1326.42
--
-- Expected current-month BudgetMonthRow count: 5
-- Expected previous-month BudgetMonthRow count: 5
-- Expected no-transaction row count in June: 1
-- ============================================================================

SELECT
    'BudgetMonth rows' AS Verification,
    COUNT(*) AS Actual,
    2 AS Expected
FROM BudgetMonth
WHERE (Year = 2026 AND Month = 5)
   OR (Year = 2026 AND Month = 6);

SELECT
    'Current BudgetMonthRow rows' AS Verification,
    COUNT(*) AS Actual,
    5 AS Expected
FROM BudgetMonthRow
WHERE BudgetMonthId =
(
    SELECT BudgetMonthId
    FROM BudgetMonth
    WHERE Year = 2026
      AND Month = 6
);

SELECT
    'Previous BudgetMonthRow rows' AS Verification,
    COUNT(*) AS Actual,
    5 AS Expected
FROM BudgetMonthRow
WHERE BudgetMonthId =
(
    SELECT BudgetMonthId
    FROM BudgetMonth
    WHERE Year = 2026
      AND Month = 5
);

SELECT
    'Current BudgetMonthPayee rows' AS Verification,
    COUNT(*) AS Actual,
    5 AS Expected
FROM BudgetMonthPayee
WHERE BudgetMonthId =
(
    SELECT BudgetMonthId
    FROM BudgetMonth
    WHERE Year = 2026
      AND Month = 6
)
AND PayeeId LIKE 'test-budget-payee-%';

SELECT
    'BudgetMonthRows missing Payee' AS Verification,
    COUNT(*) AS Actual,
    0 AS Expected
FROM BudgetMonthRow bmr
LEFT JOIN Payee p
    ON p.PayeeName = bmr.Name
WHERE bmr.BudgetMonthId IN
(
    SELECT BudgetMonthId
    FROM BudgetMonth
    WHERE (Year = 2026 AND Month = 5)
       OR (Year = 2026 AND Month = 6)
)
AND p.PayeeId IS NULL;

SELECT
    Status,
    COUNT(*) AS TransactionCount,
    ROUND(SUM(Amount), 2) AS TotalAmount
FROM Txn
WHERE Note LIKE 'test-budget-seed: current month%'
GROUP BY Status
ORDER BY
    CASE Status
        WHEN 'Projected'   THEN 1
        WHEN 'Outstanding' THEN 2
        WHEN 'Cleared'     THEN 3
        ELSE 4
    END;

SELECT
    bmr.BudgetMonthRowId,
    bmr.Name,
    bmr.PlannedAmount,
    COUNT(t.TransactionId) AS TransactionCount
FROM BudgetMonthRow bmr
INNER JOIN BudgetMonth bm
    ON bm.BudgetMonthId = bmr.BudgetMonthId
LEFT JOIN Payee p
    ON p.PayeeName = bmr.Name
LEFT JOIN Txn t
    ON t.PayeeId = p.PayeeId
   AND CAST(strftime('%Y', t.StartDate) AS INTEGER) = bm.Year
   AND CAST(strftime('%m', t.StartDate) AS INTEGER) = bm.Month
WHERE bmr.BudgetMonthId =
(
    SELECT BudgetMonthId
    FROM BudgetMonth
    WHERE Year = 2026
      AND Month = 6
)
GROUP BY
    bmr.BudgetMonthRowId,
    bmr.Name,
    bmr.PlannedAmount
ORDER BY bmr.SortIndex;
