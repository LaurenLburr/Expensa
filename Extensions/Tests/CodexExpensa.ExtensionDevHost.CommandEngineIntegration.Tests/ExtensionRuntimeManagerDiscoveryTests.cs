using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionRuntimeManagerDiscoveryTests
{
    [Fact]
    public void StartFromLoadedAssemblies_UsesDiscoveryService()
    {
        ExtensionRuntimeManager manager =
            new(
                new ExtensionRuntimeManagerOptions
                {
                    ThrowIfNoCommands = true,
                    ContinueOnRegistrationError = true
                },
                new FakeDiscoveryService());

        manager.StartFromLoadedAssemblies();

        ExtensionRuntimeManagerSnapshot snapshot =
            manager.GetSnapshot();

        Assert.Equal(ExtensionRuntimeManagerStatus.Started, snapshot.Status);
        Assert.Single(snapshot.Host.Commands);
        Assert.Equal("discovered.command", snapshot.Host.Commands[0].CommandName);
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
                            Handler = new TestCommandHandler("discovered.command"),
                            DisplayName = "Discovered Command",
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
