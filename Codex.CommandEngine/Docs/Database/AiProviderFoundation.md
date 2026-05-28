# AI Provider Foundation

Schema version: 0007

This step adds the reusable AI provider foundation for Codex.CommandEngine.

## Purpose

The AI provider layer separates provider configuration and provider capability metadata from command and workflow execution.

The engine can now track:

- AI provider definitions
- provider kinds
- provider configuration JSON
- provider metadata JSON
- provider capabilities
- enabled/disabled provider state

No API keys or secrets should be stored in this database. Provider configuration should reference safe settings only. Secrets belong in environment variables, secure user settings, or a future secret provider.

## New abstractions

- `IAiProvider`
- `IAiProviderRegistry`
- `AiProviderRequest`
- `AiProviderResponse`
- `AiProviderDescriptor`

## New core implementation

- `AiProviderRegistry`

## New data objects

- `AiProviderRecord`
- `AiProviderUpsert`
- `AiProviderCapabilityRecord`
- `AiProviderCapabilityUpsert`
- `AiProviderRepository`

## Database changes

The existing `AiProvider` table is expanded with:

- `DisplayName`
- `Description`
- `MetadataJson`

The new `AiProviderCapability` table stores provider capability metadata.

## SQL catalog entries

- `AiProvider_InsertOrReplace`
- `AiProvider_SelectByName`
- `AiProvider_SelectAll`
- `AiProviderCapability_InsertOrReplace`
- `AiProviderCapability_SelectByProvider`

## Host UI changes

The host now has:

- `AI Providers -> Provider Definitions`
- `AI Providers -> Provider Capabilities`

## Testing

Added tests cover:

- provider registry registration
- duplicate provider rejection
- provider resolution
- provider ordering
- provider persistence
- provider capability persistence
- SQL catalog seeding
- schema snapshot seeding
