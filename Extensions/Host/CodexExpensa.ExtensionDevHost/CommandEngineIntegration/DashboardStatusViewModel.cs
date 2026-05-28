namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class DashboardStatusViewModel
{
    public string Mode { get; init; } = "Runtime";

    public string RuntimeStatus { get; init; } = string.Empty;

    public int RowCount { get; init; }

    public bool Recursive { get; init; }

    public string FolderPath { get; init; } = string.Empty;

    public bool ShowingDuplicates { get; init; }

    public string Summary =>
        $"Mode: {Mode} | Status: {RuntimeStatus} | Rows: {RowCount} | Recursive: {Recursive} | Duplicates: {ShowingDuplicates}";
}
