# Expensa Loader Aggregate Tree Phase 3

The previous loader test could load Websites or Budgets, but the UI still behaved like a single-add-in replacement view.

This slice changes the loader test to load both add-ins under one aggregate root:

```text
Expensa Add-ins
    Websites
        ...
    Budgets
        ...
```

## Important behavior

The tree is cleared once.

Then both add-ins are invoked through the same Expensa-style loader.

The parsed roots are appended under one shared parent node.
