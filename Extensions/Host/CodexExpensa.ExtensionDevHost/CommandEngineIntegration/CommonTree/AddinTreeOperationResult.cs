namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

public sealed class AddinTreeOperationResult
{
    public required bool Succeeded { get; init; }

    public required string Message { get; init; }

    public string NodeId { get; init; } = string.Empty;

    public static AddinTreeOperationResult Success(
        string message,
        string nodeId = "")
    {
        return new AddinTreeOperationResult
        {
            Succeeded = true,
            Message = message,
            NodeId = nodeId
        };
    }

    public static AddinTreeOperationResult Failure(
        string message,
        string nodeId = "")
    {
        return new AddinTreeOperationResult
        {
            Succeeded = false,
            Message = message,
            NodeId = nodeId
        };
    }
}
