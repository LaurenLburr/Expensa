namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface ICommandExecutionHistoryStore
{
    void Add(
        CommandExecutionHistoryRecord record);

    IReadOnlyList<CommandExecutionHistoryRecord> ListAll();

    void Clear();
}
