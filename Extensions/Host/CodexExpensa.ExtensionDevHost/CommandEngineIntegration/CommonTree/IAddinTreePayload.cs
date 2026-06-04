namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

public interface IAddinTreePayload
{
    string AddinName { get; }

    AddinTreeNodeType NodeType { get; }

    string NodeId { get; }

    string DisplayText { get; }

    string PayloadJson { get; }
}
