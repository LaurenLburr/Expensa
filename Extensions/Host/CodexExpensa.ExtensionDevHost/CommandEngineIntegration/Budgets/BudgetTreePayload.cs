using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class BudgetTreePayload : AddinTreePayloadBase
{
    public const string Addin = "BudgetsAddin";

    public BudgetTreePayload(
        AddinTreeNodeType nodeType,
        string nodeId,
        string displayText)
        : base(Addin, nodeType, nodeId, displayText)
    {
    }

    public int? BudgetYear { get; init; }

    public int? BudgetMonth { get; init; }

    public string MonthKey { get; init; } = string.Empty;
}
