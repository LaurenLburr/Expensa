using System;
using System.Collections.Generic;
using CodexExpensa.ExtensionDevHost.Commands.Abstractions;

namespace CodexExpensa.ExtensionDevHost.Commands.Runtime;

public sealed class CommandServiceProvider : ICommandServiceProvider
{
    private readonly Dictionary<Type, object> _services = new();

    public void Register<TService>(TService instance)
        where TService : class
    {
        _services[typeof(TService)] = instance ?? throw new ArgumentNullException(nameof(instance));
    }

    public TService GetRequiredService<TService>()
        where TService : class
    {
        if (_services.TryGetValue(typeof(TService), out object? service))
            return (TService)service;

        throw new InvalidOperationException(
            $"Required command service '{typeof(TService).FullName}' is not registered.");
    }

    public TService? GetService<TService>()
        where TService : class
    {
        return _services.TryGetValue(typeof(TService), out object? service)
            ? (TService)service
            : null;
    }
}
