namespace CodexExpensa.Core.Abstractions;

/// <summary>
/// Provides migration status information without exposing any SQLite types to the UI.
/// </summary>
public interface IMigrationStatusProvider
{
    IReadOnlyList<MigrationStatusRow> GetStatus(IDatabaseSession session);
}