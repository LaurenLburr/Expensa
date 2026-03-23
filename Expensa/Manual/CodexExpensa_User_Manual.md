# CodexExpensa User Manual

## Overview

CodexExpensa is a desktop checkbook-style financial tracking application.

It is designed to help you:

- Track accounts and balances
- Record transactions
- Manage payees
- Build and review budgets

The application uses a local SQLite database stored on your machine.

---

## Getting Started

### First Launch

When you first start CodexExpensa:

- A local database is created (if it does not exist)
- Required tables are automatically initialized
- You begin with an empty system

---

## Navigation

The left panel contains the navigation tree with four main sections:

- Banks
- Accounts
- Payees
- Budgets

Clicking a node loads the corresponding screen.

---

## Banks

### What a Bank Is

A Bank represents a financial institution.

### Adding a Bank

Right-click **Banks** → *Add Bank*

Enter:

- Bank Name
- Routing Number
- Optional URL
- Active status

---

## Accounts

### What an Account Is

An Account belongs to a Bank and represents:

- Checking
- Savings (future support)

### Adding an Account

Right-click **Accounts** or a specific Bank → *Add Checking Account*

Enter:

- Account Nickname
- Account Number
- Active status

---

## Account Details

Selecting an account opens the Account Details screen.

From here you can:

- Edit account information
- View transactions
- Add new transactions

---

## Transactions

### What a Transaction Is

A Transaction represents money moving in or out of an account.

### Adding a Transaction

From Account Details:

- Enter amount
- Select payee
- Add optional notes

### Transaction Behavior

- Positive = income
- Negative = expense

---

## Payees

### What a Payee Is

A Payee is who you pay or receive money from.

### Adding a Payee

Right-click **Payees** → *Add Payee*

---

### Payee Views

#### Name View
- Shows all payees
- Supports filtering by name

#### Tag View
- Groups payees by tags
- Includes a "None" group for untagged payees

---

## Budgets

### Budget Types

- Template
- Current Month
- Historical Months

### Template

Defines recurring expected expenses.

### Current

Represents the current month’s budget.

### Historical

Past months organized by year.

---

## Saving Data

Use:

**File → Save**

This writes the in-memory database to disk.

---

## Refreshing Data

Use:

**View → Refresh**

This reloads all data from the database.

---

## Database Behavior

- Runs in-memory for speed
- Periodically saved to disk
- Located in:

```
%LOCALAPPDATA%\CodexExpensa\db\codexexpensa.db
```

---

## Tips

- Use Payees consistently for cleaner reports later
- Keep account nicknames meaningful
- Save frequently during early use
- Expand nodes in the tree to drill into data

---

## Known Limitations (Current Version)

- No reporting dashboards yet
- Limited account types
- No import/export
- Minimal validation on inputs

---

## Troubleshooting

### App won’t start
- Database migration may have failed
- Restart the app

### Data missing
- Ensure you saved changes
- Use Refresh

### UI behaving oddly
- Restart app (resets in-memory state)

---

## Future Features (Planned)

- Reports and analytics
- Import from bank files
- Advanced budgeting tools
- Improved transaction categorization

---

## Summary

CodexExpensa is designed to be:

- Fast
- Local-first
- Simple but extensible

It grows with your data and workflow.
