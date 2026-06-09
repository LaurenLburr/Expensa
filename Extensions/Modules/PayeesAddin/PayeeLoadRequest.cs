namespace PayeesAddin;

public sealed class PayeeLoadRequest
{
    public string DatabasePath { get; init; } = string.Empty;
    public string SearchText { get; init; } = string.Empty;
    public bool IncludeInactive { get; init; }
    public int MaximumRows { get; init; } = 500;
}
