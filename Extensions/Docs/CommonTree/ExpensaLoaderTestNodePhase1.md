# Expensa Loader Test Node Phase 1

This slice adds an Extension Manager diagnostic surface that invokes add-ins the same way Expensa should:

```text
AssemblyDependencyResolver
AssemblyLoadContext
reflection-created smoke runner
reflection-created request
property-based result mapping
```

## New tool node

```text
Tools
    Expensa Add-in Loader Test
```

## Supported add-ins

```text
WebsitesAddin
BudgetsAddin
```

## Purpose

This tests the add-in runtime loader before wiring add-ins back into Expensa's main tree.

It intentionally uses the default Expensa database path:

```text
%LOCALAPPDATA%\CodexExpensa\db\codexexpensa.db
```

but the form also lets you browse to another `.db` file.
