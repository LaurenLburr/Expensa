namespace Codex.CommandEngine.Core;

public interface IExtensionCommandRuntimeBootstrapper
{
    ExtensionCommandRuntimeBootstrapResult Bootstrap(
        ExtensionCommandRuntimeBootstrapRequest request);
}
