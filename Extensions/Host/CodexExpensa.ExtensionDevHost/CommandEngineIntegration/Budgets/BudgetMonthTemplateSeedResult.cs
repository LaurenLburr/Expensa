namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class BudgetMonthTemplateSeedResult
{
    public string DatabasePath { get; init; } = string.Empty;
    public string TableName { get; init; } = string.Empty;
    public int TemplateRowCount { get; init; }
    public int InsertedRowCount { get; init; }
    public int SkippedMonthCount { get; init; }
    public IReadOnlyList<int> Years { get; init; } = [];
    public IReadOnlyList<string> Warnings { get; init; } = [];

    public bool ChangedDatabase => InsertedRowCount > 0;
}
