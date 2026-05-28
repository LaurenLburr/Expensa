using System.Data;
using System.Globalization;

namespace Codex.CommandEngine.Data;

public static class DataRowExtensions
{
    public static string GetRequiredString(this DataRow row, string columnName)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        if (!row.Table.Columns.Contains(columnName))
        {
            throw new InvalidOperationException($"Expected column '{columnName}' was not found.");
        }

        object value = row[columnName];

        if (value == DBNull.Value)
        {
            throw new InvalidOperationException($"Expected a non-null value for column '{columnName}'.");
        }

        return Convert.ToString(value, CultureInfo.InvariantCulture)
            ?? throw new InvalidOperationException($"Column '{columnName}' could not be converted to a string.");
    }

    public static string GetStringOrDefault(this DataRow row, string columnName, string defaultValue = "")
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        if (!row.Table.Columns.Contains(columnName))
        {
            return defaultValue;
        }

        object value = row[columnName];

        if (value == DBNull.Value)
        {
            return defaultValue;
        }

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? defaultValue;
    }

    public static string? GetNullableString(this DataRow row, string columnName)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        if (!row.Table.Columns.Contains(columnName))
        {
            return null;
        }

        object value = row[columnName];

        if (value == DBNull.Value)
        {
            return null;
        }

        return Convert.ToString(value, CultureInfo.InvariantCulture);
    }

    public static int GetInt32OrDefault(this DataRow row, string columnName, int defaultValue = 0)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        if (!row.Table.Columns.Contains(columnName))
        {
            return defaultValue;
        }

        object value = row[columnName];

        if (value == DBNull.Value)
        {
            return defaultValue;
        }

        return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    public static bool GetBooleanOrDefault(this DataRow row, string columnName, bool defaultValue = false)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        if (!row.Table.Columns.Contains(columnName))
        {
            return defaultValue;
        }

        object value = row[columnName];

        if (value == DBNull.Value)
        {
            return defaultValue;
        }

        if (value is bool booleanValue)
        {
            return booleanValue;
        }

        if (value is string textValue)
        {
            if (bool.TryParse(textValue, out bool parsedBoolean))
            {
                return parsedBoolean;
            }

            if (int.TryParse(textValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedInteger))
            {
                return parsedInteger != 0;
            }

            return defaultValue;
        }

        return Convert.ToInt32(value, CultureInfo.InvariantCulture) != 0;
    }
}
