# Extension Dev Host Refactor Step 9 - Workspace Cleanup

## Scope

Cleans up remaining obsolete navigation leftovers from the older popup/tool-launcher architecture.

## Removed

Potential legacy nodes:
- AI Scaffold Files
- AI Test
- duplicate static docs entries

## Added

Workspace document:

```text
Docs
└── Workspace Roadmap
```

Stored under:

```text
%AppData%\Expensa\Extensions\WorkspaceDocs
```

## Purpose

This begins shifting roadmap/planning into editable workspace documentation instead of static notes or external planning docs.

## Architectural Direction

The application is now behaving more like:
- lightweight IDE shell
- project workspace manager
- AI-assisted development environment

instead of:
- popup utility launcher
- disconnected forms collection
