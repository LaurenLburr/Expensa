# PayeesAddin – Catch-Up

## Current Status

Initial tree add-in starter.

## Purpose

Load Expensa payees into the common add-in tree.

## Current Tree

```text
Payees
└── By Name
    └── Payee
```

## Database

Reads one of these tables:

```text
Payee
Payees
```

## Next Steps

- Wire PayeesAddin into Extension Manager aggregate loader.
- Add Payees database/test panel.
- Add tag grouping after the base by-name tree works.
