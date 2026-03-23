INSERT OR REPLACE INTO [SqlQuery]
([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES
(
'AccountTag.SelectAllForNavigation',
'Get account-tag navigation rows for account tree grouping with account display data',
'SELECT at.[AccountId],
        t.[TagId],
        t.[TagName],
        a.[AccountNickname],
        a.[AccountNumber]
 FROM [AccountTag] at
 JOIN [Tag] t
   ON t.[TagId] = at.[TagId]
 JOIN [Account] a
   ON a.[AccountId] = at.[AccountId]
 WHERE t.[IsActive] = 1
 ORDER BY t.[TagName], at.[SortIndex], a.[AccountNickname], a.[AccountNumber];',
'AccountTag',
1
);
