# CommandEngine Dashboard Testing Strategy

## Purpose

This document explains how to verify the CommandEngine integration dashboard inside the ExtensionMgr / ExtensionDevHost solution.

The dashboard form exists in the new integration project, but it will **not appear automatically** until the Host UI opens it from a menu item, button, or temporary test hook.

That is expected.

## Current Status

The integration project adds:

- `ExtensionRuntimeManager`
- `ExtensionRuntimeDashboardController`
- `ExtensionRuntimeDashboardForm`
- smoke-test runtime provider
- controller tests
- runtime lifecycle support

The dashboard form should exist at:

```text
D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost.CommandEngineIntegration\ExtensionRuntimeDashboardForm.cs
```

If you cannot find it, check that this project is loaded in the solution:

```text
D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost.CommandEngineIntegration\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.csproj
```

## Why You Cannot See the Form Yet

The form is a class, not an automatically launched window.

Nothing in the existing ExtensionDevHost UI opens it yet.

That means Visual Studio can build it, but the running app will not show it until we add a Host UI entry point.

## Verification Level 1: Build

In Visual Studio:

```text
Build → Rebuild Solution
```

Expected result:

```text
Build succeeded.
```

If the build fails, fix project references first.

Required loaded projects:

```text
Codex.CommandEngine.Abstractions
Codex.CommandEngine.Core
CodexExpensa.ExtensionDevHost.CommandEngineIntegration
CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests
```

## Verification Level 2: Tests

Run all tests:

```text
Test → Run All Tests
```

Expected test classes include:

```text
ExtensionRuntimeManagerTests
ExtensionRuntimeDashboardControllerTests
ExtensionRuntimeHostTests
ExtensionCommandRuntimeTests
CommandEngineRuntimeTests
```

The most relevant test class for the dashboard slice is:

```text
ExtensionRuntimeDashboardControllerTests
```

These tests verify:

- smoke runtime starts
- smoke command is registered
- smoke command executes
- runtime reload works
- runtime stop works

## Verification Level 3: Manually Open the Dashboard Form

The form needs to be opened from the existing Host app.

Add a temporary menu item, toolbar button, or debug button in the Host UI.

Temporary verification method:

```csharp
private void OpenCommandEngineDashboard()
{
    using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionRuntimeDashboardForm form = new();
    form.ShowDialog(this);
}
```

Then wire that method to a temporary button or menu click.

This is only a verification hook. It does not need to be polished yet.

## Expected Dashboard Behavior

### Initial Open

Expected summary:

```text
NotStarted: Runtime has not been started.
```

Expected command list:

```text
Empty
```

Expected diagnostics:

```text
Runtime host diagnostic text.
```

### Click Start Smoke Runtime

Expected summary:

```text
Started: Runtime started with 1 command(s).
```

Expected command:

```text
extension.smoke.test
```

Expected display name:

```text
Extension Smoke Test
```

### Select Command and Click Execute Selected

Expected popup:

```text
Succeeded: Extension runtime smoke test completed.
```

This proves the Host app can:

- create the dashboard
- start the CommandEngine runtime
- register a provider
- list commands
- execute a command
- display results

### Click Reload Smoke Runtime

Expected:

- no crash
- command list still contains `extension.smoke.test`
- status remains started

### Click Stop Runtime

Expected:

- command list clears
- summary shows stopped state
- executing a command should fail because the runtime is stopped

## Common Problems

### Problem: I Cannot Find the Form

Check whether this file exists:

```text
Extensions\Host\CodexExpensa.ExtensionDevHost.CommandEngineIntegration\ExtensionRuntimeDashboardForm.cs
```

If it does not exist, the dashboard patch was not extracted to the correct root.

Extract the patch into:

```text
D:\Git\CodexExpensa
```

Not into:

```text
D:\Git\CodexExpensa\Extensions
```

### Problem: Project Builds But Form Does Not Show

That is normal.

The form is not wired into the Host UI yet.

You need the temporary menu/button hook described above.

### Problem: Project Reference Fails

Make sure these projects are loaded in the same solution:

```text
Codex.CommandEngine.Abstractions
Codex.CommandEngine.Core
CodexExpensa.ExtensionDevHost.CommandEngineIntegration
CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests
```

### Problem: Smoke Command Does Not Appear

Check that `Start Smoke Runtime` was clicked.

The smoke command is registered by:

```text
SmokeRuntimeProviderFactory.Create()
```

The command name should be:

```text
extension.smoke.test
```

## What This Testing Proves

If all checks pass, the CommandEngine integration has proven:

- runtime bootstrap works
- provider registration works
- diagnostics flow works
- command listing works
- command execution works
- reload works
- stop works
- the Host can display and drive CommandEngine

## Next Implementation Step

After this verification, the next slice should wire the dashboard into the real ExtensionDevHost UI.

Recommended next slice:

```text
Add a CommandEngine menu item or tree node in ExtensionDevHost that opens ExtensionRuntimeDashboardForm.
```

That should be treated as a Host UI code change and should use the current Host project files.
