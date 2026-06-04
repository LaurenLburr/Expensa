using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;

public sealed class ExpensaAddinTreePayload : AddinTreePayloadBase
{
    public ExpensaAddinTreePayload(
        string addinName,
        AddinTreeNodeType nodeType,
        string nodeId,
        string displayText)
        : base(addinName, nodeType, nodeId, displayText)
    {
    }

    public ExpensaAddinLoaderKind LoaderKind { get; init; }

    public string WebsiteId { get; init; } = string.Empty;

    public string Url { get; init; } = string.Empty;

    public string TagName { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;

    public int? BudgetYear { get; init; }

    public int? BudgetMonth { get; init; }

    public string MonthKey { get; init; } = string.Empty;

    protected override object ToPayloadSnapshot()
    {
        return new
        {
            AddinName,
            NodeType,
            NodeId,
            DisplayText,
            LoaderKind,
            WebsiteId,
            Url,
            TagName,
            IsActive,
            BudgetYear,
            BudgetMonth,
            MonthKey
        };
    }
}
