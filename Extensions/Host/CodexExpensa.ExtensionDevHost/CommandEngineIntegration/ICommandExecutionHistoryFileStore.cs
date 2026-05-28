namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface ICommandExecutionHistoryFileStore : ICommandExecutionHistoryStore
{
    string HistoryPath { get; }
}
