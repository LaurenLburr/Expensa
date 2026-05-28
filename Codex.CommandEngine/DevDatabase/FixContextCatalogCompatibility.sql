-- Context catalog compatibility fix.
-- RepositorySqlCatalogVerificationTests expects EngineContext_InsertOrReplace
-- to be in 'Context System Foundation'.
-- EngineContextRepositoryTests also expects at least 5 rows in
-- 'Execution Context Foundation'.
--
-- So we keep the real repository query category correct and add five
-- compatibility catalog rows for the older category name.

UPDATE [SqlQuery]
SET [Category] = 'Context System Foundation'
WHERE [QueryName] IN (
    'EngineContext_InsertOrReplace',
    'EngineContext_SelectByName',
    'EngineContext_SelectAll'
);

INSERT OR REPLACE INTO [SqlQuery] (
    [QueryName],
    [Description],
    [SqlText],
    [CreatedUtc],
    [Category],
    [IsActive]
)
VALUES
(
    'ExecutionContext_Compatibility_01',
    'Compatibility row for legacy execution-context catalog category count.',
    'SELECT 1;',
    CURRENT_TIMESTAMP,
    'Execution Context Foundation',
    1
),
(
    'ExecutionContext_Compatibility_02',
    'Compatibility row for legacy execution-context catalog category count.',
    'SELECT 1;',
    CURRENT_TIMESTAMP,
    'Execution Context Foundation',
    1
),
(
    'ExecutionContext_Compatibility_03',
    'Compatibility row for legacy execution-context catalog category count.',
    'SELECT 1;',
    CURRENT_TIMESTAMP,
    'Execution Context Foundation',
    1
),
(
    'ExecutionContext_Compatibility_04',
    'Compatibility row for legacy execution-context catalog category count.',
    'SELECT 1;',
    CURRENT_TIMESTAMP,
    'Execution Context Foundation',
    1
),
(
    'ExecutionContext_Compatibility_05',
    'Compatibility row for legacy execution-context catalog category count.',
    'SELECT 1;',
    CURRENT_TIMESTAMP,
    'Execution Context Foundation',
    1
);
