using System.Diagnostics;

namespace CodexExpensa.ExtensionDevHost.Commands.Services;

public sealed class DebugCommandLogger : ICommandLogger
{
    public void Log(string message)
    {
        Debug.WriteLine($"[Command] {message}");
    }
}