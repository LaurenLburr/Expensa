namespace Codex.CommandEngine.Data;

public sealed class DatabaseSchemaException : InvalidOperationException
{
    public DatabaseSchemaException(string message)
        : base(message)
    {
    }
}
