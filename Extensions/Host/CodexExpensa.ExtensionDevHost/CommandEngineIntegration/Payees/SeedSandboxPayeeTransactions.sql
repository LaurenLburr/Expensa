-- Seed 100 sandbox/runtime transactions for Payees transaction-grid testing.
-- Run this only against the add-in runtime/sandbox database copy, not production.
--
-- To target one specific payee, replace NULL in Settings(TargetPayeeId)
-- with that PayeeId, for example: VALUES ('legacy-payee-27')

WITH RECURSIVE
Settings(TargetPayeeId) AS
(
    VALUES (NULL)
),
Seq(n) AS
(
    SELECT 1
    UNION ALL
    SELECT n + 1
    FROM Seq
    WHERE n < 100
),
ActiveAccounts AS
(
    SELECT
        ROW_NUMBER() OVER (ORDER BY [AccountId]) AS rn,
        [AccountId]
    FROM [Account]
    WHERE COALESCE([IsActive], 1) <> 0
),
AccountCount AS
(
    SELECT COUNT(*) AS cnt
    FROM ActiveAccounts
),
ActivePayees AS
(
    SELECT
        ROW_NUMBER() OVER (ORDER BY [PayeeId]) AS rn,
        [PayeeId]
    FROM [Payee]
    WHERE COALESCE([IsActive], 1) <> 0
),
PayeeCount AS
(
    SELECT COUNT(*) AS cnt
    FROM ActivePayees
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
    a.[AccountId],
    COALESCE(s.[TargetPayeeId], p.[PayeeId]) AS [PayeeId],
    CASE ABS(RANDOM()) % 3
        WHEN 0 THEN 'Projected'
        WHEN 1 THEN 'Outstanding'
        ELSE 'Cleared'
    END AS [Status],
    ROUND(((ABS(RANDOM()) % 25000) / 100.0) + 5.00, 2) AS [Amount],
    DATE('now', '-' || (ABS(RANDOM()) % 120) || ' days') AS [StartDate],
    'SANDBOX-SEED-' || STRFTIME('%Y%m%d%H%M%S', 'now') || '-' || printf('%03d', seq.[n]) AS [ConfirmationNumber],
    'Sandbox generated transaction for Payees grid test #' || seq.[n] AS [Note]
FROM Seq seq
CROSS JOIN Settings s
CROSS JOIN AccountCount ac
CROSS JOIN PayeeCount pc
JOIN ActiveAccounts a
    ON a.[rn] = ((ABS(RANDOM()) % ac.[cnt]) + 1)
JOIN ActivePayees p
    ON p.[rn] = ((ABS(RANDOM()) % pc.[cnt]) + 1)
WHERE ac.[cnt] > 0
  AND pc.[cnt] > 0
  AND (s.[TargetPayeeId] IS NULL OR EXISTS
      (
          SELECT 1
          FROM [Payee] checkPayee
          WHERE checkPayee.[PayeeId] = s.[TargetPayeeId]
      ));
