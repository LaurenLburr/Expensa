# WebsitesAddin – Expensa Integration Design Spec

## Purpose

Describe how this add-in integrates with the main Expensa application.

## Integration Summary

Summary of what the add-in contributes to Expensa:

- Navigation nodes
- Commands
- Forms / screens
- Data / services
- Menus / toolbars
- Context actions

Provide details for each item above in the sections below.

## Host Touchpoints

List the areas of the host application that the add-in integrates with.

| Host Area | Integration Point | Notes |
|---|---|---|
| Main navigation tree | | |
| Menus / commands | | |
| Detail panel / content host | | |
| Database / session services | | |
| Query catalog / SqlQuery | | |
| Settings / configuration | | |

## Required Abstractions

List the shared interfaces or abstractions the add-in depends on.

| Abstraction | Source Project | Purpose |
|---|---|---|
| | | |

## Expensa Data Access

Indicate how the add-in interacts with data:

- Owns its own database.
- Uses the Expensa database.
- Reads from Expensa only.
- Writes to Expensa.
- Requires migrations.
- Requires SqlQuery catalog entries.

Select and document the applicable options above, and describe any details about schema, permissions, and migration requirements.

## SQL Catalog Entries

List any SQL queries or catalog entries the add-in requires.

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

Document the expected node types, node metadata (IDs, labels, icons), and any registration mechanism used by Expensa to enumerate add-in nodes.

## Command Contract

List commands exposed by the add-in and where they can be triggered.

| Command | Trigger Location | Behavior |
|---|---|---|
| | | |

Include command identifiers, parameters, required permissions, and any asynchronous behavior or feedback expected by the host.

## UI Hosting Contract

Describe which forms / screens are hosted by Expensa and how they are opened.

| Screen | Host Location | Notes |
|---|---|---|
| | | |

Specify whether screens are hosted as modal dialogs, docked panels, or routed pages, and indicate any dependencies on host context or services.

## Startup / Registration Flow

Describe what must happen when Expensa starts:

1. Register the add-in with the host registry / composition container.
2. Register navigation nodes and menu items (deferred registration if supported).
3. Initialize data services, set connection strings, and run any required migrations.
4. Register SQL queries with the host query catalog (if required).
5. Subscribe to host events and initialize any background tasks or caches.

Adjust the steps above to reflect the actual startup sequence and error-handling requirements.

## Deployment / Update Flow

Describe how this add-in should be deployed or updated into Expensa.

- Packaging format (e.g., NuGet, ZIP, installer).
- Installation steps (copy files, run installer, register).
- Configuration updates (connection strings, feature flags).
- Database migration steps and rollback strategy.
- Versioning and compatibility policy with host Expensa versions.
- Hot-reload or restart requirements for the host.

Document any CI/CD automation and any manual steps required for production updates.

## Risks / Open Questions

- List any integration risks (e.g., breaking changes, data-loss risk).
- Describe outstanding design decisions or clarifying questions for host teams.
- Note any performance concerns, security implications, or required approvals.

## Revision History

| Date | Change | Reason |
|---|---|---|
| | | |