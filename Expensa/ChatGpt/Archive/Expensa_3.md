📄 CodexExpensa – Catch Up (Expensa 2)

STATUS
- App starts cleanly.
- Migrations, startup bootstrap, and DB configuration are working.
- Do NOT modify bootstrap or migration logic.
- Database path is fixed and correct:
  %LOCALAPPDATA%\CodexExpensa\db\codexexpensa.db
  Built using:
  Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CodexExpensa","db")

DATABASE
- DB opened via SqliteDatabase.OpenMemorySeededFromFile(dbPath)
- Migrations applied via MigrationRunner.ApplyPendingMigrations(db)
- SchemaMigrations table is populating correctly.
- DB saved on FormClosing using db.Save()

NAVIGATION TREE
Current structure:
- Banks
  - Bank nodes (tag = bankId)
- Accounts
  - Bank nodes (tag = bankId)
    - Account nodes (tag = accountId)
- Budgets (in progress)

Accounts tree behavior:
- Bank nodes under Accounts behave like Bank nodes under Banks.
- Context menu on bank node includes Add Checking Account (preselects bank).
- Account leaf nodes support Delete Account.
- AccountDetailsForm now correctly calls ShowChildForm (bug fixed).

CREDENTIALS
- Stored via WindowsCredentialStore.
- Key format: CredentialKeys.Bank(bankId)
- BankDetailsForm opens BankCredentialsForm with bank-scoped key.

TRANSACTIONS (UI IN PROGRESS)
- AccountTransactionsPanel (UserControl) introduced.
- DataGridView columns:
  - TransactionId (readonly)
  - Account dropdown
  - Payee dropdown
  - Status dropdown (Projected / Outstanding / Cleared)
  - Amount
  - StartDate
  - Confirm
  - Note
- Status enum intended for reuse in Budget UI.

BUDGETS (NEXT MAJOR FEATURE)
Target tree structure (data-driven):

- Budgets
  - Template
  - Current (Mar/2026)
  - 2009
  - 2010
  - ...
  - 2026
    - Dec
    - Nov
    - ...
    - Mar  (Current year months sorted DESC)

Creation process:
- User selects Year and Month.
- Clicks Create.
- System clones Template into a new month budget.
- New month appears in tree.

NEXT STEPS
1. Define Budget domain models.
2. Add Budget tables via migration.
3. Implement repository.
4. Build Template screen.
5. Build Monthly Budget screen.
6. Wire Budgets tree to actual data.
