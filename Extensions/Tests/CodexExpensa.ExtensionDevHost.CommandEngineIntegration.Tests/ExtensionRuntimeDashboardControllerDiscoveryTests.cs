using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionRuntimeDashboardControllerDiscoveryTests
{
    [Fact]
    public void StartFromLoadedAssemblies_UsesManagerDiscovery()
    {
        ExtensionRuntimeManager manager =
            new(
                new ExtensionRuntimeManagerOptions
                {
                    ThrowIfNoCommands = true,
                    ContinueOnRegistrationError = true
                },
                new FakeDiscoveryService());

        ExtensionRuntimeDashboardController controller = new(manager);

        ExtensionRuntimeManagerSnapshot snapshot =
            controller.StartFromLoadedAssemblies();

        Assert.Equal(ExtensionRuntimeManagerStatus.Started, snapshot.Status);
        Assert.Single(snapshot.Host.Commands);
        Assert.Equal("dashboard.discovered.command", snapshot.Host.Commands[0].CommandName);
    }

    [Fact]
    public void ReloadFromFolder_UsesManagerDiscovery()
    {
        ExtensionRuntimeManager manager =
            new(
                new ExtensionRuntimeManagerOptions
                {
                    ThrowIfNoCommands = true,
                    ContinueOnRegistrationError = true
                },
                new FakeDiscoveryService());

        ExtensionRuntimeDashboardController controller = new(manager);

        ExtensionRuntimeManagerSnapshot snapshot =
            controller.ReloadFromFolder("C:\\Temp", recursive: true);

        Assert.Equal(ExtensionRuntimeManagerStatus.Started, snapshot.Status);
        Assert.Single(snapshot.Host.Commands);
        Assert.Equal("dashboard.discovered.command", snapshot.Host.Commands[0].CommandName);
    }

    private sealed class FakeDiscoveryService : IRuntimeProviderDiscoveryService
    {
        public RuntimeProviderDiscoveryResult DiscoverFromLoadedAssemblies()
        {
            return CreateResult();
        }

        public RuntimeProviderDiscoveryResult DiscoverFromAssembly(System.Reflection.Assembly assembly)
        {
            return CreateResult();
        }

        public RuntimeProviderDiscoveryResult DiscoverFromAssemblyFile(string assemblyPath)
        {
            return CreateResult();
        }

        public RuntimeProviderDiscoveryResult DiscoverFromFolder(string folderPath, bool recursive = false)
        {
            return CreateResult();
        }

        private static RuntimeProviderDiscoveryResult CreateResult()
        {
            return new RuntimeProviderDiscoveryResult
            {
                Providers =
                [
                    new StaticRuntimeCommandRegistrationProvider(
                    [
                        new RuntimeCommandRegistration
                        {
                            Handler = new TestCommandHandler("dashboard.discovered.command"),
                            DisplayName = "Dashboard Discovered Command",
                            Category = "Discovery"
                        }
                    ])
                ]
            };
        }
    }

    private sealed class TestCommandHandler : ICommandHandler
    {
        public TestCommandHandler(string commandName)
        {
            CommandName = commandName;
        }

        public string CommandName { get; }

        public Task<CommandExecutionResult> ExecuteAsync(
            CommandExecutionRequest request,
            ICommandExecutionContext context)
        {
            return Task.FromResult(
                CommandExecutionResult.Succeeded(
                    request.CommandName,
                    request.CorrelationId,
                    "Executed.",
                    "{\"ok\":true}"));
        }
    }
}
