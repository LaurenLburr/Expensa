# CommandEngine Dashboard Test Harness

This project launches `ExtensionRuntimeDashboardForm` directly so you can test the CommandEngine integration dashboard without changing the existing ExtensionDevHost `MainForm`.

## Add to Solution

Add this project to your Extensions solution:

```text
D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.TestHarness\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.TestHarness.csproj
```

## Required Project References

Make sure these projects are also loaded in the solution:

```text
Codex.CommandEngine.Abstractions
Codex.CommandEngine.Core
CodexExpensa.ExtensionDevHost.CommandEngineIntegration
CodexExpensa.ExtensionDevHost.CommandEngineIntegration.TestHarness
```

## Run

Set this project as the startup project:

```text
CodexExpensa.ExtensionDevHost.CommandEngineIntegration.TestHarness
```

Then press `F5`.

## Expected Behavior

The dashboard should open directly.

Click:

```text
Start Smoke Runtime
```

Expected:

```text
Started: Runtime started with 1 command(s).
```

The command list should show:

```text
extension.smoke.test
```

Select it and click:

```text
Execute Selected
```

Expected popup:

```text
Succeeded: Extension runtime smoke test completed.
```

This proves the dashboard, runtime manager, smoke provider, and CommandEngine runtime can all work from the Extensions side.
