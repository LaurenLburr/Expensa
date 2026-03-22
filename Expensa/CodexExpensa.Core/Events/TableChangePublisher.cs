using System;

namespace CodexExpensa.Core.Events;

public sealed class TableChangePublisher : ITableChangePublisher
{
    public event EventHandler<TableChangedEventArgs>? TableChanged;

    public void Raise(TableChangedEventArgs args)
    {
        TableChanged?.Invoke(this, args);
    }
}