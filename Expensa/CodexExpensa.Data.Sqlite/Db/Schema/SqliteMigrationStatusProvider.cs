using System;
using System.Collections.Generic;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Data.Sqlite.Db;

namespace CodexExpensa.Data.Sqlite.Db.Schema;

public sealed class SqliteMigrationStatusProvider : IMigrationStatusProvider
{
    private readonly MigrationRunner _runner;

    public SqliteMigrationStatusProvider(MigrationRunner runner)
    {
        _runner = runner ?? throw new ArgumentNullException(nameof(runner));
    }

    public System.Collections.Generic.IReadOnlyList<CodexExpensa.Core.Abstractions.MigrationStatusRow> GetStatus(
        CodexExpensa.Core.Abstractions.IDatabaseSession session)
    {
        if (session is null) throw new ArgumentNullException(nameof(session));
        if (session is not SqliteDatabase db)
            throw new ArgumentException("Requires SqliteDatabase session.", nameof(session));

        var rows = _runner.GetStatus(db);

        var result = new List<CodexExpensa.Core.Abstractions.MigrationStatusRow>(rows.Count);
        foreach (var r in rows)
        {
            result.Add(new CodexExpensa.Core.Abstractions.MigrationStatusRow
            {
                MigrationId = r.MigrationId,
                Status = r.Status
            });
        }

        return result;
    }
}