using System;

namespace CodexExpensa.Core.Events;

public interface ITableChangePublisher
{
    event EventHandler<TableChangedEventArgs>? TableChanged;

    void Raise(TableChangedEventArgs args);
}