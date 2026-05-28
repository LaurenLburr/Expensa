using System.Text;

namespace Codex.CommandEngine.App;

public static class TextViewFormatter
{
    public static string FormatSection(string title, IEnumerable<KeyValuePair<string, string?>> values)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(values);

        StringBuilder builder = new();

        builder.AppendLine(title);
        builder.AppendLine(new string('=', title.Length));
        builder.AppendLine();

        foreach (KeyValuePair<string, string?> value in values)
        {
            builder.Append(value.Key);
            builder.Append(": ");
            builder.AppendLine(string.IsNullOrWhiteSpace(value.Value) ? "(blank)" : value.Value);
        }

        return builder.ToString();
    }

    public static string FormatRecords<T>(
        string title,
        IReadOnlyList<T> records,
        Func<T, IEnumerable<KeyValuePair<string, string?>>> valueFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(records);
        ArgumentNullException.ThrowIfNull(valueFactory);

        StringBuilder builder = new();

        builder.AppendLine(title);
        builder.AppendLine(new string('=', title.Length));
        builder.AppendLine();
        builder.AppendLine($"Count: {records.Count}");
        builder.AppendLine();

        for (int index = 0; index < records.Count; index++)
        {
            builder.AppendLine($"[{index + 1}]");
            builder.AppendLine();

            foreach (KeyValuePair<string, string?> value in valueFactory(records[index]))
            {
                builder.Append(value.Key);
                builder.Append(": ");
                builder.AppendLine(string.IsNullOrWhiteSpace(value.Value) ? "(blank)" : value.Value);
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }
}
