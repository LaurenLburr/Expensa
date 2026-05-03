using System;

namespace CodexExpensa.ExtensionDevHost.Commands.Abstractions;

public static class CommandContextExtensions
{
    public static TService GetRequiredService<TService>(this ICommandContext context)
        where TService : class
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return context.Services.GetRequiredService<TService>();
    }

    public static TService? GetService<TService>(this ICommandContext context)
        where TService : class
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return context.Services.GetService<TService>();
    }
}
