using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionRuntimeDashboardControllerManifestTests
{
    [Fact]
    public void StartFromManifests_CallsManagerManifestPath()
    {
        FakeManager manager = new();
        ExtensionRuntimeDashboardController controller = new(manager);

        controller.StartFromManifests("C:\\Temp", recursive: true);

        Assert.Equal("C:\\Temp", manager.LastManifestFolder);
        Assert.True(manager.LastManifestRecursive);
    }

    [Fact]
    public void ReloadFromManifests_CallsManagerManifestPath()
    {
        FakeManager manager = new();
        ExtensionRuntimeDashboardController controller = new(manager);

        controller.ReloadFromManifests("C:\\Temp", recursive: false);

        Assert.Equal("C:\\Temp", manager.LastReloadManifestFolder);
        Assert.False(manager.LastReloadManifestRecursive);
    }

    private sealed class FakeManager : IExtensionRuntimeManager
    {
        public ExtensionRuntimeManagerStatus Status => ExtensionRuntimeManagerStatus.NotStarted;
        public string LastManifestFolder { get; private set; } = string.Empty;
        public bool LastManifestRecursive { get; private set; }
        public string LastReloadManifestFolder { get; private set; } = string.Empty;
        public bool LastReloadManifestRecursive { get; private set; }

        public void Start(IEnumerable<IRuntimeCommandRegistrationProvider> providers) { }
        public void StartFromLoadedAssemblies() { }
        public void StartFromFolder(string folderPath, bool recursive = false) { }

        public void StartFromManifests(string folderPath, bool recursive = false)
        {
            LastManifestFolder = folderPath;
            LastManifestRecursive = recursive;
        }

        public void Reload(IEnumerable<IRuntimeCommandRegistrationProvider> providers) { }
        public void ReloadFromLoadedAssemblies() { }
        public void ReloadFromFolder(string folderPath, bool recursive = false) { }

        public void ReloadFromManifests(string folderPath, bool recursive = false)
        {
            LastReloadManifestFolder = folderPath;
            LastReloadManifestRecursive = recursive;
        }

        public void Stop() { }
        public ExtensionRuntimeManagerSnapshot GetSnapshot() => new();
        public IReadOnlyList<RuntimeCommandDescriptor> ListCommands() => [];

        public Task<CommandExecutionResult> ExecuteCommandAsync(
            string commandName,
            IReadOnlyDictionary<string, object?>? parameters = null,
            string contextJson = "{}",
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
