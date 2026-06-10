using System.Data;
using System.Globalization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

internal static class SafeDataGridViewBinding
{
    public static void Attach(DataGridView grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        grid.DataError -= Grid_DataError;
        grid.DataError += Grid_DataError;
    }

    public static object? Sanitize(object? dataSource)
    {
        if (dataSource is DataTable table)
        {
            return Sanitize(table);
        }

        return dataSource;
    }

    public static DataTable Sanitize(DataTable table)
    {
        ArgumentNullException.ThrowIfNull(table);

        DataTable safeTable = new(table.TableName);

        foreach (DataColumn column in table.Columns)
        {
            Type safeType = ShouldForceString(column)
                ? typeof(string)
                : column.DataType;

            safeTable.Columns.Add(column.ColumnName, safeType);
        }

        foreach (DataRow sourceRow in table.Rows)
        {
            DataRow targetRow = safeTable.NewRow();

            foreach (DataColumn sourceColumn in table.Columns)
            {
                DataColumn targetColumn = safeTable.Columns[sourceColumn.ColumnName]
                    ?? throw new InvalidOperationException($"Missing copied column: {sourceColumn.ColumnName}");

                object? value = sourceRow[sourceColumn];

                targetRow[targetColumn] = targetColumn.DataType == typeof(string)
                    ? FormatAsString(value)
                    : value;
            }

            safeTable.Rows.Add(targetRow);
        }

        return safeTable;
    }

    private static bool ShouldForceString(DataColumn column)
    {
        if (string.Equals(column.ColumnName, "Image", StringComparison.OrdinalIgnoreCase) ||
            column.ColumnName.EndsWith("Image", StringComparison.OrdinalIgnoreCase) ||
            column.ColumnName.EndsWith("Icon", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return column.DataType == typeof(byte[]) ||
            column.DataType.FullName == "System.Drawing.Image" ||
            column.DataType.FullName == "System.Drawing.Bitmap";
    }

    private static string FormatAsString(object? value)
    {
        if (value is null || value is DBNull)
        {
            return string.Empty;
        }

        if (value is byte[] bytes)
        {
            return $"<{bytes.Length} bytes>";
        }

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static void Grid_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        e.ThrowException = false;
    }
}
