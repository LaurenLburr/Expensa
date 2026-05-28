namespace Codex.CommandEngine.Core;

public interface ICommandMetadataProvider
{
    IReadOnlyList<CommandMetadataRecord> GetCommandMetadata();
}
