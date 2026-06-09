# Test Cleanup Phase 2 – CommandEngineIntegration Tests

This updates `CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests`.

## Why

The existing tests were very narrow smoke-runtime tests. They were not wrong, but they were brittle and did not clearly protect the current public contract.

## Updated coverage

The replacement tests cover:

```text
ExtensionRuntimeManager status lifecycle
Controller delegation
Smoke runtime startup
Smoke command execution
Reload behavior
Stop behavior
Diagnostic text availability
Project shape
CommandEngine.Core reference
SmokeRuntimeProviderFactory using CommandEngine smoke provider
```

## Design goal

The tests now focus on the current integration contract instead of old implementation details.
