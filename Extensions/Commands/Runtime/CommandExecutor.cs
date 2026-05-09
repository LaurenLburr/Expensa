using System;
using CodexExpensa.ExtensionDevHost.Commands.Abstractions;

namespace CodexExpensa.ExtensionDevHost.Commands.Runtime;

public sealed class CommandExecutor
{
    public void Execute(IExtMgrCommand command, ICommandContext context)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        if (context == null)
            throw new ArgumentNullException(nameof(context));

        command.Execute(context);
    }
}
