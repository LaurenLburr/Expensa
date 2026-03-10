# CodexExpensa Catch-Up Document

## Current focus
Stabilize the core WinForms app and make the budgeting flow minimally usable before adding enhancements.

---

## Current application structure

### Navigation roots in MainForm
- Banks
- Accounts
- Payees
- Budgets

### Current UI pattern
- Left side: TreeView navigation
- Right side: hosted child form in `panelHost`
- Forms are opened through `ShowChildForm(...)`

---

## What is currently working

### Banks
- Banks root exists
- Bank nodes open `BankDetailsForm`
- `BanksLandingForm` exists and is wired
- Add Bank flow is working

### Accounts
- Accounts root exists
- Accounts are grouped by bank
- Account nodes open `AccountDetailsForm`
- `AccountsListForm` exists and is wired
- Add Checking Account flow is working
- Account details includes transaction tab

### Payees
- Payees root exists
- Payee nodes open `PayeeDetailsForm`
- `PayeesLandingForm` exists
- Search box exists in `PayeesLandingForm`
- Search filters payees by name as user types
- Grid header click sorting works
- Sort glyphs now work after setting column sort mode to `Programmatic`
- Current sortable columns:
  - PayeeName
  - SortIndex
  - IsActive
  - IncludeInBudgetTemplate
- Double-click and Enter open the selected payee
- Add Payee flow exists from the Payees root context menu
- Delete Payee flow exists from the Payee node context menu

### Budgets
- Budgets root exists
- Template node exists
- Current node exists
- Year/month scaffold exists
- `BudgetTemplateForm` exists
- `BudgetMonthForm` exists as placeholder / partial depending on current local code
- Budget screens are the current next major work area

---

## Important recent fixes

### Payee domain model
`Payee` had drifted out of sync with the database schema.

Current required shape should include:
- `PayeeId`
- `PayeeName`
- `IncludeInBudgetTemplate`
- `SortIndex`
- `IsActive`
- `WebsiteId` (nullable)

This was necessary because UI code now depends on:
- `SortIndex`
- `IsActive`

### PayeesLandingForm
Main fixes made:
- moved `_grid.ColumnHeaderMouseClick += ...` so it happens **after** `_grid` is created
- implemented full `ApplyFilter()` logic
- enabled sorting by clicking grid headers
- fixed sort glyph error by setting:
  - `col.SortMode = DataGridViewColumnSortMode.Programmatic`

### Search node
There was discussion about making the Payees `Search` tree node active.
Decision for now:
- leave the Search node in the tree
- do not focus on making it clever yet
- prioritize minimum usable behavior first

---

## Database / schema status

### Core business tables currently present
- `Bank`
- `Account`
- `Payee`
- `BudgetMonth`
- `BudgetMonthPayee`
- `Website`
- `WebsiteCredential`
- `WebQuestion`
- `SqlQuery`

### Staging/import tables
Old database data has been staged into `stage_` tables copied from the legacy DB schema.

Known stage tables include:
- `stage_BankAccount`
- `stage_Payee`
- `stage_Budget`
- `stage_BudgetTemplate`
- `stage_Web`
- `stage_WebQuestion`

### SqlQuery table
Current structure includes:
- `QueryIndex`
- `QueryName`
- `Description`
- `SqlText`
- generated `Fingerprint`
- `IsActive`

### Websites
Design direction is now:
- Websites are independent of payees
- Payee may optionally link to a `WebsiteId`
- login info is stored through `WebsiteCredential`
- `WebQuestion` belongs to `Website`

### Tags
Design direction agreed:
- one `Tag` table
- one `PayeeTag` table
- one `WebsiteTag` table
- missing tags should display as `None` when sorting/grouping by tag

Tag UI is **not built yet**.

---

## Legacy import work status

### Goal
Import old budget database into the new schema while preserving:
- Payee ↔ Budget links
- Account ↔ Budget links
- Payee ↔ Website links
- Website ↔ WebQuestion links

### Current import strategy
- old data copied into `stage_` tables first
- then transformed into new schema

### Key migration rule
Legacy IDs are converted into deterministic string IDs, for example:
- `legacy-payee-<old id>`
- `legacy-bankaccount-<old id>`
- `legacy-web-<old id>`

### Known migration issues encountered
- wrong attached DB path / wrong source DB
- duplicate website URLs causing `Website.Url` unique constraint errors
- duplicate bank `(BankName, RoutingNumber)` combinations
- duplicate `(WebsiteId, QuestionName)` in `WebQuestion`
- duplicate `PayeeName` values triggering unique constraint errors

### Current migration direction
Use duplicate-safe / deduplicating transforms rather than assuming clean source data.

---

## Tree behavior
User requested:
- tree nodes should start collapsed

Action taken / intended:
- remove explicit `.Expand()` calls in `BuildNavigationTree()`
- optionally call `treeNav.CollapseAll()` after building if needed

---

## Current priority order

### 1. Minimum usable budgeting flow
This is the immediate priority.

Need:
- workable `BudgetTemplateForm`
- way to create a budget month from template
- basic `BudgetMonthForm` that can display/edit planned amounts

### 2. Budget Template management
Current intent:
- manage template membership via payees
- `IncludeInBudgetTemplate` drives whether a payee is copied into a new budget month

### 3. Budget month creation
Minimum required behavior:
- create `BudgetMonth`
- copy all template payees into `BudgetMonthPayee`
- preserve account/payee links where applicable

### 4. Budget month editing
Minimum required columns:
- PayeeName
- SortIndex
- PlannedAmount
- possibly AccountId later

---

## Agreed design principles

### Minimum viable first
Current working rule:
- focus only on what is required to make the program usable
- defer enhancements until the basic flow works

### Avoid complexity until needed
Examples of things intentionally deferred:
- advanced Search node behavior
- website UI
- tag UI
- web question UI
- grouping by tag in grids
- richer budget logic
- non-essential automation

### Use deterministic legacy IDs for imports
This preserves relationships during migration without guessing.

---

## Immediate next steps

### Budget screens
1. Finish `BudgetTemplateForm`
   - show payees
   - allow template membership management
   - allow useful sort options

2. Add budget creation flow
   - create a month
   - copy template payees into `BudgetMonthPayee`

3. Finish `BudgetMonthForm`
   - show month rows
   - edit `PlannedAmount`
   - save cleanly

### After that
4. Revisit tag support
5. Revisit website management
6. Revisit Search node behavior

---

## Current known “good enough for now” decisions
- Leave Search node in tree, even if it is not smart yet
- Use in-memory filtering/sorting for Payees screen
- Keep UI changes minimal until budgeting is usable
- Keep import work staged and deterministic

---

## Files recently touched / discussed
- `MainForm.cs`
- `PayeesLandingForm.cs`
- `Payee.cs`
- `BudgetTemplateForm.cs`
- various migration SQL scripts
- stage-table transform scripts

---

## Notes for next session
Start with the budget screens, not websites or tags.

Best next concrete task:
- make `BudgetTemplateForm` fully usable
- then implement "Create Budget Month from Template"

That is the shortest path to making CodexExpensa genuinely usable.