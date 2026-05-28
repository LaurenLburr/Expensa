# CommandEngine Integration Starter

This project is the first bridge between ExtensionMgr / ExtensionDevHost and `Codex.CommandEngine.Core`.

It intentionally does not modify the existing ExtensionMgr UI yet.

## Current Purpose

- Own one long-lived `ExtensionRuntimeManager`.
- Start/reload/stop the CommandEngine extension runtime.
- Provide UI-ready snapshots.
- Execute registered commands through a stable boundary.
- Provide a smoke-test provider for first UI wiring.

## First UI Wiring Target

Use `SmokeRuntimeProviderFactory.Create()` to start the runtime, then:

1. Show `manager.GetSnapshot().Host.Summary`.
2. Bind `manager.GetSnapshot().Host.Commands` to a grid/tree.
3. Display `manager.GetSnapshot().DiagnosticText` in a diagnostics textbox.
4. Execute `ExtensionSmokeTestCommandHandler.RegisteredCommandName`.
