# Extension Manager Spec (Updated)

## Core Architecture

- One manager DB: `%AppData%\Expensa\Extensions\ExtensionMgr.db`
- One DB per extension
- Manager DB = registration + control only
- Extensions own their own data

## Registration

Table: `ExtensionProject`

Columns:
- `ProjectName`
- `RelativeBinPath` (optional)
- `AssemblyName`
- `IsEnabled`
- `SortOrder`

## Path Resolution

If `RelativeBinPath` exists:

`[RepoRoot]\[RelativeBinPath]\[AssemblyName].dll`

Else:

`[RepoRoot]\BuildOutput\[ProjectName]\[Configuration]\[TargetFramework]\[AssemblyName].dll`

## Runtime Load Rule (Locked)

ExtMgr must **not** load an extension directly from its compiled output folder.

Instead:

1. Build the extension project normally.
2. Copy the compiled extension binaries to a separate runtime load folder.
3. Load the copied binaries from that runtime load folder.

### Why

This avoids file locking on the original build output DLLs and keeps the normal edit → build → reload loop working.

### Recommended load cache

`%AppData%\Expensa\Extensions\Loaded\[ProjectName]\`

### Development workflow

1. Edit extension code
2. Build extension project
3. ExtMgr copies binaries to load cache
4. ExtMgr reloads extension from copied binaries

## Packaging Rule (Locked)

All zip files must:

- be rooted at `Extensions`
- not include `CodexExpensa` above it
- not start at project level

### Correct structure

```text
Extensions/
    Host/
    Core/
    Modules/
    Tests/
```

This allows direct extraction into:

`D:\Git\CodexExpensa\Extensions`

## Next Step: Project Space Setup UI

The next UI feature is a **Project Space Setup** workflow inside ExtMgr.

### Purpose

Create a ready-to-use extension development space with:
- a default manager DB
- a default solution file name
- the basic project files

### Inputs

Recommended minimum inputs:
- `Project Space Name`
- `Root Folder`
- `Default Solution File Name`
- optional overwrite/initialize confirmation

### Defaults

#### Default manager DB path

`%AppData%\Expensa\Extensions\ExtensionMgr.db`

#### Default solution file name

`Extension_Manager.slnx`

#### Default project set

- `CodexExpensa.ExtensionDevHost`
- `CodexExpensa.Navigation.Abstractions`
- `CodexExpensa.Navigation.Hosting`
- `CodexExpensa.Feature.Websites`
- `CodexExpensa.Navigation.Hosting.Tests`

### What the setup action should create

Under the chosen project space root:

- solution file
- core project folders
- host project folder
- modules folder
- tests folder
- starter project files
- default manager DB if missing

### Expected behavior

- create missing folders
- create default DB if missing
- create starter files only when requested
- avoid silently overwriting existing work
- report exactly what was created

## Summary

The current baseline is:

- one manager DB at `%AppData%\Expensa\Extensions\ExtensionMgr.db`
- one separate DB per extension
- manual extension registration
- standard compiled output path with optional override
- runtime load by copying binaries to a separate load cache first
- zip packaging rooted at `Extensions`
- next UI target is Project Space Setup
