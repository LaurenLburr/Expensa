using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionPersistenceFileStoreTests
{
    [Fact]
    public void SqliteStore_ExposesDatabasePath()
    {
        string path =
            Path.Combine(
                Path.GetTempPath(),
                "CommandExecutionPersistenceFileStoreTests",
                Guid.NewGuid().ToString("N"),
                "execution.db");

        SqliteCommandExecutionPersistenceStore store =
            new(new CommandExecutionPersistenceOptions
            {
                DatabasePath = path
            });

        Assert.IsAssignableFrom<ICommandExecutionPersistenceFileStore>(store);
        Assert.Equal(path, store.DatabasePath);
    }
}
