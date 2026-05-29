# Website Load Runtime Smoke Runner

This slice adds a tiny smoke runner around the real runtime handler:

```text
WebsiteLoadRuntimeCommandHandler
```

The runner executes:

```text
websites.load
```

and can save the command output JSON to disk.

Default target for console-style use:

```text
%LOCALAPPDATA%\Expensa\Extensions\WebsitesAddin\websites-load-smoke.json
```

This gives us a safe verification point before wiring the result into the real Expensa `TreeView`.

## Why this matters

The current bridge now has three clean layers:

```text
WebsiteLoadCommand
  UI-neutral service

WebsiteLoadRuntimeCommandHandler
  CommandEngine runtime adapter

WebsiteLoadRuntimeSmokeRunner
  test/smoke execution helper
```

No WinForms dependency belongs in the module yet.
