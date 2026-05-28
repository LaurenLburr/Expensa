using Codex.CommandEngine.Abstractions;
using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class AiProviderRegistryTests
{
    [Fact]
    public void Register_WhenProviderIsNull_Throws()
    {
        AiProviderRegistry registry = new();

        Assert.Throws<ArgumentNullException>(() => registry.Register(null!));
    }

    [Fact]
    public void Register_WhenProviderKeyAlreadyExists_Throws()
    {
        AiProviderRegistry registry = new();
        TestAiProvider provider = new("test-provider", "Test Provider");

        registry.Register(provider);

        Assert.Throws<InvalidOperationException>(() => registry.Register(provider));
    }

    [Fact]
    public void TryResolve_WhenProviderIsRegistered_ReturnsProvider()
    {
        AiProviderRegistry registry = new();
        TestAiProvider provider = new("test-provider", "Test Provider");

        registry.Register(provider);

        bool resolved = registry.TryResolve("test-provider", out IAiProvider actual);

        Assert.True(resolved);
        Assert.Same(provider, actual);
    }

    [Fact]
    public void ListProviders_ReturnsDescriptorsOrderedByKindThenDisplayName()
    {
        AiProviderRegistry registry = new();
        registry.Register(new TestAiProvider("provider-b", "Beta", "OpenAI"));
        registry.Register(new TestAiProvider("provider-a", "Alpha", "Local"));

        IReadOnlyList<AiProviderDescriptor> providers = registry.ListProviders();

        Assert.Equal(2, providers.Count);
        Assert.Equal("provider-a", providers[0].ProviderKey);
        Assert.Equal("provider-b", providers[1].ProviderKey);
    }

    private sealed class TestAiProvider : IAiProvider
    {
        public TestAiProvider(string providerKey, string displayName, string providerKind = "Test")
        {
            Descriptor = new AiProviderDescriptor(providerKey, displayName, providerKind, "Concrete test AI provider.");
        }

        public AiProviderDescriptor Descriptor { get; }

        public Task<AiProviderResponse> ExecuteAsync(AiProviderRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Prompt);

            return Task.FromResult(new AiProviderResponse
            {
                ResponseText = $"Echo: {request.Prompt}",
                RawResponseJson = "{}",
                MetadataJson = "{}"
            });
        }
    }
}
