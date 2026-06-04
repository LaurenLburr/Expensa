using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;
using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class BudgetTreeProvider : AddinTreeProviderBase<BudgetTreePayload>
{
    private readonly HostBudgetRuntimeModuleInvoker invoker;

    public BudgetTreeProvider()
        : this(new HostBudgetRuntimeModuleInvoker())
    {
    }

    public BudgetTreeProvider(
        HostBudgetRuntimeModuleInvoker invoker)
        : base(BudgetTreePayload.Addin)
    {
        ArgumentNullException.ThrowIfNull(invoker);

        this.invoker = invoker;
    }

    public override async Task<IReadOnlyList<AddinTreeNode<BudgetTreePayload>>> LoadAsync(
        AddinTreeLoadRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        CommandExecutionResult executionResult =
            await invoker.ExecuteAsync(
                new HostBudgetRuntimeLoadRequest
                {
                    SearchText = request.SearchText,
                    IncludeClosed = request.IncludeInactive,
                    MaximumRows = request.MaximumRows
                },
                cancellationToken).ConfigureAwait(true);

        return ParseBudgetTreeNodes(
            executionResult.OutputJson);
    }

    public override Task<AddinTreeOperationResult> FillAsync(
        BudgetTreePayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return Task.FromResult(
            AddinTreeOperationResult.Success(
                $"Loaded {payload.DisplayText}.",
                payload.NodeId));
    }

    public override Task<AddinTreeOperationResult> ModifyAsync(
        BudgetTreePayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return Task.FromResult(
            AddinTreeOperationResult.Failure(
                "Budget modify is not implemented yet.",
                payload.NodeId));
    }

    public override Task<AddinTreeOperationResult> DeleteAsync(
        BudgetTreePayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return Task.FromResult(
            AddinTreeOperationResult.Failure(
                "Budget delete is not implemented yet.",
                payload.NodeId));
    }

    private static IReadOnlyList<AddinTreeNode<BudgetTreePayload>> ParseBudgetTreeNodes(
        string outputJson)
    {
        if (string.IsNullOrWhiteSpace(outputJson))
        {
            return [];
        }

        using JsonDocument document =
            JsonDocument.Parse(outputJson);

        if (!document.RootElement.TryGetProperty("nodes", out JsonElement nodesElement) ||
            nodesElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        List<AddinTreeNode<BudgetTreePayload>> nodes =
            [];

        foreach (JsonElement nodeElement in nodesElement.EnumerateArray())
        {
            nodes.Add(
                ReadBudgetNode(nodeElement));
        }

        return nodes;
    }

    private static AddinTreeNode<BudgetTreePayload> ReadBudgetNode(
        JsonElement element)
    {
        List<AddinTreeNode<BudgetTreePayload>> children =
            [];

        if (element.TryGetProperty("children", out JsonElement childrenElement) &&
            childrenElement.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement childElement in childrenElement.EnumerateArray())
            {
                children.Add(
                    ReadBudgetNode(childElement));
            }
        }

        string nodeTypeText =
            GetString(element, "nodeType");

        BudgetTreePayload payload =
            new(
                ParseNodeType(nodeTypeText),
                GetString(element, "nodeId"),
                GetString(element, "displayText"))
            {
                BudgetYear = GetNullableInt(element, "budgetYear"),
                BudgetMonth = GetNullableInt(element, "budgetMonth"),
                MonthKey = GetString(element, "monthKey")
            };

        return CreateNode(
            payload,
            children);
    }

    private static AddinTreeNodeType ParseNodeType(
        string nodeType)
    {
        return nodeType switch
        {
            "BudgetYear" => AddinTreeNodeType.BudgetYear,
            "BudgetMonth" => AddinTreeNodeType.BudgetMonth,
            "BudgetRoot" => AddinTreeNodeType.Root,
            _ => AddinTreeNodeType.Unknown
        };
    }

    private static string GetString(
        JsonElement element,
        string propertyName)
    {
        return element.TryGetProperty(propertyName, out JsonElement property) &&
            property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? string.Empty
                : string.Empty;
    }

    private static int? GetNullableInt(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property) ||
            property.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return null;
        }

        return property.TryGetInt32(out int value)
            ? value
            : null;
    }
}
