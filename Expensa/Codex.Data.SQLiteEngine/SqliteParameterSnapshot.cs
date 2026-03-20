namespace Codex.Data.SQLiteEngine;

public sealed class SqliteParameterSnapshot
{
    public required string Name { get; init; }

    public object? Value { get; init; }

    public string? DbType { get; init; }

    public override string ToString()
    {
        return $"{Name}={Value ?? "NULL"}";
    }
}
