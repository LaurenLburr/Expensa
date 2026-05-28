using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public static class SqliteDataReaderExtensions
{
    public static string GetRequiredString(this SqliteDataReader reader, int ordinal)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.IsDBNull(ordinal))
        {
            throw new InvalidOperationException($"Expected a non-null string at ordinal {ordinal}.");
        }

        return reader.GetString(ordinal);
    }

    public static string GetRequiredString(this SqliteDataReader reader, string columnName)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        int ordinal = GetOrdinalOrDefault(reader, columnName, -1);

        if (ordinal < 0)
        {
            throw new InvalidOperationException($"Expected column '{columnName}' was not found.");
        }

        return reader.GetRequiredString(ordinal);
    }

    public static string GetStringOrDefault(this SqliteDataReader reader, int ordinal, string defaultValue = "")
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.IsDBNull(ordinal))
        {
            return defaultValue;
        }

        return reader.GetString(ordinal);
    }

    public static string GetStringOrDefault(this SqliteDataReader reader, string columnName, string defaultValue = "")
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        int ordinal = GetOrdinalOrDefault(reader, columnName, -1);

        if (ordinal < 0)
        {
            return defaultValue;
        }

        return reader.GetStringOrDefault(ordinal, defaultValue);
    }

    public static string? GetNullableString(this SqliteDataReader reader, int ordinal)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.IsDBNull(ordinal))
        {
            return null;
        }

        return reader.GetString(ordinal);
    }

    public static string? GetNullableString(this SqliteDataReader reader, string columnName)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        int ordinal = GetOrdinalOrDefault(reader, columnName, -1);

        if (ordinal < 0)
        {
            return null;
        }

        return reader.GetNullableString(ordinal);
    }

    public static bool GetBooleanOrDefault(this SqliteDataReader reader, int ordinal, bool defaultValue = false)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.IsDBNull(ordinal))
        {
            return defaultValue;
        }

        return reader.GetInt32(ordinal) != 0;
    }

    public static bool GetBooleanOrDefault(this SqliteDataReader reader, string columnName, bool defaultValue = false)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        int ordinal = GetOrdinalOrDefault(reader, columnName, -1);

        if (ordinal < 0)
        {
            return defaultValue;
        }

        return reader.GetBooleanOrDefault(ordinal, defaultValue);
    }

    public static int GetInt32OrDefault(this SqliteDataReader reader, string columnName, int defaultValue = 0)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        int ordinal = GetOrdinalOrDefault(reader, columnName, -1);

        if (ordinal < 0 || reader.IsDBNull(ordinal))
        {
            return defaultValue;
        }

        return reader.GetInt32(ordinal);
    }

    private static int GetOrdinalOrDefault(SqliteDataReader reader, string columnName, int defaultOrdinal)
    {
        try
        {
            return reader.GetOrdinal(columnName);
        }
        catch (ArgumentOutOfRangeException)
        {
            return defaultOrdinal;
        }
    }
}
