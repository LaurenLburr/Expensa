using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface IExtensionRuntimeManager
{
    ExtensionRuntimeManagerStatus Status { get; }

    void Start(IEnumerable<IRuntimeCommandRegistrationProvider> providers);
    void StartFromLoadedAssemblies();
    void StartFromFolder(string folderPath, bool recursive = false);
    void StartFromManifests(string folderPath, bool recursive = false);

    void Reload(IEnumerable<IRuntimeCommandRegistrationProvider> providers);
    void ReloadFromLoadedAssemblies();
    void ReloadFromFolder(string folderPath, bool recursive = false);
    void ReloadFromManifests(string folderPath, bool recursive = false);

    void Stop();

    ExtensionRuntimeManagerSnapshot GetSnapshot();

    IReadOnlyList<RuntimeCommandDescriptor> ListCommands();

    Task<CommandExecutionResult> ExecuteCommandAsync(
        string commandName,
        IReadOnlyDictionary<string, object?>? parameters = null,
        string contextJson = "{}",
        CancellationToken cancellationToken = default);
}
