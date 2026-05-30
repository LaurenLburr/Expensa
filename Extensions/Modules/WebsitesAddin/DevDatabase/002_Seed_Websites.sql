INSERT OR REPLACE INTO [Website] (
    [WebsiteId],
    [DisplayName],
    [Url],
    [Category],
    [IsEnabled],
    [SortOrder]
)
VALUES
(
    'banking.demo',
    'Demo Bank',
    'https://example.com/bank',
    'Banking',
    1,
    100
),
(
    'utilities.demo',
    'Demo Utility',
    'https://example.com/utility',
    'Utilities',
    1,
    200
),
(
    'disabled.demo',
    'Disabled Demo Site',
    'https://example.com/disabled',
    'Archive',
    0,
    900
);
