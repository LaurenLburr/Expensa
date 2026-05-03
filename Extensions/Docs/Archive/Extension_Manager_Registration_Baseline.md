# Extension Manager Registration Baseline

## Purpose

This package adds a basic SQLite-backed registration model for the Extension Manager.

It supports **manual registration of development projects** instead of scanning folders for DLLs.

## Included files

- `Extensions/Core/CodexExpensa.Navigation.Abstractions/Models/ExtensionProjectRegistration.cs`
- `Extensions/Host/CodexExpensa.ExtensionDevHost/Data/ExtensionRegistrationRecord.cs`
- `Extensions/Host/CodexExpensa.ExtensionDevHost/Data/ExtensionManagerDatabase.cs`
- `Extensions/Host/CodexExpensa.ExtensionDevHost/Data/ExtensionManagerSchema.sql`
- `Extensions/Host/CodexExpensa.ExtensionDevHost/Data/ExtensionManager.basic.db`

## Important note

This package is **additive starter work**, not a replacement for your current implementation.

It does **not** overwrite your existing `MainForm.cs` or other live host files because I do not have your current real host files in this conversation.

## Database table

`ExtensionProject`

Columns:

- `ProjectName` (`TEXT`, primary key)
- `RelativeBinPath` (`TEXT`, nullable)
- `AssemblyName` (`TEXT`, required)
- `IsEnabled` (`INTEGER`, required, default `1`)
- `SortOrder` (`INTEGER`, required, default `0`)

## Path resolution behavior

If `RelativeBinPath` is empty, code derives the default location:

`[RepoRoot]\BuildOutput\[ProjectName]\[Configuration]\[TargetFramework]\[AssemblyName].dll`

If `RelativeBinPath` is provided, code resolves:

`[RepoRoot]\[RelativeBinPath]\[AssemblyName].dll`

## Seed data in the basic database

The starter database includes:

- `CodexExpensa.Feature.Websites` (enabled)
- `CodexExpensa.Navigation.Hosting.Tests` (disabled example)

## Next integration step

The next real step is to wire your actual host UI and loading pipeline to:

1. open this database
2. read enabled registrations
3. resolve assembly paths
4. load assemblies by reflection

Because I do not have your live current `MainForm.cs`, that integration step is intentionally **not** included in this package.
