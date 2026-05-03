# AiGeneratedAddin – Expensa Integration Design Spec

## Purpose

Describe how this add-in integrates with the main Expensa application.

## Integration Summary

Explain what the add-in contributes to Expensa:

- Navigation nodes:
- Commands:
- Forms/screens:
- Data/services:
- Menus/toolbars:
- Context actions:

## Host Touchpoints

| Host Area | Integration Point | Notes |
|---|---|---|
| Main navigation tree | | |
| Menus/commands | | |
| Detail panel/content host | | |
| Database/session services | | |
| Query catalog / SqlQuery | | |
| Settings/configuration | | |

## Required Abstractions

List the shared interfaces or abstractions the add-in depends on.

| Abstraction | Source Project | Purpose |
|---|---|---|
| | | |

## Expensa Data Access

Describe whether the add-in:

- owns its own database
- uses the Expensa database
- reads from Expensa only
- writes to Expensa
- requires migrations
- requires SqlQuery catalog entries

## SQL Catalog Entries

| Query Name | Purpose | Owner |
|---|---|---|
| | | |

## Navigation Contract

Describe how Expensa discovers and places this add-in in navigation.

```text
Expensa
└── [Add-in Root Node]
    └── [Child Nodes]
```

## Command Contract

| Command | Trigger Location | Behavior |
|---|---|---|
| | | |

## UI Hosting Contract

Describe which forms/screens are hosted by Expensa and how they are opened.

| Screen | Host Location | Notes |
|---|---|---|
| | | |

## Startup / Registration Flow

Describe what must happen when Expensa starts:

1. 
2. 
3. 

## Deployment / Update Flow

Describe how this add-in should be deployed or updated into Expensa.

## Risks / Open Questions

- 

## Revision History

| Date | Change | Reason |
|---|---|---|
| | | |