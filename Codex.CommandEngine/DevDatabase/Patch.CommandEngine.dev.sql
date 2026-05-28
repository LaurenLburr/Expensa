--
-- Codex.CommandEngine template DB schema patch
-- Apply this to:
-- D:\Git\CodexExpensa\Codex.CommandEngine\DevDatabase\CommandEngine.dev.db
--

PRAGMA foreign_keys = OFF;

BEGIN TRANSACTION;

ALTER TABLE AiProvider ADD COLUMN DisplayName TEXT NOT NULL DEFAULT '';
ALTER TABLE AiProvider ADD COLUMN Description TEXT NOT NULL DEFAULT '';
ALTER TABLE AiProvider ADD COLUMN MetadataJson TEXT NOT NULL DEFAULT '{}';

ALTER TABLE ExecutionContext ADD COLUMN Scope TEXT NOT NULL DEFAULT '';
ALTER TABLE ExecutionContext ADD COLUMN Description TEXT NOT NULL DEFAULT '';
ALTER TABLE ExecutionContext ADD COLUMN MetadataJson TEXT NOT NULL DEFAULT '{}';
ALTER TABLE ExecutionContext ADD COLUMN IsEnabled INTEGER NOT NULL DEFAULT 1;

CREATE TABLE IF NOT EXISTS AiProviderCapability (
    AiProviderCapabilityId TEXT PRIMARY KEY,
    AiProviderId           TEXT NOT NULL,
    CapabilityName         TEXT NOT NULL,
    Description            TEXT NOT NULL DEFAULT '',
    MetadataJson           TEXT NOT NULL DEFAULT '{}',
    IsEnabled              INTEGER NOT NULL DEFAULT 1,
    CreatedUtc             TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc             TEXT NULL,
    FOREIGN KEY (AiProviderId) REFERENCES AiProvider(AiProviderId) ON DELETE CASCADE,
    UNIQUE (AiProviderId, CapabilityName)
);

CREATE INDEX IF NOT EXISTS IX_AiProviderCapability_AiProviderId
ON AiProviderCapability (AiProviderId);

CREATE INDEX IF NOT EXISTS IX_AiProviderCapability_CapabilityName
ON AiProviderCapability (CapabilityName);

UPDATE AiProvider
SET DisplayName = ProviderName
WHERE DisplayName = '';

UPDATE ExecutionContext
SET Scope = 'Default'
WHERE Scope = '';

INSERT OR REPLACE INTO SqlQuery
(
    QueryName,
    Description,
    SqlText,
    Category,
    IsActive,
    UpdatedUtc
)
VALUES
(
    'AiProviderCapability_InsertOrReplace',
    'Inserts or updates an AI provider capability.',
    'INSERT INTO AiProviderCapability (
    AiProviderCapabilityId,
    AiProviderId,
    CapabilityName,
    Description,
    MetadataJson,
    IsEnabled
)
VALUES (
    $AiProviderCapabilityId,
    $AiProviderId,
    $CapabilityName,
    $Description,
    $MetadataJson,
    $IsEnabled
)
ON CONFLICT(AiProviderId, CapabilityName) DO UPDATE SET
    Description = excluded.Description,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;',
    'AI Provider Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'AiProviderCapability_SelectByProvider',
    'Lists AI provider capabilities for one provider.',
    'SELECT AiProviderCapabilityId,
       AiProviderId,
       CapabilityName,
       Description,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProviderCapability
WHERE AiProviderId = $AiProviderId
ORDER BY CapabilityName ASC;',
    'AI Provider Foundation',
    1,
    CURRENT_TIMESTAMP
);

COMMIT TRANSACTION;

PRAGMA foreign_keys = ON;
