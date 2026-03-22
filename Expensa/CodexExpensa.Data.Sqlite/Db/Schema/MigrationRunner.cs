using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Codex.Data.SQLiteEngine;
using CodexExpensa.Db.Schema;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.Data.Sqlite.Db.Schema;

public sealed class MigrationRunner
{
    private const string MigrationsFolderToken = ".Migrations.";
    private const string MigrationExtension = ".mig";

    public IReadOnlyList<MigrationInfo> GetAvailableMigrations()
    {
        var asm = typeof(SchemaMarker).Assembly;

        var resourceNames = asm.GetManifestResourceNames()
            .Where(n => n.Contains(MigrationsFolderToken, StringComparison.OrdinalIgnoreCase)
                     && n.EndsWith(MigrationExtension, StringComparison.OrdinalIgnoreCase))
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var list = new List<MigrationInfo>(resourceNames.Count);

        foreach (var resourceName in resourceNames)
        {
            var fileName = ExtractFileName(resourceName);
            var migrationId = RemoveKnownExtension(fileName);

            list.Add(new MigrationInfo(migrationId, resourceName));
        }

        return list;
    }

    public IReadOnlyList<string> GetAppliedMigrationIds(SqliteDatabase db)
    {
        if (db is null) throw new ArgumentNullException(nameof(db));

        EnsureSchemaMigrationsTable(db);

        const string sql = """
SELECT MigrationId
FROM SchemaMigrations
ORDER BY MigrationId;
""";

        return db.Query(sql, r => r.GetString(0));
    }

    public IReadOnlyList<MigrationStatusRow> GetStatus(SqliteDatabase db)
    {
        if (db is null) throw new ArgumentNullException(nameof(db));

        var availableInfos = GetAvailableMigrations();
        var availableById = availableInfos.ToDictionary(x => x.MigrationId, StringComparer.OrdinalIgnoreCase);
        var applied = GetAppliedMigrationIds(db).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var allIds = availableById.Keys
            .Union(applied)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase);

        var result = new List<MigrationStatusRow>();

        foreach (var id in allIds)
        {
            if (!availableById.ContainsKey(id) && applied.Contains(id))
            {
                result.Add(new MigrationStatusRow
                {
                    MigrationId = id,
                    Status = "NonExistent"
                });
                continue;
            }

            if (availableById.TryGetValue(id, out var info))
            {
                if (applied.Contains(id))
                {
                    result.Add(new MigrationStatusRow
                    {
                        MigrationId = id,
                        Status = "Applied"
                    });
                    continue;
                }

                var sql = ReadEmbeddedText(typeof(SchemaMarker).Assembly, info.ResourceName);
                var isBlank = string.IsNullOrWhiteSpace(sql);

                result.Add(new MigrationStatusRow
                {
                    MigrationId = id,
                    Status = isBlank ? "Blank" : "Pending"
                });

                continue;
            }

            result.Add(new MigrationStatusRow
            {
                MigrationId = id,
                Status = "NonExistent"
            });
        }

        return result;
    }

    public MigrationRunResult ApplyPendingMigrations(SqliteDatabase db)
    {
        if (db is null) throw new ArgumentNullException(nameof(db));

        EnsureSchemaMigrationsTable(db);

        var available = GetAvailableMigrations();
        var applied = new HashSet<string>(GetAppliedMigrationIds(db), StringComparer.OrdinalIgnoreCase);

        var pending = available
            .Where(m => !applied.Contains(m.MigrationId))
            .OrderBy(m => m.MigrationId, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (pending.Count == 0)
            return new MigrationRunResult(0, Array.Empty<string>(), Array.Empty<string>());

        var appliedNow = new List<string>();
        var skippedBlank = new List<string>();

        foreach (var mig in pending)
        {
            var didApply = TryApplySingleMigration(db, mig);
            if (didApply)
                appliedNow.Add(mig.MigrationId);
            else
                skippedBlank.Add(mig.MigrationId);
        }

        return new MigrationRunResult(appliedNow.Count, appliedNow, skippedBlank);
    }

    private static bool TryApplySingleMigration(SqliteDatabase db, MigrationInfo mig)
    {
        var asm = typeof(SchemaMarker).Assembly;
        var sql = ReadEmbeddedText(asm, mig.ResourceName);

        if (string.IsNullOrWhiteSpace(sql))
            return false;

        db.ExecuteInTransaction((ISqliteTransactionScope tx) =>
        {
            db.ExecuteNonQuery(sql, parameters: null, tx);

            const string insert = """
INSERT INTO SchemaMigrations (MigrationId, AppliedUtc, Checksum)
VALUES ($id, $utc, $sum);
""";

            var p = new[]
            {
                new SqliteParameter("$id", mig.MigrationId),
                new SqliteParameter("$utc", DateTime.UtcNow.ToString("O")),
                new SqliteParameter("$sum", Sha256Hex(sql)),
            };

            db.ExecuteNonQuery(insert, p, tx);
        });

        return true;
    }

    private static void EnsureSchemaMigrationsTable(SqliteDatabase db)
    {
        const string sql = """
CREATE TABLE IF NOT EXISTS SchemaMigrations (
  MigrationId TEXT NOT NULL PRIMARY KEY,
  AppliedUtc  TEXT NOT NULL,
  Checksum    TEXT NULL
);
""";

        db.ExecuteNonQuery(sql);
    }

    private static string ExtractFileName(string resourceName)
    {
        var markerIndex = resourceName.LastIndexOf(MigrationsFolderToken, StringComparison.OrdinalIgnoreCase);
        if (markerIndex >= 0)
        {
            var start = markerIndex + MigrationsFolderToken.Length;
            return resourceName[start..];
        }

        return resourceName;
    }

    private static string RemoveKnownExtension(string fileName)
    {
        if (fileName.EndsWith(MigrationExtension, StringComparison.OrdinalIgnoreCase))
            return fileName[..^MigrationExtension.Length];

        return Path.GetFileNameWithoutExtension(fileName);
    }

    private static string ReadEmbeddedText(Assembly asm, string resourceName)
    {
        using var stream = asm.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException($"Embedded resource not found: {resourceName}");

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return reader.ReadToEnd();
    }

    private static string Sha256Hex(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text ?? string.Empty);
        var hash = SHA256.HashData(bytes);

        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
            sb.Append(b.ToString("x2"));

        return sb.ToString();
    }
}

public sealed record MigrationInfo(string MigrationId, string ResourceName);

public sealed record MigrationRunResult(
    int AppliedCount,
    IReadOnlyList<string> AppliedMigrationIds,
    IReadOnlyList<string> SkippedBlankMigrationIds);
