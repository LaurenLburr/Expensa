# PayeesAddin Phase 1

Adds the initial `PayeesAddin` module.

## Tree

```text
Payees
└── By Name
    └── Payee row
```

## Runtime command

```text
Payees.LoadTree
```

## Important

Discard the earlier `VendorsAddin` slice. Payees are the correct existing Expensa concept.

## Next slice

Wire `PayeesAddin` into Extension Manager's aggregate add-in loader and deploy/test flow.
