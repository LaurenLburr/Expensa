# MainForm Ambiguity Cleanup

## Problem

A test zip placed `MainForm.cs` here:

```text
Extensions\Host\CodexExpensa.ExtensionDevHost\MainForm.cs
```

That created a duplicate `MainForm` class alongside the real one and caused ambiguity errors.

## Correct location

The real file belongs here:

```text
Extensions\Host\CodexExpensa.ExtensionDevHost\Extensions\MainForm.cs
```

## Cleanup

After extracting this zip through the watcher, run:

```powershell
D:\Git\CodexExpensa\Cleanup_Duplicate_MainForm.ps1
```

That script removes the bad duplicate project-root `MainForm.cs`.

## Why this happened

The pasted file was real, but the test zip path was wrong. The auto-update pipeline extracted correctly; the payload was dumb. Tiny distinction, huge compiler tantrum.
