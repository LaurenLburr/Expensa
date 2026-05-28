--
-- Codex.CommandEngine AiProviderCapability CreatedUtc NULL patch
-- Apply to:
-- D:\Git\CodexExpensa\Codex.CommandEngine\DevDatabase\CommandEngine.dev.db
--
-- Fixes:
-- System.InvalidOperationException:
-- The data is NULL at ordinal 6.
--
-- Ordinal 6 in AiProviderRepository.ReadCapability is CreatedUtc.
--

PRAGMA foreign_keys = OFF;

BEGIN TRANSACTION;

UPDATE AiProviderCapability
SET CreatedUtc = CURRENT_TIMESTAMP
WHERE CreatedUtc IS NULL
   OR trim(CreatedUtc) = '';

CREATE TRIGGER IF NOT EXISTS TR_AiProviderCapability_SetCreatedUtc_AfterInsert
AFTER INSERT ON AiProviderCapability
FOR EACH ROW
WHEN NEW.CreatedUtc IS NULL OR trim(NEW.CreatedUtc) = ''
BEGIN
    UPDATE AiProviderCapability
    SET CreatedUtc = CURRENT_TIMESTAMP
    WHERE AiProviderCapabilityId = NEW.AiProviderCapabilityId;
END;

COMMIT TRANSACTION;

PRAGMA foreign_keys = ON;
