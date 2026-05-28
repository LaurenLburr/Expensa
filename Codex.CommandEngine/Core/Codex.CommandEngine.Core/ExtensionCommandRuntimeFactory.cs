namespace Codex.CommandEngine.Core;

public static class ExtensionCommandRuntimeFactory
{
    public static IExtensionCommandRuntime Create(
        IEnumerable<RuntimeCommandRegistration> registrations)
    {
        ArgumentNullException.ThrowIfNull(registrations);

        CommandEngineRuntimeBootstrapResult bootstrapResult =
            new CommandEngineRuntimeBuilder()
                .AddCommands(registrations)
                .Build();

        return new ExtensionCommandRuntime(bootstrapResult.Runtime);
    }

    public static IExtensionCommandRuntime CreateRequired(
        IEnumerable<RuntimeCommandRegistration> registrations)
    {
        ArgumentNullException.ThrowIfNull(registrations);

        CommandEngineRuntimeBootstrapResult bootstrapResult =
            new CommandEngineRuntimeBuilder()
                .AddCommands(registrations)
                .BuildRequired();

        return new ExtensionCommandRuntime(bootstrapResult.Runtime);
    }

    public static IExtensionCommandRuntime CreateFromProviders(
        IEnumerable<IRuntimeCommandRegistrationProvider> providers)
    {
        return BootstrapFromProviders(new ExtensionCommandRuntimeBootstrapRequest
        {
            Providers = providers.ToList()
        }).Runtime;
    }

    public static IExtensionCommandRuntime CreateRequiredFromProviders(
        IEnumerable<IRuntimeCommandRegistrationProvider> providers)
    {
        return BootstrapFromProviders(new ExtensionCommandRuntimeBootstrapRequest
        {
            Providers = providers.ToList(),
            ThrowIfNoCommands = true
        }).Runtime;
    }

    public static ExtensionCommandRuntimeBootstrapResult BootstrapFromProviders(
        ExtensionCommandRuntimeBootstrapRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new ExtensionCommandRuntimeBootstrapper()
            .Bootstrap(request);
    }
}
