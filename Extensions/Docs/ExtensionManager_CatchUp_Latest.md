# CodexExpensa Extension Manager – Catch-Up

## Current baseline

Repository root:
`D:\Git\CodexExpensa\Extensions`

Confirmed solution baseline:
- `CodexExpensa.ExtensionDevHost`
- `CodexExpensa.Feature.Websites`
- `CodexExpensa.Navigation.Abstractions`
- `CodexExpensa.Navigation.Hosting`
- `CodexExpensa.Navigation.Hosting.Tests`

## Locked rules

### Packaging
All zip files must open with:
`Extensions/`

Not:
- project root only
- `CodexExpensa/Extensions`
- loose files

### Manager DB
Manager DB path:
`%AppData%\Expensa\Extensions\ExtensionMgr.db`

### Extension DBs
Each extension owns its own DB.
Manager DB is registration/control only.

### Runtime loading
ExtMgr must not load extension DLLs directly from build output.
Use a copied runtime load/cache folder first.

Recommended load cache:
`%AppData%\Expensa\Extensions\Loaded\[ProjectName]\`

### SQL catalog rule
Manager SQL belongs in manager infrastructure only.

`SqlQuery` schema in manager DB is:

```sql
CREATE TABLE SqlQuery (
    QueryName   TEXT    PRIMARY KEY,
    Description TEXT    NOT NULL
                        DEFAULT (''),
    SqlText     TEXT    NOT NULL
                        DEFAULT (''),
    CreatedUtc  TEXT    NOT NULL
                        DEFAULT CURRENT_TIMESTAMP,
    Category    TEXT    NOT NULL
                        DEFAULT (''),
    IsActive    INTEGER NOT NULL
                        DEFAULT (1),
    UNIQUE (
        QueryName ASC
    )
);
```

### SqlQuery maintenance rule
All queries inserted into `SqlQuery` must be written using an `INSERT OR REPLACE` pattern.

### SqlText rule
Queries saved in `SqlQuery.SqlText` must be stored as readable text format, not compressed one-line garbage.

## Extension registration model

Manual registration is the model.

No folder scanning for dev projects.

Registered project data concept:
- `ProjectName`
- `RelativeBinPath` optional
- `AssemblyName`
- `IsEnabled`
- `SortOrder`

Path resolution concept:
- if `RelativeBinPath` exists:
  - `[RepoRoot]\[RelativeBinPath]\[AssemblyName].dll`
- else:
  - `[RepoRoot]\BuildOutput\[ProjectName]\[Configuration]\[TargetFramework]\[AssemblyName].dll`

## Command/menu system direction

The menu system evolved into a reusable command framework.

### Core design
- `IExtMgrCommand` defines the command contract
- `ExtMgrCommandBase` holds defaults + runtime metadata override support
- `CommandRegistry` is the central registry and menu builder
- `MainForm` should stay small
- registry should populate the `MenuStrip`
- command instances provide defaults
- JSON provides overrides

### Command metadata
Command model includes:
- `CommandKey`
- `TopLevelMenu`
- `MenuText`
- `MenuOrder`
- `ItemOrder`
- `IsSeparator`

### Menu ordering
Top-level menus are ordered by `MenuOrder`.
Items within menus are ordered by `ItemOrder`.

### Spacer support
A spacer/separator menu item is supported through a separator-style command type.

## Command metadata persistence

We moved away from DB for command menu config and chose JSON.

JSON path:
`%AppData%\Expensa\Extensions\CommandMenuConfig.json`

JSON stores command menu metadata overrides:
- `CommandKey`
- `MenuText`
- `MenuOrder`
- `ItemOrder`

### JSON model
- code owns defaults
- JSON owns overrides
- registry loads and applies overrides on startup
- registry saves overrides when metadata changes
- menu redraw happens live

## Current UI direction

### Project Space
There is a Project Space concept/UI direction for setting up:
- default DB
- default solution file name
- starter project structure

Default solution file name used in spec:
`Extension_Manager.slnx`

### Query catalog
A Query Catalog UI direction exists:
- query listing
- query editing
- JSON/text friendliness
- eventually manager query maintenance

### Command catalog
Command Catalog became the active editable UI for menu metadata.

Requested direction:
- see all commands
- edit caption/order
- drag to reorder
- autosave
- live redraw
- JSON tab for direct editing
- button/link to open JSON folder

## MainForm state

Uploaded real `MainForm` currently includes:
- `CommandRegistry`
- `CommandConfigPath`
- `RebuildMenu()`
- registry config load on startup
- menu rebuild event hook

That file is the real source of truth for host menu wiring.

## Important compile issue already found

A constructor mismatch happened in `CommandCatalogForm`.

Real current uploaded `CommandCatalogForm` had the 2-argument constructor:
- `CommandRegistry registry`
- `Action rebuildMenu`

Later versions expected 3 arguments:
- `CommandRegistry registry`
- `string configPath`
- `Action rebuildMenu`

The fix direction was:
- re-add the JSON editor and folder link
- restore the 3-argument constructor
- update `OpenCommandCatalogCommand` to pass:
  - `mainForm.CommandRegistry`
  - `mainForm.CommandConfigPath`
  - `mainForm.RebuildMenu`

## Architectural intent going forward

The command framework is intended to be reusable in other apps.

That means:
- keep `MainForm` thin
- keep commands decoupled from UI plumbing
- keep registry responsible for menu creation
- keep metadata persistence outside concrete command logic
- prefer generic command framework pieces over ExtMgr-specific hacks

## Next likely work items

Most likely next steps:
1. stabilize the command catalog constructor/call-site versions
2. finalize drag reorder + autosave + live redraw
3. keep JSON editor tab and open-folder support
4. possibly support cross-menu drag later
5. later add undo/redo or richer menu editing if wanted

## Notes to preserve

- Work only from real provided files
- Do not invent solution/project structures
- Zip root must be `Extensions`
- When docs are requested, include both `.md` and `.pdf`
- For this repo, partial surgical zips are preferred over fake “full repo” replacements unless explicitly requested

## Suggested commit message

```text
Refactor ExtMgr menu system into registry-driven commands with JSON-backed menu config

- keep MainForm thin and let CommandRegistry populate MenuStrip
- add command ordering and separator support
- add Command Catalog UI for viewing/editing command metadata
- move menu metadata persistence to CommandMenuConfig.json
- support live menu redraw after metadata changes
- add drag reorder/autosave direction for command layout
- preserve ExtMgr-specific paths and packaging rules
```
