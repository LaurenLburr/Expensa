using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class RuntimeProviderDiscoveryServiceTests
{
    [Fact]
    public void DiscoverFromAssembly_FindsProviderWithParameterlessConstructor()
    {
        RuntimeProviderDiscoveryService service = new();

        RuntimeProviderDiscoveryResult result =
            service.DiscoverFromAssembly(typeof(TestDiscoveryProvider).Assembly);

        Assert.Contains(result.Providers, static provider => provider.GetType() == typeof(TestDiscoveryProvider));
    }

    [Fact]
    public void DiscoverFromAssembly_ReportsProviderWithoutParameterlessConstructor()
    {
        RuntimeProviderDiscoveryService service = new();

        RuntimeProviderDiscoveryResult result =
            service.DiscoverFromAssembly(typeof(ProviderWithoutParameterlessConstructor).Assembly);

        Assert.Contains(result.Issues, static issue => issue.Source.Contains(nameof(ProviderWithoutParameterlessConstructor), StringComparison.Ordinal));
    }

    [Fact]
    public void DiscoverFromFolder_WhenFolderMissing_ReturnsIssue()
    {
        RuntimeProviderDiscoveryService service = new();

        RuntimeProviderDiscoveryResult result =
            service.DiscoverFromFolder(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));

        Assert.Empty(result.Providers);
        Assert.Single(result.Issues);
        Assert.Contains("Folder was not found", result.Issues[0].Message);
    }

    [Fact]
    public void Format_WhenIssuesExist_ReturnsReadableText()
    {
        RuntimeProviderDiscoveryResult result = new()
        {
            Issues =
            [
                new RuntimeProviderDiscoveryIssue
                {
                    Source = "Test",
                    Message = "Something failed."
                }
            ]
        };

        string text =
            RuntimeProviderDiscoveryTextFormatter.Format(result);

        Assert.Contains("Runtime Provider Discovery", text);
        Assert.Contains("Something failed.", text);
    }

    public sealed class TestDiscoveryProvider : IRuntimeCommandRegistrationProvider
    {
        public IReadOnlyList<RuntimeCommandRegistration> GetRegistrations()
        {
            return [];
        }
    }

    public sealed class ProviderWithoutParameterlessConstructor : IRuntimeCommandRegistrationProvider
    {
        public ProviderWithoutParameterlessConstructor(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public IReadOnlyList<RuntimeCommandRegistration> GetRegistrations()
        {
            return [];
        }
    }
}
