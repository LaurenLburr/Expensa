namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteTagAssignmentResult
{
    public required string TagId { get; init; }

    public required string TagName { get; init; }

    public required string WebsiteId { get; init; }

    public bool AssignmentCreated { get; init; }
}
