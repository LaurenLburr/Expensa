# Common Tree Framework Phase 1

This slice introduces the shared add-in tree foundation.

## Goal

All add-ins should eventually use one common tree framework:

```text
AddinTreeProviderBase<TPayload>
    LoadAsync
    FillAsync
    ModifyAsync
    DeleteAsync
```

The add-in-specific payload should be the only specialized part:

```text
WebsiteTreePayload
BudgetTreePayload
```

## New common namespace

```text
CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree
```

## New core contracts

```text
IAddinTreePayload
AddinTreePayloadBase
AddinTreeNode<TPayload>
AddinTreeProviderBase<TPayload>
AddinTreeRenderer
AddinTreePayloadReader
AddinTreeOperationResult
```

## Migration plan

1. Add common framework. This slice.
2. Migrate Budgets to `AddinTreeProviderBase<BudgetTreePayload>`.
3. Migrate Websites to `AddinTreeProviderBase<WebsiteTreePayload>`.
4. Remove old duplicate renderer/invoker/provider classes once both add-ins are stable.
