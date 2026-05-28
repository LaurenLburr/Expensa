namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class DashboardStatusTextFormatter
{
    public static string Format(DashboardStatusViewModel status)
    {
        ArgumentNullException.ThrowIfNull(status);

        return
            $"Mode: {status.Mode}    " +
            $"Status: {status.RuntimeStatus}    " +
            $"Rows: {status.RowCount}    " +
            $"Recursive: {status.Recursive}    " +
            $"Duplicates: {status.ShowingDuplicates}";
    }
}
