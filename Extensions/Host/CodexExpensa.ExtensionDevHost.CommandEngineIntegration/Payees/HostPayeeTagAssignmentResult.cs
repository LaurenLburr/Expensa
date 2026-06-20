namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Payees;

public sealed class HostPayeeTagAssignmentResult
{
    public string TagId { get; init; } = string.Empty;
    public string TagName { get; init; } = string.Empty;
    public string PayeeId { get; init; } = string.Empty;
    public bool AssignmentCreated { get; init; }
}
