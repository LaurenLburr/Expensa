namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface ICommandExecutionPersistenceFileStore : ICommandExecutionPersistenceStore
{
    string DatabasePath { get; }
}
