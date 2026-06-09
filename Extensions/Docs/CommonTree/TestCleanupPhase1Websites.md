# Test Cleanup Phase 1 – WebsitesAddin

The uploaded `WebsitesAddin.Tests` project was testing an older Websites add-in implementation that is no longer present.

## Current WebsitesAddin shape

The uploaded `WebsitesAddin` project currently contains:

```text
WebsitesAddinExtension.cs
WebsitesAddin.csproj
Docs
README / scaffold docs
```

The current extension class is:

```csharp
WebsitesAddinExtension : ITreeNodeExtension
```

## Removed stale test expectations

The old test set expected removed/absent classes and files:

```text
WebsiteLoadCommand
WebsiteLoadRequest
WebsiteLoadRuntimeSmokeRunner
WebsiteLoadCommandHandler
SqliteWebsiteRepository
WebsiteInMemoryDatabaseFactory
WebsitesAddinCommandProvider
WebsitesRuntimeCommandRegistrationProvider
extension.json
DevDatabase scripts
```

Those tests are stale against the uploaded project.

## Replacement tests

The new tests verify the current project shape:

```text
WebsitesAddinExtension implements ITreeNodeExtension
ExtensionKey is WebsitesAddin
SortOrder is 100
Project references Navigation.Abstractions
Legacy runtime-command classes are not required by current tests
```

## Important

This cleanup does not restore the older runtime-command WebsitesAddin. It makes the tests match the uploaded current project.
