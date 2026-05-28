using System.Data;

namespace Codex.CommandEngine.Data;

public static class DataTableMapper
{
    public static IReadOnlyList<T> MapRows<T>(
        DataTable table,
        Func<DataRow, T> mapper)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(mapper);

        List<T> results = [];

        foreach (DataRow row in table.Rows)
        {
            results.Add(mapper(row));
        }

        return results;
    }

    public static T? MapSingleOrDefault<T>(
        DataTable table,
        Func<DataRow, T> mapper)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(mapper);

        if (table.Rows.Count == 0)
        {
            return null;
        }

        return mapper(table.Rows[0]);
    }
}
