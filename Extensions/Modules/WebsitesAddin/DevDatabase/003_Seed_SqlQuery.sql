INSERT OR REPLACE INTO [SqlQuery] (
    [QueryName],
    [SqlText],
    [Description],
    [UpdatedUtc]
)
VALUES
(
    'Website.Select.Enabled',
    'SELECT [WebsiteId], [DisplayName], [Url], [Category], [IsEnabled], [SortOrder] FROM [Website] WHERE [IsEnabled] = 1 ORDER BY [SortOrder], [DisplayName];',
    'Select enabled websites ordered for tree loading.',
    CURRENT_TIMESTAMP
),
(
    'Website.Select.All',
    'SELECT [WebsiteId], [DisplayName], [Url], [Category], [IsEnabled], [SortOrder] FROM [Website] ORDER BY [SortOrder], [DisplayName];',
    'Select all websites ordered for tree loading.',
    CURRENT_TIMESTAMP
),
(
    'Website.Select.Search.Enabled',
    'SELECT [WebsiteId], [DisplayName], [Url], [Category], [IsEnabled], [SortOrder] FROM [Website] WHERE [IsEnabled] = 1 AND ([DisplayName] LIKE @SearchText OR [Url] LIKE @SearchText OR [Category] LIKE @SearchText) ORDER BY [SortOrder], [DisplayName];',
    'Search enabled websites ordered for tree loading.',
    CURRENT_TIMESTAMP
),
(
    'Website.Select.Search.All',
    'SELECT [WebsiteId], [DisplayName], [Url], [Category], [IsEnabled], [SortOrder] FROM [Website] WHERE ([DisplayName] LIKE @SearchText OR [Url] LIKE @SearchText OR [Category] LIKE @SearchText) ORDER BY [SortOrder], [DisplayName];',
    'Search all websites ordered for tree loading.',
    CURRENT_TIMESTAMP
);
