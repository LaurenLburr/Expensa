using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class RuntimeCommandRegistrationProviderTests
{
    [Fact]
    public void StaticProvider_ReturnsConfiguredRegistrations()
    {
        StaticRuntimeCommandRegistrationProvider provider =
            new(
            [
                new RuntimeCommandRegistration
                {
                    Handler = new TestCommandHandler("extension.one"),
                    DisplayName = "Extension One",
                    Category = "Extension"
                }
            ]);

        IReadOnlyList<RuntimeCommandRegistration> registrations =
            provider.GetRegistrations();

        Assert.Single(registrations);
        Assert.Equal("extension.one", registrations[0].Handler.CommandName);
    }

    [Fact]
    public void ProviderCollection_CombinesProvidersInOrder()
    {
        RuntimeCommandRegistrationProviderCollection collection =
            new RuntimeCommandRegistrationProviderCollection()
                .AddProvider(new StaticRuntimeCommandRegistrationProvider(
                [
                    new RuntimeCommandRegistration
                    {
                        Handler = new TestCommandHandler("extension.one")
                    }
                ]))
                .AddProvider(new StaticRuntimeCommandRegistrationProvider(
                [
                    new RuntimeCommandRegistration
                    {
                        Handler = new TestCommandHandler("extension.two")
                    }
                ]));

        IReadOnlyList<RuntimeCommandRegistration> registrations =
            collection.GetRegistrations();

        Assert.Equal(2, registrations.Count);
        Assert.Equal("extension.one", registrations[0].Handler.CommandName);
        Assert.Equal("extension.two", registrations[1].Handler.CommandName);
    }

    [Fact]
    public async Task ExtensionCommandRuntimeFactory_CreateRequiredFromProviders_BuildsExecutableRuntime()
    {
        IExtensionCommandRuntime runtime =
            ExtensionCommandRuntimeFactory.CreateRequiredFromProviders(
            [
                new StaticRuntimeCommandRegistrationProvider(
                [
                    new RuntimeCommandRegistration
                    {
                        Handler = new TestCommandHandler("extension.run"),
                        DisplayName = "Run Extension",
                        Category = "Extension"
                    }
                ])
            ]);

        CommandExecutionResult result =
            await runtime.ExecuteCommandAsync(new ExtensionRuntimeCommandRequest
            {
                CommandName = "extension.run",
                CorrelationId = "correlation-001"
            });

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
    }

    [Fact]
    public void ExtensionCommandRuntimeFactory_CreateRequiredFromProviders_WhenNoRegistrations_Throws()
    {
        Assert.Throws<InvalidOperationException>(
            () => ExtensionCommandRuntimeFactory.CreateRequiredFromProviders(
            [
                new StaticRuntimeCommandRegistrationProvider([])
            ]));
    }

    private sealed class TestCommandHandler : ICommandHandler
    {
        public TestCommandHandler(string commandName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

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
