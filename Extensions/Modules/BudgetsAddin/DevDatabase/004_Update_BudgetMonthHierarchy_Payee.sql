INSERT OR REPLACE INTO [SqlQuery]
(
    [QueryName],
    [Description],
    [SqlText],
    [Category],
    [IsActive]
)
VALUES
(
    'Budgets.SelectBudgetMonthHierarchy',
    'Loads the selected BudgetMonthRow records followed by associated transactions for the selected BudgetMonthId.',
    'WITH [SelectedBudgetRows] AS
(
    SELECT
        [BudgetMonthRow].[BudgetMonthRowId],
        [BudgetMonthRow].[BudgetMonthId],
        [BudgetMonth].[Year] AS [BudgetYear],
        [BudgetMonth].[Month] AS [BudgetMonthNumber],
        [BudgetMonthRow].[TemplateRowId],
        [BudgetMonthRow].[Name] AS [BudgetName],
        [BudgetMonthRow].[SortIndex],
        [BudgetMonthRow].[PlannedAmount],
        [Payee].[PayeeId],
        [Payee].[PayeeName] AS [Payee]
    FROM [BudgetMonthRow]
    INNER JOIN [BudgetMonth]
        ON [BudgetMonth].[BudgetMonthId] =
           [BudgetMonthRow].[BudgetMonthId]
    LEFT JOIN [Payee]
        ON [Payee].[PayeeName] =
           [BudgetMonthRow].[Name]
    WHERE [BudgetMonthRow].[BudgetMonthId] =
          @BudgetMonthId
),
[DisplayRows] AS
(
    SELECT
        [SelectedBudgetRows].[SortIndex]
            AS [BudgetSortIndex],
        [SelectedBudgetRows].[BudgetMonthRowId]
            AS [BudgetRowSortKey],
        0 AS [RowSortIndex],
        0 AS [ChildSortIndex],
        ''Budget'' AS [RowType],
        [SelectedBudgetRows].[BudgetName]
            AS [Item],
        [SelectedBudgetRows].[Payee],
        [SelectedBudgetRows].[PayeeId],
        NULL AS [Status],
        [SelectedBudgetRows].[PlannedAmount]
            AS [Amount],
        NULL AS [StartDate],
        NULL AS [Account],
        NULL AS [ConfirmationNumber],
        NULL AS [Note],
        [SelectedBudgetRows].[BudgetMonthRowId],
        NULL AS [TransactionId]
    FROM [SelectedBudgetRows]

    UNION ALL

    SELECT
        [SelectedBudgetRows].[SortIndex]
            AS [BudgetSortIndex],
        [SelectedBudgetRows].[BudgetMonthRowId]
            AS [BudgetRowSortKey],
        1 AS [RowSortIndex],
        COALESCE(
            [Txn].[StartDate],
            ''''
        ) || '':'' || [Txn].[TransactionId]
            AS [ChildSortIndex],
        ''Transaction'' AS [RowType],
        ''    ? '' ||
            COALESCE(
                [Payee].[PayeeName],
                [SelectedBudgetRows].[BudgetName]
            ) AS [Item],
        COALESCE(
            [Payee].[PayeeName],
            [SelectedBudgetRows].[Payee],
            [SelectedBudgetRows].[BudgetName]
        ) AS [Payee],
        COALESCE(
            [Txn].[PayeeId],
            [SelectedBudgetRows].[PayeeId]
        ) AS [PayeeId],
        [Txn].[Status],
        [Txn].[Amount],
        [Txn].[StartDate],
        [Account].[AccountNickname]
            AS [Account],
        [Txn].[ConfirmationNumber],
        [Txn].[Note],
        [SelectedBudgetRows].[BudgetMonthRowId],
        [Txn].[TransactionId]
    FROM [SelectedBudgetRows]
    INNER JOIN [Txn]
        ON [Txn].[PayeeId] =
           [SelectedBudgetRows].[PayeeId]
       AND CAST(
               STRFTIME(''%Y'', [Txn].[StartDate])
               AS INTEGER
           ) =
           [SelectedBudgetRows].[BudgetYear]
       AND CAST(
               STRFTIME(''%m'', [Txn].[StartDate])
               AS INTEGER
           ) =
           [SelectedBudgetRows].[BudgetMonthNumber]
    LEFT JOIN [Payee]
        ON [Payee].[PayeeId] =
           [Txn].[PayeeId]
    LEFT JOIN [Account]
        ON [Account].[AccountId] =
           [Txn].[AccountId]
)
SELECT
    [RowType],
    [Item],
    [Payee],
    [Status],
    [Amount],
    [StartDate],
    [Account],
    [ConfirmationNumber],
    [Note],
    [BudgetMonthRowId],
    [PayeeId],
    [TransactionId]
FROM [DisplayRows]
ORDER BY
    [BudgetSortIndex],
    [BudgetRowSortKey],
    [RowSortIndex],
    [ChildSortIndex]
LIMIT @MaximumRows;',
    'Budgets',
    1
);
