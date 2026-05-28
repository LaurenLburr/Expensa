using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

internal static class SqliteDataReaderExtensions
{
    public static bool HasColumn(this SqliteDataReader reader, string columnName)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        for (int ordinal = 0; ordinal < reader.FieldCount; ordinal++)
        {
            if (string.Equals(reader.GetName(ordinal), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static string GetRequiredString(this SqliteDataReader reader, string columnName)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        int ordinal = reader.GetRequiredOrdinal(columnName);

        if (reader.IsDBNull(ordinal))
        {
            throw new InvalidOperationException($"Required string column '{columnName}' was NULL.");
        }

        string value = reader.GetString(ordinal);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Required string column '{columnName}' was blank.");
        }

        return value;
    }

    public static string GetStringOrDefault(
        this SqliteDataReader reader,
        string columnName,
        string defaultValue = "")
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        int ordinal = reader.GetOptionalOrdinal(columnName);

        if (ordinal < 0 || reader.IsDBNull(ordinal))
        {
            return defaultValue;
        }

        return reader.GetString(ordinal);
    }

    public static string? GetNullableString(this SqliteDataReader reader, string columnName)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        int ordinal = reader.GetOptionalOrdinal(columnName);

        if (ordinal < 0 || reader.IsDBNull(ordinal))
        {
            return null;
        }

        return reader.GetString(ordinal);
    }

    public static bool GetBooleanOrDefault(
        this SqliteDataReader reader,
        string columnName,
        bool defaultValue = false)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        int ordinal = reader.GetOptionalOrdinal(columnName);

        if (ordinal < 0 || reader.IsDBNull(ordinal))
        {
            return defaultValue;
        }

        object value = reader.GetValue(ordinal);

        return value switch
        {
            bool boolValue => boolValue,
            int intValue => intValue != 0,
            long longValue => longValue != 0,
            string stringValue => string.Equals(stringValue, "true", StringComparison.OrdinalIgnoreCase)
                || stringValue == "1",
            _ => Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture) != 0
        };
    }

    public static int GetInt32OrDefault(
        this SqliteDataReader reader,
        string columnName,
        int defaultValue = 0)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        int ordinal = reader.GetOptionalOrdinal(columnName);

        if (ordinal < 0 || reader.IsDBNull(ordinal))
        {
            return defaultValue;
        }

        return reader.GetInt32(ordinal);
    }

    public static int GetRequiredInt32(this SqliteDataReader reader, string columnName)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        int ordinal = reader.GetRequiredOrdinal(columnName);

        if (reader.IsDBNull(ordinal))
        {
            throw new InvalidOperationException($"Required integer column '{columnName}' was NULL.");
        }

        return reader.GetInt32(ordinal);
    }

    private static int GetRequiredOrdinal(this SqliteDataReader reader, string columnName)
    {
        int ordinal = reader.GetOptionalOrdinal(columnName);

        if (ordinal < 0)
        {
            throw new InvalidOperationException($"Required column '{columnName}' was not returned by the query.");
        }

        return ordinal;
    }

    private static int GetOptionalOrdinal(this SqliteDataReader reader, string columnName)
    {
        for (int ordinal = 0; ordinal < reader.FieldCount; ordinal++)
        {
            if (string.Equals(reader.GetName(ordinal), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return ordinal;
            }
        }

        return -1;
    }
}
