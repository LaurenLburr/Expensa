# Codex → Expensa --- Conversation Catch-Up

## Solution / Repo State

Repo root: D:`\Git`{=tex}`\CodexExpensa`{=tex}

Solution: Expensa`\Expensa`{=tex}.sln

Remote: origin → https://github.com/LaurenBurr/Expensa.git

Branching model: main = stable feature branches per vertical slice

------------------------------------------------------------------------

## High-Level Architecture

CodexExpensa (repo root -- future umbrella for multiple apps) └──
Expensa (current solution)

Codex = container Expensa = this app

------------------------------------------------------------------------

## Projects in the Solution

1.  CodexExpensa.App.WinForms\
    UI layer, composition root, runs migrations at startup

2.  CodexExpensa.Core\
    Pure domain + abstractions (no SQLite / no WinForms)

3.  CodexExpensa.Data.Sqlite\
    SQLite implementation

    -   Long-lived connection\
    -   In-memory DB seeded from file\
    -   Save() flush to disk\
    -   Migration runner

4.  CodexExpensa.Db.Schema\
    Embedded migration scripts (\*.mig)

5.  CodexExpensa.Security.Windows\
    DPAPI encryption services (for secrets)

------------------------------------------------------------------------

## Database Strategy

Runtime model: - File DB = persistence - App opens in-memory DB cloned
from file - Save() flushes memory → file

Migrations: - Embedded in schema DLL - Auto-run at startup - Safe to run
multiple times - Blank migrations allowed - DB status screen shows
applied/pending/missing

------------------------------------------------------------------------

## Security Direction

Windows DPAPI for: - Bank routing/account numbers - Website
credentials - Security questions

Encryption handled at the data access boundary.

------------------------------------------------------------------------

## Functional Direction

Budgets (one per month) Each row: - Payee - Expected amount - Selected
account - Status: Projected / Outstanding / Cleared

Accounts: - Multiple banks - Multiple accounts per bank - Types:
Checking, Savings, CreditCard

Transactions rule: Account balances derived from transactions only.

Reconciliation: Done on the main budget screen.

Budget template: Used to create a new month, but months may add
non-template rows.

------------------------------------------------------------------------

## Current Technical Capabilities

-   SQLite session abstraction
-   In-memory runtime database
-   Save() for persistence
-   Migration pipeline
-   Git + VS integration working

Infrastructure phase complete.

------------------------------------------------------------------------

## Current Development Phase

Vertical Slice #1 --- Accounts (read-only)

Goal: DB → Repository → Core model → WinForms grid

Domain started:

Core └── Domain └── Accounts Account AccountType IAccountRepository

Next step: Implement SQLite account repository.

------------------------------------------------------------------------

## Development Workflow

Each feature: - New branch - One vertical slice - End-to-end working -
Merge to main

------------------------------------------------------------------------

## Key Design Rules

-   Core has zero infrastructure dependencies
-   Data layer maps to Core models
-   UI never talks SQL
-   Full file responses
-   DB schema evolves via idempotent scripts
-   Secrets encrypted via DPAPI

------------------------------------------------------------------------

## Immediate Next Task

Implement: CodexExpensa.Data.Sqlite → Accounts → SqliteAccountRepository

Then display accounts in WinForms UI.

------------------------------------------------------------------------

## Resume Instruction for New Chat

Continue Expensa --- Vertical Slice 1 (Accounts read-only). Implement
SQLite account repository.
