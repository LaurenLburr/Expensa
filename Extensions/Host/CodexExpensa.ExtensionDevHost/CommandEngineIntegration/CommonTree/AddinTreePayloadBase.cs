using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

public abstract class AddinTreePayloadBase : IAddinTreePayload
{
    protected AddinTreePayloadBase(
        string addinName,
        AddinTreeNodeType nodeType,
        string nodeId,
        string displayText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(addinName);
        ArgumentException.ThrowIfNullOrWhiteSpace(nodeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayText);

        AddinName = addinName;
        NodeType = nodeType;
        NodeId = nodeId;
        DisplayText = displayText;
    }

    public string AddinName { get; }

    public AddinTreeNodeType NodeType { get; }

    public string NodeId { get; }

    public string DisplayText { get; }

    [JsonIgnore]
    public virtual string PayloadJson =>
        JsonSerializer.Serialize(
            ToPayloadSnapshot(),
            AddinTreeJsonSerializerOptions.Default);

    protected virtual object ToPayloadSnapshot()
    {
        return new
        {
            AddinName,
            NodeType,
            NodeId,
            DisplayText
        };
    }
}

public static class AddinTreeJsonSerializerOptions
{
    public static readonly JsonSerializerOptions Default =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };
}
