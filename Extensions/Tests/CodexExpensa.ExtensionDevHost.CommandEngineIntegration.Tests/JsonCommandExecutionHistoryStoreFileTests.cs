using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class JsonCommandExecutionHistoryStoreFileTests
{
    [Fact]
    public void Constructor_StoresHistoryPath()
    {
        string path = Path.Combine(Path.GetTempPath(), "CommandExecutionHistoryFileTests", Guid.NewGuid().ToString("N"), "history.json");
        JsonCommandExecutionHistoryStore store = new(path);
        Assert.Equal(path, store.HistoryPath);
    }

    [Fact]
    public void JsonStore_ImplementsFileStore()
    {
        ICommandExecutionHistoryStore store = new JsonCommandExecutionHistoryStore(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "history.json"));
        Assert.IsAssignableFrom<ICommandExecutionHistoryFileStore>(store);
    }

    [Fact]
    public void DefaultHistoryPath_EndsWithExpectedFileName()
    {
        string path = JsonCommandExecutionHistoryStore.GetDefaultHistoryPath();

        Assert.EndsWith(
            Path.Combine("Expensa", "Extensions", "CommandEngineExecutionHistory.json"),
            path,
            StringComparison.OrdinalIgnoreCase);
    }
}
