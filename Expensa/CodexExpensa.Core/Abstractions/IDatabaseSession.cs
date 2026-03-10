using System.Data;
using System.Data.Common;

namespace CodexExpensa.Core.Abstractions;

public interface IDatabaseSession
{
    void Save();

    bool IsInMemory { get; }

    string? PersistedFilePath { get; }

    DataTable QueryDataTable(
        string queryName,
        IEnumerable<DbParameter>? parameters = null);

    void Execute(
        string queryName,
        IEnumerable<DbParameter>? parameters = null);
}