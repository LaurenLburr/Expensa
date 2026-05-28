namespace Codex.CommandEngine.Core;

public interface ICommandEngineRuntimeBootstrapper
{
    CommandEngineRuntimeBootstrapResult Bootstrap(
        CommandEngineRuntimeBootstrapRequest request);
}
