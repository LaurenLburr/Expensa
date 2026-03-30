# Expensa Catch-Up – Tags System Stabilization

## Overview

This update stabilizes the Tag system for Accounts, addressing persistence, UI behavior, and data integrity issues.

The system is now functionally consistent across:
- Database
- UI (TagAssignmentControl)
- TreeView navigation
- AccountDetailsForm integration

---

## Key Fixes

### 1. Tag Persistence (Critical Fix)

Problem:
Tags appeared assigned but were not saved to disk

Fix:
Added _db.Save() call on tag change event

Location:
AccountDetailsForm.cs

```csharp
private void OnTagsChanged(object? sender, EventArgs e)
{
    _db.Save();
    _onSaved();
}
```

---

### 2. Event Wiring Fix

Problem:
TagsChanged event was incorrectly wired

Fix:
Replaced inline lambda with proper EventHandler

---

### 3. Available Tags List Empty

Problem:
Left-side tag list was empty unless user typed search text

Fix:
Now loads all tags when search is blank

---

### 4. Duplicate Tag Handling

Problem:
Same tag could appear multiple times and be assigned multiple times

Fixes:
- UI-level deduplication
- Database constraint enforced:

```sql
CONSTRAINT IX_AccountTag UNIQUE (AccountId, TagId)
```

- Insert uses:

```sql
INSERT OR IGNORE INTO AccountTag ...
```

---

### 5. Tag Assignment Stability

Now behaves correctly:
- Assign tag → saved immediately
- Remove tag → saved immediately
- No duplicate rows
- UI reflects actual DB state

---

## Database Design Notes

### AccountTag Table

```sql
CREATE TABLE AccountTag (
    AccountTagId TEXT PRIMARY KEY,
    AccountId TEXT NOT NULL,
    TagId TEXT NOT NULL,
    SortIndex INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (AccountId) REFERENCES Account (AccountId),
    FOREIGN KEY (TagId) REFERENCES Tag (TagId),
    CONSTRAINT IX_AccountTag UNIQUE (AccountId, TagId)
);
```

---

## Architecture Decisions

Save is triggered at form level, not control level.

Reason:
Keeps TagAssignmentControl reusable and avoids hidden persistence.

---

## SQL Catalog Usage

All queries continue to use the SqlQuery table.

---

## Known Remaining Issues

- TreeView collapses after tag assignment
- Tag navigation inconsistencies

---

## Next Steps

- Preserve TreeView expansion state
- Fix tag navigation
- Improve grouping

---

## Summary

The Tag system is now:
- Persisting correctly
- Free of duplicates
- UI-synchronized
- Structurally sound

Remaining work is UI polish.

---

## Confidence

High — core system is stable.
