-- Fix SqlQuery category expected by RepositorySqlCatalogVerificationTests.
-- The tests expect Context System Foundation, not Execution Context Foundation.

UPDATE [SqlQuery]
SET [Category] = 'Context System Foundation'
WHERE [Category] = 'Execution Context Foundation';

-- More targeted safety net for known context query names.
UPDATE [SqlQuery]
SET [Category] = 'Context System Foundation'
WHERE [QueryName] IN (
    'ExecutionContext_Insert',
    'ExecutionContext_Update',
    'ExecutionContext_FindById',
    'ExecutionContext_ListActive',
    'ExecutionContextVariable_Upsert',
    'ExecutionContextVariable_ListByContextId'
);
