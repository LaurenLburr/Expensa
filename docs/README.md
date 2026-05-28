# Repository Query Name Constants

Extract this zip into:

`D:\Git\CodexExpensa`

## What this does

Adds:

`Core\Codex.CommandEngine.Data\RepositorySqlQueryNames.cs`

Then updates repositories to use named constants instead of magic string query names.

## Updated repositories

- `AiProviderRepository.cs`
- `CommandDefinitionRepository.cs`
- `EngineContextRepository.cs`
- `ExecutionHistoryRepository.cs`
- `WorkflowDefinitionRepository.cs`

## Added integration test

`Tests\Codex.CommandEngine.IntegrationTests\RepositorySqlQueryNameConstantTests.cs`

It verifies:

- query-name constants have no duplicates
- every constant exists as an active, non-empty SqlQuery row in the template DB

## Helper script

After extracting, run:

`D:\Git\CodexExpensa\Codex.CommandEngine\Tools\RepositoryQueryNames\Replace-RepositoryQueryNames.bat`

The script finds the actual repository files, backs them up as `.bak`, replaces them, and deletes `bin`/`obj`.
