namespace CodexExpensa.Core.Abstractions;

/// <summary>
/// Represents the active database session for the application.
/// This is the single abstraction used by the UI and infrastructure.
/// </summary>
public interface IDatabaseSession
{
    /// <summary>
    /// Flush in-memory data to disk.
    /// File-based databases may treat this as a no-op.
    /// </summary>
    void Save();

    /// <summary>
    /// True when the database is running in memory with a persisted backing file.
    /// </summary>
    bool IsInMemory { get; }

    /// <summary>
    /// Full path to the persisted database file if one exists.
    /// </summary>
    string? PersistedFilePath { get; }
}