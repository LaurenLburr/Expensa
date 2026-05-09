namespace CodexExpensa.ExtensionDevHost.Commands.Abstractions;

public interface ICommandServiceProvider
{
    TService GetRequiredService<TService>() where TService : class;

    TService? GetService<TService>() where TService : class;
}
