using System;

namespace CodexExpensa.Core.Events;

public sealed class TableChangedEventArgs : EventArgs
{
    public required string TableName { get; init; }
    public required TableChangeOperation Operation { get; init; }
    public required DateTime OccurredUtc { get; init; }

    public string? KeyValue { get; init; }
    public string? Source { get; init; }
    public string? Summary { get; init; }
}