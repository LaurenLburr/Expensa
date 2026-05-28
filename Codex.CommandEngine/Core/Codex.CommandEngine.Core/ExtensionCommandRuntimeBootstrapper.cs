namespace Codex.CommandEngine.Core;

public sealed class ExtensionCommandRuntimeBootstrapper : IExtensionCommandRuntimeBootstrapper
{
    public ExtensionCommandRuntimeBootstrapResult Bootstrap(
        ExtensionCommandRuntimeBootstrapRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        IReadOnlyList<RuntimeCommandRegistration> registrations =
            new RuntimeCommandRegistrationProviderCollection()
                .AddProviders(request.Providers)
                .GetRegistrations();

        CommandEngineRuntimeBootstrapResult runtimeBootstrapResult =
            new CommandEngineRuntimeBootstrapper()
                .Bootstrap(new CommandEngineRuntimeBootstrapRequest
                {
                    Commands = registrations,
                    ThrowIfNoCommands = request.ThrowIfNoCommands,
                    ContinueOnRegistrationError = request.ContinueOnRegistrationError
                });

        return new ExtensionCommandRuntimeBootstrapResult
        {
            Runtime = new ExtensionCommandRuntime(runtimeBootstrapResult.Runtime),
            RegisteredCommands = runtimeBootstrapResult.RegisteredCommands,
            Diagnostics = runtimeBootstrapResult.Diagnostics
        };
    }
}
