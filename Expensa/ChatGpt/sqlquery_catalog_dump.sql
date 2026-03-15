-- SqlQuery Catalog Dump
-- Generated 2026-03-15 02:45:57Z

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Account.Insert',
    'Account',
    'Inserts a new Account row into the current schema.',
    'INSERT INTO [Account]\n(\n    [AccountId],\n    [BankId],\n    [AccountNickname],\n    [SortIndex],\n    [AccountNumber],\n    [AccountType],\n    [IsActive]\n)\nVALUES\n(\n    @AccountId,\n    @BankId,\n    @AccountNickname,\n    @SortIndex,\n    @AccountNumber,\n    @AccountType,\n    @IsActive\n);',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Account.SelectByBankId',
    'Account',
    'Returns all Account rows for a specific BankId ordered by SortIndex and AccountNickname for display in account lists.',
    'SELECT\n    [AccountId],\n    [BankId],\n    [AccountNickname],\n    [SortIndex],\n    [AccountNumber],\n    [AccountType],\n    [IsActive]\nFROM [Account]\nWHERE [BankId] = @BankId\nORDER BY\n    [SortIndex],\n    [AccountNickname];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Account.SelectById',
    'Account',
    'Returns a single Account row by AccountId for editing or display.',
    'SELECT\n    [AccountId],\n    [BankId],\n    [AccountNickname],\n    [SortIndex],\n    [AccountNumber],\n    [AccountType],\n    [IsActive]\nFROM [Account]\nWHERE [AccountId] = @AccountId;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Account.Update',
    'Account',
    'Updates an existing Account row identified by AccountId.',
    'UPDATE [Account]\nSET\n    [BankId] = @BankId,\n    [AccountNickname] = @AccountNickname,\n    [SortIndex] = @SortIndex,\n    [AccountNumber] = @AccountNumber,\n    [AccountType] = @AccountType,\n    [IsActive] = @IsActive\nWHERE [AccountId] = @AccountId;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'AccountsByBank',
    'Account',
    'Returns all Account rows for a given BankId ordered by SortIndex. Used by AccountTransactionsPanel.',
    'SELECT *\nFROM Account\nWHERE BankId = @BankId\nORDER BY SortIndex',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Bank.Insert',
    'Bank',
    'Inserts a new Bank row into the current schema.',
    'INSERT INTO [Bank]\n(\n    [BankId],\n    [BankName],\n    [RoutingNumber],\n    [Url],\n    [IsActive]\n)\nVALUES\n(\n    @BankId,\n    @BankName,\n    @RoutingNumber,\n    @Url,\n    @IsActive\n);',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Bank.SelectById',
    'Bank',
    'Returns a single Bank row by BankId.',
    'SELECT\n    [BankId],\n    [BankName],\n    [RoutingNumber],\n    [Url],\n    [IsActive]\nFROM [Bank]\nWHERE [BankId] = @BankId;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Bank.Update',
    'Bank',
    'Updates an existing Bank row identified by BankId.',
    'UPDATE [Bank]\nSET\n    [BankName] = @BankName,\n    [RoutingNumber] = @RoutingNumber,\n    [Url] = @Url,\n    [IsActive] = @IsActive\nWHERE [BankId] = @BankId;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Banks.SelectAll',
    'Bank',
    'Returns all Bank rows ordered by BankName for navigation lists and selection screens.',
    'SELECT\n    [BankId],\n    [BankName],\n    [RoutingNumber],\n    [Url],\n    [IsActive]\nFROM [Bank]\nORDER BY\n    [BankName];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonth.CopyTemplatePayees',
    'Budget',
    'Copies payees that are flagged for inclusion in the budget template into BudgetMonthPayee for a specific budget month. Used when creating a new budget so the new month starts with the standard set of budget items.',
    'INSERT INTO [BudgetMonthPayee]\n(\n    [BudgetMonthPayeeId],\n    [BudgetMonthId],\n    [PayeeId],\n    [SortIndex],\n    [PlannedAmount],\n    [AccountId]\n)\nSELECT\n    @BudgetMonthId || ''-'' || p.[PayeeId],\n    @BudgetMonthId,\n    p.[PayeeId],\n    p.[SortIndex],\n    COALESCE(p.[DefaultAmount],0),\n    p.[DefaultAccountId]\nFROM [Payee] p\nWHERE p.[IncludeInBudgetTemplate] = 1;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonth.Create',
    'Budget',
    'Creates a new BudgetMonth row for a specific BudgetMonthId, Year, and Month.',
    'INSERT INTO [BudgetMonth]\n(\n    [BudgetMonthId],\n    [Year],\n    [Month],\n    [CreatedUtc]\n)\nVALUES\n(\n    @BudgetMonthId,\n    @Year,\n    @Month,\n    datetime(''now'')\n);',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonth.ExistsByYearMonth',
    'Budget',
    'Checks whether a BudgetMonth already exists for a given Year and Month and returns its BudgetMonthId if present.',
    'SELECT\n    [BudgetMonthId]\nFROM [BudgetMonth]\nWHERE [Year] = @Year\nAND [Month] = @Month;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonth.MissingMonthsForYear',
    'Budget',
    'Returns months within a specific year that do not yet have a BudgetMonth\nrecord. Used by the budget year view to show which months are still\navailable for budget creation.',
    'SELECT\n    m.MonthNumber,\n    CASE m.MonthNumber\n        WHEN 1 THEN ''January''\n        WHEN 2 THEN ''February''\n        WHEN 3 THEN ''March''\n        WHEN 4 THEN ''April''\n        WHEN 5 THEN ''May''\n        WHEN 6 THEN ''June''\n        WHEN 7 THEN ''July''\n        WHEN 8 THEN ''August''\n        WHEN 9 THEN ''September''\n        WHEN 10 THEN ''October''\n        WHEN 11 THEN ''November''\n        WHEN 12 THEN ''December''\n    END AS MonthName,\n    printf(''%04d-%02d'', @Year, m.MonthNumber) AS BudgetMonthId\nFROM\n(\n    SELECT 1 AS MonthNumber UNION ALL\n    SELECT 2 UNION ALL\n    SELECT 3 UNION ALL\n    SELECT 4 UNION ALL\n    SELECT 5 UNION ALL\n    SELECT 6 UNION ALL\n    SELECT 7 UNION ALL\n    SELECT 8 UNION ALL\n    SELECT 9 UNION ALL\n    SELECT 10 UNION ALL\n    SELECT 11 UNION ALL\n    SELECT 12\n) m\nLEFT JOIN BudgetMonth bm\n    ON bm.Year = @Year\n   AND bm.Month = m.MonthNumber\nWHERE bm.BudgetMonthId IS NULL\nORDER BY m.MonthNumber;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonth.SelectAll',
    'Budget',
    'Returns all BudgetMonth rows ordered by Year and Month.',
    'SELECT\n    [BudgetMonthId],\n    [Year],\n    [Month],\n    [CreatedUtc]\nFROM [BudgetMonth]\nORDER BY\n    [Year],\n    [Month];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonth.SelectById',
    'Budget',
    'Returns a single BudgetMonth row by BudgetMonthId.',
    'SELECT\n    [BudgetMonthId],\n    [Year],\n    [Month],\n    [CreatedUtc]\nFROM [BudgetMonth]\nWHERE [BudgetMonthId] = @BudgetMonthId;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonth.SelectByYearMonth',
    'Budget',
    'Returns the BudgetMonth row for a given Year and Month. Used to resolve BudgetMonthId from a selected year and month.',
    'SELECT\n    [BudgetMonthId],\n    [Year],\n    [Month],\n    [CreatedUtc]\nFROM [BudgetMonth]\nWHERE [Year] = @Year\nAND [Month] = @Month;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonthPayee.CreateFromTemplate',
    'Budget',
    'Creates BudgetMonthPayee rows for a new budget month using payees marked for inclusion in the budget template. Copies SortIndex, DefaultAmount, and DefaultAccountId from Payee into the new month-specific budget rows.',
    'INSERT INTO [BudgetMonthPayee]\n(\n    [BudgetMonthPayeeId],\n    [BudgetMonthId],\n    [PayeeId],\n    [SortIndex],\n    [PlannedAmount],\n    [AccountId]\n)\nSELECT\n    @BudgetMonthId || ''-'' || p.[PayeeId],\n    @BudgetMonthId,\n    p.[PayeeId],\n    p.[SortIndex],\n    COALESCE(p.[DefaultAmount], 0),\n    p.[DefaultAccountId]\nFROM [Payee] p\nWHERE p.[IncludeInBudgetTemplate] = 1\nAND NOT EXISTS\n(\n    SELECT 1\n    FROM [BudgetMonthPayee] bmp\n    WHERE bmp.[BudgetMonthId] = @BudgetMonthId\n      AND bmp.[PayeeId] = p.[PayeeId]\n);',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonthPayee.SelectByBudgetMonthId',
    'Budget',
    'Returns the full catalog of budget items for a specific BudgetMonthId including PayeeName for display in the monthly budget grid.',
    'SELECT\n    bmp.[BudgetMonthPayeeId],\n    bmp.[BudgetMonthId],\n    bmp.[PayeeId],\n    p.[PayeeName],\n    bmp.[SortIndex],\n    bmp.[PlannedAmount],\n    bmp.[AccountId]\nFROM [BudgetMonthPayee] bmp\nINNER JOIN [Payee] p\n    ON p.[PayeeId] = bmp.[PayeeId]\nWHERE bmp.[BudgetMonthId] = @BudgetMonthId\nORDER BY\n    bmp.[SortIndex],\n    p.[PayeeName];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonthPayee.UpdatePlannedAmount',
    'Budget',
    'Updates PlannedAmount for a single BudgetMonthPayee row identified by BudgetMonthPayeeId.',
    'UPDATE [BudgetMonthPayee]\nSET [PlannedAmount] = @PlannedAmount\nWHERE [BudgetMonthPayeeId] = @BudgetMonthPayeeId;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetTemplate.SelectRows',
    'Budget',
    'Returns the payees included in the budget template view ordered by SortIndex and PayeeName.',
    'SELECT\n    [PayeeId],\n    [PayeeName],\n    [SortIndex]\nFROM [vw_BudgetTemplatePayees]\nORDER BY\n    [SortIndex],\n    [PayeeName];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'TemplatePayees',
    'Budget',
    'Legacy template-payee query returning payees selected for the budget template. Superseded by BudgetTemplate.SelectRows.',
    'SELECT PayeeId, PayeeName, SortIndex\nFROM vw_BudgetTemplatePayees',
    0
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonth.DebugRows',
    'Diagnostics',
    'Diagnostic query used to inspect BudgetMonthPayee data across all months. \nReturns the BudgetMonthPayeeId, BudgetMonthId, PayeeName, PlannedAmount, \nand AccountId to verify that budget rows were created correctly and that \npayees and accounts are linked properly. Useful when troubleshooting missing \nor incorrect rows in the BudgetMonth grid.',
    'SELECT\n    bmp.BudgetMonthPayeeId,\n    bmp.BudgetMonthId,\n    p.PayeeName,\n    bmp.PlannedAmount,\n    bmp.AccountId\nFROM BudgetMonthPayee bmp\nJOIN Payee p\n  ON p.PayeeId = bmp.PayeeId\nORDER BY\n    bmp.BudgetMonthId,\n    bmp.SortIndex;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonth.SelectWithCounts',
    'Diagnostics',
    'Diagnostic query that returns all BudgetMonth rows with a count of related BudgetMonthPayee rows for each month.',
    'SELECT\n    bm.[BudgetMonthId],\n    bm.[Year],\n    bm.[Month],\n    COUNT(bmp.[BudgetMonthPayeeId]) AS [PayeeRowCount]\nFROM [BudgetMonth] bm\nLEFT JOIN [BudgetMonthPayee] bmp\n    ON bmp.[BudgetMonthId] = bm.[BudgetMonthId]\nGROUP BY\n    bm.[BudgetMonthId],\n    bm.[Year],\n    bm.[Month]\nORDER BY\n    bm.[Year],\n    bm.[Month];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'SqlQuery.SelectCategoryCounts',
    'Diagnostics',
    'Returns nlumber of quieries in the catalog',
    'SELECT\n    [Category],\n    COUNT(*) AS [QueryCount]\nFROM [SqlQuery]\nGROUP BY [Category]\nORDER BY [Category];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonth.InsertFromStage',
    'Import',
    'Creates missing BudgetMonth rows from legacy stage_Budget_old data using distinct year and month values. Existing months are ignored.',
    'INSERT OR IGNORE INTO [BudgetMonth]\n(\n    [BudgetMonthId],\n    [Year],\n    [Month],\n    [CreatedUtc]\n)\nSELECT DISTINCT\n    printf(''%04d-%02d'', sb.[BudgetYear], sb.[BudgetMonth]),\n    sb.[BudgetYear],\n    sb.[BudgetMonth],\n    CURRENT_TIMESTAMP\nFROM [stage_Budget_old] sb;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonth.MissingFromStage',
    'Import',
    'Preview query for budget migration. Shows year and month combinations in stage_Budget_old that do not yet exist in BudgetMonth.',
    'SELECT DISTINCT\n    printf(''%04d-%02d'', sb.[BudgetYear], sb.[BudgetMonth]) AS [BudgetMonthId],\n    sb.[BudgetYear]  AS [Year],\n    sb.[BudgetMonth] AS [Month]\nFROM [stage_Budget_old] sb\nLEFT JOIN [BudgetMonth] bm\n    ON bm.[BudgetMonthId] =\n       printf(''%04d-%02d'', sb.[BudgetYear], sb.[BudgetMonth])\nWHERE bm.[BudgetMonthId] IS NULL\nORDER BY\n    sb.[BudgetYear],\n    sb.[BudgetMonth];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonthPayee.ImportFromStage',
    'Import',
    'Imports BudgetMonthPayee rows from legacy stage_Budget_old data by mapping legacy Payee and Account ids into the current schema and upserting by BudgetMonthPayeeId.',
    'INSERT INTO [BudgetMonthPayee]\n(\n    [BudgetMonthPayeeId],\n    [BudgetMonthId],\n    [PayeeId],\n    [SortIndex],\n    [PlannedAmount],\n    [AccountId]\n)\nSELECT\n    printf(\n        ''%04d-%02d-%s'',\n        sb.[BudgetYear],\n        sb.[BudgetMonth],\n        p.[PayeeId]\n    ),\n\n    printf(''%04d-%02d'', sb.[BudgetYear], sb.[BudgetMonth]),\n\n    p.[PayeeId],\n\n    COALESCE(p.[SortIndex],0),\n\n    sb.[TotalAmount],\n\n    CASE\n        WHEN sb.[BankAccountID] IS NULL\n          OR sb.[BankAccountID] = 0\n        THEN NULL\n        ELSE a.[AccountId]\n    END\n\nFROM [stage_Budget_old] sb\nINNER JOIN [Payee] p\n    ON p.[PayeeId] =\n       ''legacy-payee-'' || CAST(sb.[PayeeID] AS TEXT)\nLEFT JOIN [Account] a\n    ON a.[AccountId] =\n       ''legacy-bankaccount-'' || CAST(sb.[BankAccountID] AS TEXT)\n\nON CONFLICT([BudgetMonthPayeeId]) DO UPDATE SET\n    [SortIndex]     = excluded.[SortIndex],\n    [PlannedAmount] = excluded.[PlannedAmount],\n    [AccountId]     = excluded.[AccountId];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonthPayee.ImportTemplate',
    'Import',
    'Creates BudgetMonthPayee rows for a new budget month using the payees currently marked as part of the budget template. This determines which budget items are initially visible in the new month.',
    'INSERT INTO BudgetMonthPayee\n(\n    BudgetMonthId,\n    PayeeId,\n    AmountPlanned,\n    AmountActual,\n    CreatedUtc\n)\nSELECT\n    @BudgetMonthId,\n    p.PayeeId,\n    0,\n    0,\n    CURRENT_TIMESTAMP\nFROM Payees p\nWHERE p.IncludeInBudgetTemplate = 1\nAND NOT EXISTS\n(\n    SELECT 1\n    FROM BudgetMonthPayee b\n    WHERE b.BudgetMonthId = @BudgetMonthId\n    AND b.PayeeId = p.PayeeId\n);',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonthPayee.MissingAccountMappingsFromStage',
    'Import',
    'Diagnostic query for budget migration. Shows legacy stage_Budget_old rows whose BankAccountID does not map to a current Account record.',
    'SELECT\n    sb.[BudgetYear],\n    sb.[BudgetMonth],\n    sb.[PayeeID],\n    sb.[PayeeName],\n    sb.[BankAccountID]\nFROM [stage_Budget_old] sb\nLEFT JOIN [Account] a\n    ON a.[AccountId] = ''legacy-bankaccount-'' || CAST(sb.[BankAccountID] AS TEXT)\nWHERE sb.[BankAccountID] IS NOT NULL\n  AND sb.[BankAccountID] <> 0\n  AND a.[AccountId] IS NULL\nORDER BY\n    sb.[BudgetYear],\n    sb.[BudgetMonth],\n    sb.[PayeeID];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonthPayee.MissingPayeeMappingsFromStage',
    'Import',
    'Diagnostic query for budget migration. Shows legacy stage_Budget_old rows whose PayeeID does not map to a current Payee record.',
    'SELECT\n    sb.[BudgetYear],\n    sb.[BudgetMonth],\n    sb.[PayeeID],\n    sb.[PayeeName]\nFROM [stage_Budget_old] sb\nLEFT JOIN [Payee] p\n    ON p.[PayeeId] = ''legacy-payee-'' || CAST(sb.[PayeeID] AS TEXT)\nWHERE p.[PayeeId] IS NULL\nORDER BY\n    sb.[BudgetYear],\n    sb.[BudgetMonth],\n    sb.[PayeeID];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'BudgetMonthPayee.PreviewImportFromStage',
    'Import',
    'Preview query for budget migration. Shows the BudgetMonthPayee rows that would be created from stage_Budget_old before running the import.',
    'SELECT\n    printf(\n        ''%04d-%02d-%s'',\n        sb.[BudgetYear],\n        sb.[BudgetMonth],\n        p.[PayeeId]\n    ) AS [BudgetMonthPayeeId],\n\n    printf(''%04d-%02d'', sb.[BudgetYear], sb.[BudgetMonth]) AS [BudgetMonthId],\n\n    p.[PayeeId],\n    p.[PayeeName],\n    COALESCE(p.[SortIndex], 0) AS [SortIndex],\n    sb.[TotalAmount] AS [PlannedAmount],\n\n    CASE\n        WHEN sb.[BankAccountID] IS NULL\n          OR sb.[BankAccountID] = 0\n        THEN NULL\n        ELSE a.[AccountId]\n    END AS [AccountId],\n\n    sb.[BudgetYear],\n    sb.[BudgetMonth],\n    sb.[PayeeID] AS [StagePayeeID],\n    sb.[BankAccountID] AS [StageBankAccountID]\n\nFROM [stage_Budget_old] sb\nINNER JOIN [Payee] p\n    ON p.[PayeeId] = ''legacy-payee-'' || CAST(sb.[PayeeID] AS TEXT)\nLEFT JOIN [Account] a\n    ON a.[AccountId] = ''legacy-bankaccount-'' || CAST(sb.[BankAccountID] AS TEXT)\n\nORDER BY\n    sb.[BudgetYear],\n    sb.[BudgetMonth],\n    p.[SortIndex],\n    p.[PayeeName];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'SqlQuery.SelectActiveByName',
    'Infrastructure',
    'Returns an active SqlQuery catalog entry by QueryName. Used by the query loader to resolve SQL text.',
    'SELECT\n    [QueryIndex],\n    [QueryName],\n    [Description],\n    [SqlText],\n    [Fingerprint],\n    [IsActive]\nFROM [SqlQuery]\nWHERE [QueryName] = @QueryName\nAND [IsActive] = 1;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'UserSettings.SelectByName',
    'Infrastructure',
    'Returns the SettingValue for a specific UserSettings entry identified by SettingName. Used to load persisted UI and application settings from the database.',
    'SELECT\n    [SettingValue]\nFROM [UserSettings]\nWHERE [SettingName] = @SettingName;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'UserSettings.Upsert',
    'Infrastructure',
    'Creates or updates a UserSettings entry identified by SettingName. Used to persist UI and application settings such as grid layouts in the database.',
    'INSERT INTO [UserSettings]\n(\n    [SettingName],\n    [SettingValue]\n)\nVALUES\n(\n    @SettingName,\n    @SettingValue\n)\nON CONFLICT([SettingName]) DO UPDATE SET\n    [SettingValue] = excluded.[SettingValue];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'SchemaMigrations.InsertApplied',
    'Migration',
    'Records a migration as applied in the SchemaMigrations table. Stores the MigrationId, the UTC timestamp when the migration was applied, and the SHA256 checksum of the migration SQL.',
    'INSERT INTO [SchemaMigrations]\n(\n    [MigrationId],\n    [AppliedUtc],\n    [Checksum]\n)\nVALUES\n(\n    $id,\n    $utc,\n    $sum\n);',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'SchemaMigrations.SelectAll',
    'Migration',
    'Returns all migration records from SchemaMigrations ordered by MigrationId.',
    'SELECT\n    [MigrationId],\n    [AppliedUtc],\n    [Checksum]\nFROM [SchemaMigrations]\nORDER BY\n    [MigrationId];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'SchemaMigrations.SelectAppliedIds',
    'Migration',
    'Returns the list of applied migration identifiers from SchemaMigrations ordered by MigrationId.',
    'SELECT\n    [MigrationId]\nFROM [SchemaMigrations]\nORDER BY\n    [MigrationId];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Payee.DeleteById',
    'Payee',
    'Deletes a Payee row by PayeeId.',
    'DELETE FROM [Payee]\nWHERE [PayeeId] = @PayeeId;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Payee.Insert',
    'Payee',
    'Inserts a new Payee row into the current schema.',
    'INSERT INTO [Payee]\n(\n    [PayeeId],\n    [PayeeName],\n    [IncludeInBudgetTemplate],\n    [SortIndex],\n    [IsActive],\n    [WebsiteId],\n    [DefaultAccountId],\n    [DefaultAmount]\n)\nVALUES\n(\n    @PayeeId,\n    @PayeeName,\n    @IncludeInBudgetTemplate,\n    @SortIndex,\n    @IsActive,\n    @WebsiteId,\n    @DefaultAccountId,\n    @DefaultAmount\n);',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Payee.SelectAll',
    'Payee',
    'Returns all Payee rows ordered so that payees included in the budget template appear first, followed by the remaining payees sorted alphabetically by PayeeName. Used by payee management screens.',
    'SELECT\n    [PayeeId],\n    [PayeeName],\n    [IncludeInBudgetTemplate],\n    [SortIndex],\n    [IsActive],\n    [WebsiteId],\n    [DefaultAccountId],\n    [DefaultAmount]\nFROM [Payee]\nORDER BY\n    [PayeeName];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Payee.SelectById',
    'Payee',
    'Returns a single Payee row by PayeeId.',
    'SELECT\n    [PayeeId],\n    [PayeeName],\n    [IncludeInBudgetTemplate],\n    [SortIndex],\n    [IsActive],\n    [WebsiteId],\n    [DefaultAccountId],\n    [DefaultAmount]\nFROM [Payee]\nWHERE [PayeeId] = @PayeeId;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'Payee.Update',
    'Payee',
    'Updates an existing Payee row identified by PayeeId.',
    'UPDATE [Payee]\nSET\n    [PayeeName] = @PayeeName,\n    [IncludeInBudgetTemplate] = @IncludeInBudgetTemplate,\n    [SortIndex] = @SortIndex,\n    [IsActive] = @IsActive,\n    [WebsiteId] = @WebsiteId,\n    [DefaultAccountId] = @DefaultAccountId,\n    [DefaultAmount] = @DefaultAmount\nWHERE [PayeeId] = @PayeeId;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'UserSettings.Select',
    'Settings',
    'Get All settings',
    'SELECT\n    [SettingValue]\nFROM [UserSettings]\nWHERE [SettingName] = @SettingName;',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'SqlQuery.SelectCatalog',
    'System',
    'Returns the full SQL query catalog used by the Query Catalog UI and export tools.',
    'SELECT\n        [QueryName],\n        [Category],\n        [Description],\n        [SqlText],\n        [IsActive]\n     FROM [SqlQuery]\n     ORDER BY\n        [Category],\n        [QueryName];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'SqlQuery.Upsert',
    'System',
    'Creates or updates a query definition in the SqlQuery catalog.',
    'INSERT INTO [SqlQuery]\n    (\n        [QueryName],\n        [Category],\n        [Description],\n        [SqlText],\n        [IsActive]\n    )\n    VALUES\n    (\n        @QueryName,\n        @Category,\n        @Description,\n        @SqlText,\n        @IsActive\n    )\n    ON CONFLICT([QueryName]) DO UPDATE SET\n        [Category] = excluded.[Category],\n        [Description] = excluded.[Description],\n        [SqlText] = excluded.[SqlText],\n        [IsActive] = excluded.[IsActive];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

INSERT INTO [SqlQuery]
(
    [QueryName],
    [Category],
    [Description],
    [SqlText],
    [IsActive]
)
VALUES
(
    'PayeeTag.SelectAllForNavigation',
    'Tag',
    'Returns all payee-tag assignments with tag names for grouped payee navigation in the tree.',
    'SELECT\n    pt.[PayeeId],\n    pt.[TagId],\n    t.[TagName]\nFROM [PayeeTag] pt\nINNER JOIN [Tag] t\n    ON t.[TagId] = pt.[TagId]\nWHERE t.[IsActive] = 1\nORDER BY\n    t.[TagName],\n    pt.[SortIndex];',
    1
)
ON CONFLICT([QueryName]) DO UPDATE SET
    [Category] = excluded.[Category],
    [Description] = excluded.[Description],
    [SqlText] = excluded.[SqlText],
    [IsActive] = excluded.[IsActive];

